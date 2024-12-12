using System.Linq.Expressions;
using Model;

namespace ModuleLamps.Interfaces
{
    /// <summary>
    /// Имитация БД
    /// </summary>
    internal interface ILampsStore
    {
        Task<ILamp> Add(ILamp person);

        Task<ILamp> Update(ILamp person);

        Task<ILamp> Delete(ILamp person);

        Task<IEnumerable<ILamp>> GetAll(Expression<Func<ILamp, bool>>? filter = null);

        Task<ILamp> GetById(long id);
    }
}

