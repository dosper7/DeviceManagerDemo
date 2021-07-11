using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Adapter.InMemoryDB
{
    public class InMemoryDevicesDatabase : IDeviceStore
    {
        private static ConcurrentDictionary<Guid, DeviceModel> database = new ConcurrentDictionary<Guid, DeviceModel>();
        private static Random random = new Random();
        private readonly ILogger<InMemoryDevicesDatabase> _logger;

        public InMemoryDevicesDatabase(ILogger<InMemoryDevicesDatabase> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Seed();
        }

        private static void Seed()
        {
            var brands = new[] { "Iphone", "Samsung", "Nokia" };
            var namesSeed = new[] { "ABC" };

            static string RandomString(string prefix)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return string.Concat($"{prefix}_", new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray()));
            }

            for (int i = 0; i < 100; i++)
            {
                var id = Guid.NewGuid();
                var brand = brands[random.Next(0, brands.Length)];
                database.AddOrUpdate(id, new DeviceModel()
                {
                    Brand = brand,
                    Name = RandomString(brand),
                    CreationTime = DateTime.Now.AddDays((i + 1) * -1),
                    Id = Guid.NewGuid()

                }, (id, item) => item);
            }
        }


        public Task<DeviceModel> AddDeviceAsync(DeviceModel device)
        {
            if (device is null)
                throw new ArgumentNullException(nameof(device));

            device.Id = Guid.NewGuid();
            database.AddOrUpdate(device.Id, device, (id, _device) => device);
            _logger.LogDebug($"Added device with id {device.Id}.");
            return Task.FromResult(device);
        }

        public Task<DeviceModel> DeleteDeviceAsync(Guid deviceId)
        {
            bool isRemoved = database.TryRemove(deviceId, out DeviceModel device);
            _logger.LogDebug($"Removed device with id {deviceId} was {isRemoved}.");
            return Task.FromResult(device);
        }

        public Task<PagedResult<DeviceModel>> GetAllDevicesAsync(int startIndex = 0, int pageSize = 10)
        {
            var results = new PagedResult<DeviceModel>();
            results.TotalCount = database.Count;
            results.Items = database.Values.Skip(startIndex * pageSize).Take(pageSize).OrderByDescending(c => c.CreationTime);
            _logger.LogDebug($"Get All devices with success.");
            return Task.FromResult(results);
        }

        public Task<DeviceModel> GetDeviceByIdAsync(Guid deviceId)
        {
            _logger.LogDebug($"Get Device with id {deviceId}.");
            return database.TryGetValue(deviceId, out DeviceModel device) ? Task.FromResult(device) : Task.FromResult(default(DeviceModel));
        }

        public Task<PagedResult<DeviceModel>> SearchDeviceAsync(DeviceModel deviceModel, int startIndex = 0, int pageSize = 10)
        {
            if (deviceModel is null)
                throw new ArgumentNullException(nameof(deviceModel));

            var sb = new StringBuilder("Searching devices with:");
            if (deviceModel.Brand != default) sb.AppendLine($" Brand = {deviceModel.Brand}");
            if (deviceModel.CreationTime != default) sb.AppendLine($" CreationTime = {deviceModel.CreationTime}");
            if (deviceModel.Name != default) sb.AppendLine($" Name = {deviceModel.Name}");
            _logger.LogDebug(sb.AppendLine().ToString());

            //searching
            var items = from dbItem in database.Values
                        where
                            (deviceModel.Id == Guid.Empty || deviceModel.Id == dbItem.Id) &&
                            (string.IsNullOrWhiteSpace(deviceModel.Brand) || dbItem.Brand.Contains(deviceModel.Brand)) &&
                            (string.IsNullOrWhiteSpace(deviceModel.Name) || dbItem.Name.Contains(deviceModel.Name)) &&
                            (deviceModel.CreationTime == DateTimeOffset.MinValue || deviceModel.CreationTime == dbItem.CreationTime)
                        orderby dbItem.CreationTime descending
                        select dbItem;


            var results = new PagedResult<DeviceModel>()
            {
                TotalCount = items.Count(),
                //paging
                Items = items.Skip(startIndex * pageSize).Take(pageSize)
            };

            return Task.FromResult(results);

        }

        public Task<DeviceModel> UpateDeviceAsync(DeviceModel device, bool partialUpdate = false)
        {
            if (device is null)
                throw new ArgumentNullException(nameof(device));

            _logger.LogDebug($"updating device with id {device.Id} in mode partialUpdate = {partialUpdate}.");
            if (database.TryGetValue(device.Id, out DeviceModel dbItem))
            {
                if (partialUpdate)
                {
                    dbItem.Brand = device.Brand == default ? dbItem.Brand : device.Brand;
                    dbItem.CreationTime = device.CreationTime == default ? dbItem.CreationTime : device.CreationTime;
                    dbItem.Name = device.Name == default ? dbItem.Name : device.Name;
                }
                else
                {
                    dbItem.Brand = device.Brand;
                    dbItem.CreationTime = device.CreationTime;
                    dbItem.Name = dbItem.Name;
                }

                database[device.Id] = dbItem;
            }
            return Task.FromResult(default(DeviceModel));
        }
    }
}
