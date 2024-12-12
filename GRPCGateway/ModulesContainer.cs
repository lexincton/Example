using System.Reflection;
using Common;

namespace GRPCGateway
{
    internal class ModulesContainer
    {
        private readonly List<IModule> _modules = new();

        private readonly string _path;
        public ModulesContainer(string path)
        {
            _path = path;
        }

        public void LoadModules()
        {
            if (!Directory.Exists(_path))
                return;

            var dllFiles = Directory.GetFiles(_path, "Module*.dll");
            foreach (var dll in dllFiles)
            {
                var assembly = Assembly.LoadFrom(dll);
                var moduleTypes = assembly.GetTypes()
                    .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var moduleType in moduleTypes)
                {
                    var moduleInstance = (IModule)Activator.CreateInstance(moduleType);
                    _modules.Add(moduleInstance);
                }
            }
        }

        public void RegistryModules(IServiceCollection services)
        {
            foreach (var item in _modules)
            {
                item.RegisterServices(services);
            }
        }
    }
}
