using System.Linq.Expressions;
using Model;

namespace ModulePersons.Interfaces
{
    /// <summary>
    /// Имитация БД
    /// </summary>
    internal interface IPersonsStore
    {
        Task<IPerson> Add(IPerson person);

        Task<IPerson> Update(IPerson person);

        Task<IPerson> Delete(IPerson person);

        Task<IEnumerable<IPerson>> GetAll(Expression<Func<IPerson, bool>>? filter = null);

        Task<IPerson> GetById(long id);
    }
}
