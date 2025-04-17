using System.Collections.Concurrent;

namespace Orion.Core.Pool;

public class ObjectPool<T> where T : IDisposable, new()
{
    private readonly ConcurrentQueue<T> _pool;
    private readonly int _initialSize;
    private readonly SemaphoreSlim _semaphore;

    public ObjectPool(int initialSize = 10)
    {
        if (initialSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialSize), "Initial size must be greater than zero.");
        }

        _initialSize = initialSize;
        _pool = new ConcurrentQueue<T>();
        _semaphore = new SemaphoreSlim(initialSize, int.MaxValue);

        ExpandPool();
    }

    private void ExpandPool()
    {
        for (int i = 0; i < _initialSize; i++)
        {
            T obj = new T();
            _pool.Enqueue(obj);
            _semaphore.Release();
        }
    }

    public T Get()
    {
        _semaphore.Wait();

        if (!_pool.TryDequeue(out T? obj))
        {
            lock (_pool)
            {
                // Double-check in case another thread expanded the pool
                if (!_pool.TryDequeue(out obj))
                {
                    ExpandPool();
                    _pool.TryDequeue(out obj);
                }
            }
        }

        return obj!;
    }

    public async Task<T> GetAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        if (!_pool.TryDequeue(out T? obj))
        {
            lock (_pool)
            {
                // Double-check in case another thread expanded the pool
                if (!_pool.TryDequeue(out obj))
                {
                    ExpandPool();
                    _pool.TryDequeue(out obj);
                }
            }
        }

        return obj!;
    }

    public void Return(T obj)
    {
        _pool.Enqueue(obj);
        _semaphore.Release();
    }

    public void Dispose()
    {
        while (_pool.TryDequeue(out T? obj))
        {
            obj.Dispose();
        }

        _semaphore.Dispose();
    }
}
