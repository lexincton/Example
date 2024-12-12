using System.Linq.Expressions;
using Model;
using ModuleLamps.Interfaces;

namespace ModuleLamps.Objects
{
    internal class ServiceLampsMonolit : IServiceLampsMonolit
    {
        private readonly ILampsStore _lampsStore;

        public ServiceLampsMonolit(ILampsStore lampsStore)
        {
            _lampsStore = lampsStore;
        }

        public async Task<ILamp> Add(ILamp lamp)
        {
            return await _lampsStore.Add(lamp);
        }

        public async Task<ILamp> Delete(ILamp lamp)
        {
            return await _lampsStore.Delete(lamp);
        }

        public async Task<IEnumerable<ILamp>> GetAll(Expression<Func<ILamp, bool>>? filter = null)
        {
            return await _lampsStore.GetAll(filter);
        }

        public async Task<ILamp> GetById(long id)
        {
            return await _lampsStore.GetById(id);
        }

        public Task<ILamp> Update(ILamp lamp)
        {
            return _lampsStore.Update(lamp);
        }
    }
}
