using System.Collections.Generic;

namespace CustomerService.Business
{
    public interface IBizManager<TEntity>
        where TEntity : class
    {
        IList<TEntity> GetAll();
        TEntity GetByID(string id);
        void Add(TEntity entity);
        bool DeleteByID(string id);
        void UpdateByID(string id, TEntity entity);
    }
}