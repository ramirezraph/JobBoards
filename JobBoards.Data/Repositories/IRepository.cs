using JobBoards.Data.Entities.Interfaces;

namespace JobBoards.Data.Repositories;

public interface IRepository<T> where T : IEntity
{
    IEnumerable<T> GetAll();
    T? GetById(Guid Id);
    T? Add(T entity);
    T? Update(Guid Id, T updatedEntity);
    T? Delete(Guid Id);
}