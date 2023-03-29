using JobBoards.Data.Entities.Common;

namespace JobBoards.Data.Persistence.Repositories;

public interface IRepository<T> where T : Entity<Guid>
{
    IEnumerable<T> GetAll();
    T? GetById(Guid Id);
    T? Add(T entity);
    T? Update(Guid Id, T updatedEntity);
    T? Delete(Guid Id);
}