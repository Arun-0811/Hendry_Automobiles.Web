using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        public IBrandRepository Brand { get; }

        Task SaveAsync();
    }
}
