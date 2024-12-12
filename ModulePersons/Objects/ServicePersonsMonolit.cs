using System.Linq.Expressions;
using Model;
using ModulePersons.API;
using ModulePersons.Interfaces;

namespace ModulePersons.Objects
{
    internal class ServicePersonsMonolit : IServicePersonsMonolit
    {
        private readonly IPersonsStore _personsStore;

        public ServicePersonsMonolit(IPersonsStore personsStore)
        {
            _personsStore = personsStore;
        }

        public async Task<IPerson> Add(IPerson person)
        {
            return await _personsStore.Add(person);
        }

        public async Task<IPerson> Delete(IPerson person)
        {
            return await _personsStore.Delete(person);
        }

        public async Task<IEnumerable<IPerson>> GetAll(Expression<Func<IPerson, bool>>? filter = null)
        {
            return await _personsStore.GetAll(filter);
        }

        public async Task<IPerson> GetById(long id)
        {
            return await _personsStore.GetById(id);
        }

        public Task<IPerson> Update(IPerson person)
        {
            return _personsStore.Update(person);
        }
    }
}
