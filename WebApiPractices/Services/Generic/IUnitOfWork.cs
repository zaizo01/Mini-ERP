using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractices.Services.Generic
{
    public interface IUnitOfWork: IDisposable
    {
        ApplicationDbContext Context { get;  }
        void Commit();
    }

    public class UnitOfWork : IUnitOfWork
    {
        public ApplicationDbContext Context { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            Context = context;
        }

        public void Commit()
        {
            Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context.DisposeAsync();
        }
    }
}
