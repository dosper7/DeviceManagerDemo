using System;

namespace DeviceManager.Business.Models
{
    public class DeviceModel
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid Id { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}, Brand:{Brand}, CreationTime:{CreationTime}, Id:{Id}";
        }

    }
}
