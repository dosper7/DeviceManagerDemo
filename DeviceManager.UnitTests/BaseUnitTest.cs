using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace DeviceManager.UnitTests
{
    public class BaseDeviceTest<T>
    {
        protected Mock<IDeviceStore> Database { get; set; }
        protected Mock<ILogger<T>> Logger { get; set; }
        protected Mock<IMediator> Mediator { get; set; }

        public BaseDeviceTest()
        {
            Database = new Mock<IDeviceStore>();
            Logger = new Mock<ILogger<T>>();
            Mediator = new Mock<IMediator>();
        }

        internal static DeviceModel GetDeviceMock(string name = "MockName",
                                                string brand = "Iphone",
                                                DateTime? creationTime = null,
                                                Guid? id = null) => new DeviceModel()
                                                {
                                                    Name = name,
                                                    Brand = brand,
                                                    CreationTime = creationTime ?? DateTime.Now,
                                                    Id = id ?? Guid.NewGuid()
                                                };

    }
}
