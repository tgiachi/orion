using Orion.Foundations.Types;

namespace Orion.Core.Server.Internal.Services;

/// <summary>
///  ServiceDefinitionObject is a record that represents a service definition.
/// </summary>
/// <param name="ServiceType"></param>
/// <param name="ImplementationType"></param>
/// <param name="Lifetime"></param>
/// <param name="IsAutoStart"></param>
public record ServiceDefinitionObject(
    Type ServiceType,
    Type ImplementationType,
    ServiceLifetimeType Lifetime,
    bool IsAutoStart,
    int Priority = 0
);
