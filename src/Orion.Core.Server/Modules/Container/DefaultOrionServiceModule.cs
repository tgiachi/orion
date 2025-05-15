using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Services;

namespace Orion.Core.Server.Modules.Container;

public class DefaultOrionServiceModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services
            .AddService<ITextTemplateService, TextTemplateService>()
            .AddService<ISchedulerSystemService, SchedulerSystemService>()
            .AddService<IEventDispatcherService, EventDispatcherService>();
    }
}
