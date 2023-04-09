namespace JobBoards.Data.Entities.Common;

public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; private set; }

    protected Entity(TId id)
    {
        Id = id;
    }
#pragma warning disable CS8618
    protected Entity()
    {
    }
#pragma warning restore CS8618
}