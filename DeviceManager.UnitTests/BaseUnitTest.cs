using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace DeviceManager.UnitTests
{
    public class BaseDeviceTest<T>
    {
        protected Mock<IDeviceStore> Database { get; set; }
        protected Mock<ILogger<T>> Logger { get; set; }
        protected Mock<IMediator> Mediator { get; set; }
        protected DeviceBuilder MockDeviceBuilder { get; set; }

        public BaseDeviceTest()
        {
            Database = new Mock<IDeviceStore>();
            Logger = new Mock<ILogger<T>>();
            Mediator = new Mock<IMediator>();
            MockDeviceBuilder = new DeviceBuilder();
        }

        public class DeviceBuilder
        {
            private DeviceModel device = new DeviceModel()
            {
                Name = "MockName",
                Brand = "samsung",
                CreationTime = DateTime.Today,
                Id = Guid.NewGuid()
            };

            public DeviceBuilder WithName(string name)
            {
                device.Name = name;
                return this;
            }

            public DeviceBuilder WithBrand(string brand)
            {
                device.Brand = brand;
                return this;
            }

            public DeviceBuilder WithCreationTime(DateTime creationTime)
            {
                device.CreationTime = creationTime;
                return this;
            }

            public DeviceBuilder WithId(Guid id)
            {
                device.Id = id;
                return this;
            }

            public DeviceModel Build(bool newInstance = false)
            {
                return newInstance ? new DeviceModel()
                {
                    Name = device.Name,
                    Brand = device.Brand,
                    CreationTime = device.CreationTime,
                    Id = device.Id
                } : device;
            }
        }

    }

}
