using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks.Dataflow;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Data.Metrics.ProcessQueue;
using Orion.Core.Server.Interfaces.Services.System;


namespace Orion.Server.Services.System;

public class ProcessQueueService : IProcessQueueService
{
    public int MaxParallelTask { get; set; }
    public ProcessQueueConfig Config { get; set; }

    public IObservable<ProcessQueueMetric> GetMetrics => _metricsSubject.AsObservable();

    private readonly ILogger _logger;

    private readonly ConcurrentDictionary<string, ActionBlock<Func<Task>>> _queues;
    private readonly ConcurrentDictionary<string, ProcessStats> _stats;
    private readonly Subject<ProcessQueueMetric> _metricsSubject;
    private readonly CancellationTokenSource _globalCts;
    private readonly IDisposable _metricsSubscription;
    private readonly Dictionary<string, Func<Task>> _contextExecutors = new();


    private readonly ConcurrentQueue<Action> _mainThreadQueue = new();

    // private readonly SemaphoreSlim _mainThreadSemaphore = new(1);

    public ProcessQueueService(ILogger<ProcessQueueService> logger, OrionServerConfig config)
    {
        Config = config.Process.ProcessQueue;
        _logger = logger;
        _queues = new ConcurrentDictionary<string, ActionBlock<Func<Task>>>();
        _stats = new ConcurrentDictionary<string, ProcessStats>();
        _metricsSubject = new Subject<ProcessQueueMetric>();
        _globalCts = new CancellationTokenSource();

        _metricsSubscription = Observable
            .Interval(TimeSpan.FromSeconds(1))
            .Subscribe(_ => EmitMetrics());
    }

    public Task Enqueue(string context, Action action, CancellationToken cancellationToken = default)
    {
        return Enqueue(
            context,
            () =>
            {
                action();
                return Task.CompletedTask;
            },
            cancellationToken
        );
    }

    // Overload per Func<T>
    public Task<T> Enqueue<T>(string context, Func<T> func, CancellationToken cancellationToken = default)
    {
        return Enqueue(context, () => Task.FromResult(func()), cancellationToken);
    }

    // Overload per Func<Task>
    public Task Enqueue(string context, Func<Task> func, CancellationToken cancellationToken = default)
    {
        var queue = GetOrCreateQueue(context);
        var stats = GetOrCreateStats(context);

        var tcs = new TaskCompletionSource();


        _logger.LogDebug("Enqueueing task for context {Context} current {Size}", context, stats.QueuedItems);


        queue.Post(
            async () =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _globalCts.Token);
                    if (cts.Token.IsCancellationRequested)
                    {
                        tcs.SetCanceled(cts.Token);
                        return;
                    }

                    await func();
                    stats.IncrementProcessed(sw.Elapsed);
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    stats.IncrementFailed();
                    tcs.SetException(ex);
                    _logger.LogError(ex, "Failed to process task for context {Context}", context);
                }
            }
        );

        stats.IncrementQueued();
        return tcs.Task;
    }

    public Task<T> Enqueue<T>(string context, Func<Task<T>> func, CancellationToken cancellationToken = default)
    {
        var queue = GetOrCreateQueue(context);
        var stats = GetOrCreateStats(context);

        var tcs = new TaskCompletionSource<T>();

        _logger.LogDebug("Enqueueing task for context {Context} current {Size}", context, queue.InputCount);

        queue.Post(
            async () =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _globalCts.Token);
                    if (cts.Token.IsCancellationRequested)
                    {
                        tcs.SetCanceled(cancellationToken);
                        return;
                    }

                    var result = await func();
                    stats.IncrementProcessed(sw.Elapsed);
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    stats.IncrementFailed();
                    tcs.SetException(ex);
                    _logger.LogError(ex, "Failed to process task for context {Context}", context);
                }
            }
        );

        stats.IncrementQueued();
        return tcs.Task;
    }


    public void EnqueueOnMainThread(Action action)
    {
        var stats = GetOrCreateStats("main_thread");

        stats.IncrementQueued();


        _logger.LogDebug("Enqueueing action on main thread");
        //   _mainThreadSemaphore.Wait();

        _mainThreadQueue.Enqueue(action);
        //  _mainThreadSemaphore.Release();
    }

    public void ProcessMainThreadQueue()
    {
        var stats = GetOrCreateStats("main_thread");

        if (_mainThreadQueue.TryDequeue(out var action))
        {
            var startTime = Stopwatch.GetTimestamp();
            try
            {
                action();
            }
            catch (Exception ex)
            {
                stats.IncrementFailed();
                _logger.LogError(ex, "Failed to process main thread action");
            }
            finally
            {
                var elapsed = Stopwatch.GetElapsedTime(startTime);
                stats.IncrementProcessed(elapsed);
            }
        }
    }

    public void EnqueueOnMainThread<T>(Func<T> func)
    {
        EnqueueOnMainThread(new Action(() => func()));
    }

    public void EnqueueOnMainThread<T>(Func<Task<T>> func)
    {
        EnqueueOnMainThread(new Action(async () => await func()));
    }

    public void EnqueueOnMainThread(Func<Task> func)
    {
        EnqueueOnMainThread(new Action(async () => await func()));
    }

    public void EnsureContext(string context)
    {
        GetOrCreateQueue(context);
        GetOrCreateStats(context);
    }


    private ActionBlock<Func<Task>> GetOrCreateQueue(string context)
    {
        context = context.ToLower();

        return _queues.GetOrAdd(
            context,
            _ => new ActionBlock<Func<Task>>(
                async task => { await task(); },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = MaxParallelTask,
                    CancellationToken = _globalCts.Token
                }
            )
        );
    }

    private ProcessStats GetOrCreateStats(string context)
    {
        return _stats.GetOrAdd(context, _ => new ProcessStats());
    }

    private void EmitMetrics()
    {
        foreach (var (context, stats) in _stats)
        {
            var metric = new ProcessQueueMetric(
                context,
                stats.QueuedItems,
                stats.ProcessedItems,
                stats.FailedItems,
                stats.AverageProcessingTime
            );
            _metricsSubject.OnNext(metric);
        }
    }

    public void Dispose()
    {
        _metricsSubscription.Dispose();
        _metricsSubject.Dispose();
        _globalCts.Dispose();

        foreach (var queue in _queues.Values)
        {
            queue.Complete();
        }

        _queues.Clear();

        _mainThreadQueue.Clear();

        _stats.Clear();

        GC.SuppressFinalize(this);
    }


    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        MaxParallelTask = Config.MaxParallelTask;

        _logger.LogInformation("Process queue service started with {MaxParallelTask} parallel tasks", MaxParallelTask);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
