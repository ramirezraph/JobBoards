using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Persistence.Repositories.Common;

public interface IRepository<TEntity> where TEntity : Entity<Guid>
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(Guid id);
    Task AddAsync(TEntity entity);
    Task RemoveAsync(TEntity entity);
}