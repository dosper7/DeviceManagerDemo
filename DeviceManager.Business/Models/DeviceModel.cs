using System;

namespace DeviceManager.Business.Models
{
    public class DeviceModel
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public Guid Id { get; set; }
    }
}
