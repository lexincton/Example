using System.Linq.Expressions;
using Model;

namespace ModulePersons.API
{
    public interface IServicePersonsMonolit
    {
        Task<IPerson> Add(IPerson person);

        Task<IPerson> Update(IPerson person);

        Task<IPerson> Delete(IPerson person);

        Task<IEnumerable<IPerson>> GetAll(Expression<Func<IPerson, bool>>? filter = null);

        Task<IPerson> GetById(long id);
    }
}
