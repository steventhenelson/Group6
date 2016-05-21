using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ChocAn.EfData;

namespace ChocAn.Repositories
{
    // this generic class acts as the repository for the class type it is initialized as. 
    // it performs the generalized CRUD functions to and from the database
    public class GenericRepository<TEntityType> where TEntityType : class
    {
        internal ChocAnDb Context;
        internal DbSet<TEntityType> DatabaseSet;

        public GenericRepository(ChocAnDb context)
        {
            Context = context;
            DatabaseSet = context.Set<TEntityType>();
        }

        public virtual IEnumerable<TEntityType> Retrieve()
        {
            return DatabaseSet.ToList();
        }

        public virtual TEntityType GetEntityById(object id)
        {
            return DatabaseSet.Find(id);
        }

        public virtual void AddEntity(TEntityType entity)
        {
            DatabaseSet.Add(entity);
        }

        public virtual void UpdateEntity(TEntityType entity)
        {
            DatabaseSet.Attach(entity);
            Context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }
    }
}