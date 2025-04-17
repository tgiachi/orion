using HyperCube.Postman.Base.Events;
using HyperCube.Postman.Interfaces.Events;

namespace Orion.Core.Server.Events.Scheduler;

public abstract record AddSchedulerJobEvent(string Name, TimeSpan TotalSpan, Func<Task> Action) : BasePostmanRecordEvent;
