namespace BibliaOnline.Application.Abstractions;

public interface ICurrentUser
{
    Guid? UserId { get; }
}
