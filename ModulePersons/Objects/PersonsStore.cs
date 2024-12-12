using System.Linq.Expressions;
using Model;
using ModulePersons.Interfaces;

namespace ModulePersons.Objects
{
    internal class PersonsStore : IPersonsStore
    {
        private readonly IList<IPerson> _persons = new List<IPerson>();
        public Task<IPerson> Add(IPerson person)
        {
            return Task.Run(() =>
            {
                if (_persons.Any(x => x.Id == person.Id))
                    throw new ArgumentException($"Сотрудник с Id {person.Id} уже существует");

                person.Id = _persons.Count + 1;
                _persons.Add(person);
                return person;
            });
        }

        public Task<IPerson> Delete(IPerson person)
        {
            return Task.Run(() =>
            {
                var personLoc = _persons.FirstOrDefault(x => x.Id == person.Id);
                if (personLoc == null)
                    throw new ArgumentException($"Сотрудник с Id {person.Id} не существует");

                _persons.Remove(personLoc);
                return personLoc;
            });
        }

        public Task<IPerson> Update(IPerson person)
        {
            return Task.Run(() =>
            {
                var personLoc = _persons.FirstOrDefault(x => x.Id == person.Id);
                if (personLoc == null)
                    throw new ArgumentException($"Сотрудник с Id {person.Id} не существует");

                _persons.Remove(personLoc);
                _persons.Add(person);

                return person;
            });
        }


        public Task<IEnumerable<IPerson>> GetAll(Expression<Func<IPerson, bool>>? filter)
        {
            return Task.Run(() =>
            {
                var persons = _persons.AsQueryable();
                if (filter != null)
                    persons = persons.Where(filter);

                return persons.AsEnumerable();
            });
        }

        public Task<IPerson> GetById(long id)
        {
            return Task.Run(() =>
            {
                var person = _persons.FirstOrDefault(x => x.Id == id);
                if (person == null)
                    throw new ArgumentException($"Сотрудник с Id {id} не существует");

                return person;
            });
        }
    }
}
