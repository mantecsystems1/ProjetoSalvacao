namespace Biblia.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; set; }

    protected Entity() => Id = Guid.NewGuid();

    protected Entity(Guid id) => Id = id;
}
