using System.Linq.Expressions;
using Model;

namespace ModuleLamps.Objects
{
    public interface IServiceLampsMonolit
    {
        Task<ILamp> Add(ILamp person);

        Task<ILamp> Update(ILamp person);

        Task<ILamp> Delete(ILamp person);

        Task<IEnumerable<ILamp>> GetAll(Expression<Func<ILamp, bool>>? filter = null);

        Task<ILamp> GetById(long id);
    }
}

