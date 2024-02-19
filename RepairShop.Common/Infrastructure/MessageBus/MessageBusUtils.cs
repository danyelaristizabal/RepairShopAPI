using Microsoft.Extensions.Configuration;
using RepairShop.Common.Infrastructure.Settings;
using System.Reflection;

namespace RepairShop.Common.Infrastructure.MessageBus
{
    public static class MessageBusUtils
    {
        private static bool CanRegisterKey(this Assembly assembly, string key, ConfigurationManager config)
        {
            var messageBusSettings = config.GetSection(nameof(MessagebusSettings)).Get<MessagebusSettings[]>();
            var orderedSettings = messageBusSettings.OrderBy(x => x.AssemblyName).ToList();
            var currentAssemblyName = assembly.GetName().Name;

            var settingsWithKey = orderedSettings.Where(x => x.Consumers.ContainsKey(key) || x.Publishers.ContainsKey(key)).ToList();

            if (settingsWithKey.Count == 1)
            {
                return true;
            }
            else if (settingsWithKey.Count > 1)
            {
                var orderedAssemblyNames = settingsWithKey.Select(x => x.AssemblyName).OrderBy(x => x).ToList();
                if (orderedAssemblyNames.First() == currentAssemblyName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
