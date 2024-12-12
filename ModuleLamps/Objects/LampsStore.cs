using System.Linq.Expressions;
using Model;
using ModuleLamps.Interfaces;

namespace ModuleLamps.Objects
{
    internal class LampsStore : ILampsStore
    {
        private readonly IList<ILamp> _lamps = new List<ILamp>();

        public Task<ILamp> Add(ILamp lamp)
        {
            return Task.Run(() =>
            {
                if (_lamps.Any(x => x.Id == lamp.Id))
                    throw new ArgumentException($"Светильник с Id {lamp.Id} уже существует");

                lamp.Id = _lamps.Count + 1;
                _lamps.Add(lamp);
                return lamp;
            });
        }

        public Task<ILamp> Delete(ILamp lamp)
        {
            return Task.Run(() =>
            {
                var lampLoc = _lamps.FirstOrDefault(x => x.Id == lamp.Id);
                if (lampLoc == null)
                    throw new ArgumentException($"Светильник с Id {lamp.Id} не существует");

                _lamps.Remove(lampLoc);
                return lampLoc;
            });
        }

        public Task<IEnumerable<ILamp>> GetAll(Expression<Func<ILamp, bool>>? filter)
        {
            return Task.Run(() =>
            {
                var lamps = _lamps.AsQueryable();
                if (filter != null)
                    lamps = lamps.Where(filter);

                return lamps.AsEnumerable();
            });
        }

        public Task<ILamp> GetById(long id)
        {
            return Task.Run(() =>
            {
                var lamp = _lamps.FirstOrDefault(x => x.Id == id);
                if (lamp == null)
                    throw new ArgumentException($"Светильник с Id {id} не существует");

                return lamp;
            });
        }

        public Task<ILamp> Update(ILamp lamp)
        {
            return Task.Run(() =>
            {
                var lampLoc = _lamps.FirstOrDefault(x => x.Id == lamp.Id);
                if (lampLoc == null)
                    throw new ArgumentException($"Светильник с Id {lamp.Id} не существует");

                _lamps.Remove(lampLoc);
                _lamps.Add(lamp);
                return lamp;
            });
        }
    }
}

