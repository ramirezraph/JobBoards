namespace JobBoards.Data.Entities.Common;

public abstract class Entity<TId> where TId : notnull
{
    public Guid Id { get; private set; }

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
    }
}