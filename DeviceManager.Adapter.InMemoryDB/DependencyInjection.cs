using DeviceManager.Business.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManager.Adapter.InMemoryDB
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            return services.AddSingleton<IDeviceStore, InMemoryDevicesDatabase>();
        }
    }
}
