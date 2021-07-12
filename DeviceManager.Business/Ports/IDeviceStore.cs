using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using System;
using System.Threading.Tasks;

namespace DeviceManager.Business.Ports
{
    public interface IDeviceStore
    {
        Task<DeviceModel> AddDeviceAsync(DeviceModel device);
        Task<DeviceModel> GetDeviceByIdAsync(Guid deviceId);
        Task<PagedResult<DeviceModel>> GetAllDevicesAsync(int startIndex = 0, int pageSize = 10);
        Task<DeviceModel> UpateDeviceAsync(DeviceModel device);
        Task<DeviceModel> DeleteDeviceAsync(Guid deviceId);
        Task<PagedResult<DeviceModel>> SearchDeviceAsync(DeviceModel deviceModel, int startIndex = 0, int pageSize = 10);
    }
}
