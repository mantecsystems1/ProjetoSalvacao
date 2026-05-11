namespace Biblia.Application.Abstractions;

public interface ICurrentUser
{
    Guid? UserId { get; }
}
