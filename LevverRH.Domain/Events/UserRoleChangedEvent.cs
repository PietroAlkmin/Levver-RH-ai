using LevverRH.Domain.Enums;

namespace LevverRH.Domain.Events;

public class UserRoleChangedEvent
{
    public Guid UserId { get; }
    public UserRole RoleAnterior { get; }
    public UserRole RoleNova { get; }
    public Guid AlteradoPorUserId { get; }
    public DateTime DataHora { get; }

    public UserRoleChangedEvent(
   Guid userId,
        UserRole roleAnterior,
   UserRole roleNova,
        Guid alteradoPorUserId)
    {
        UserId = userId;
      RoleAnterior = roleAnterior;
      RoleNova = roleNova;
  AlteradoPorUserId = alteradoPorUserId;
   DataHora = DateTime.UtcNow;
  }
}
