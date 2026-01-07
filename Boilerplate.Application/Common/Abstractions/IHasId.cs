namespace Boilerplate.Application.Common.Abstractions;

/// <summary>
/// Marker interface for DTOs that have an Id property.
/// Enables simplified Created() helper usage in controllers.
/// </summary>
public interface IHasId
{
    Guid Id { get; }
}
