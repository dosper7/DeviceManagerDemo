using DeviceManager.Adapter.InMemoryDB;
using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using DeviceManager.Business.Ports;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeviceManager.UnitTests
{
    public class InMemoryDBUnitTests
    {
        private readonly IDeviceStore _database;
        private readonly Mock<ILogger<InMemoryDevicesDatabase>> _logger;

        public InMemoryDBUnitTests()
        {
            _logger = new Mock<ILogger<InMemoryDevicesDatabase>>();
            _database = new InMemoryDevicesDatabase(_logger.Object);
        }

        [Fact]
        public async Task Add_Device_should_return_Device_with_id()
        {
            var device = new DeviceModel()
            {
                Brand = "1",
                Name = "1",
                CreationTime = DateTime.Today
            };

            var getAllBefore = await _database.GetAllDevicesAsync().ConfigureAwait(false);
            var beforeCount = getAllBefore.TotalCount;

            var addedDevice = await _database.AddDeviceAsync(device).ConfigureAwait(false);

            var getAllAfter = await _database.GetAllDevicesAsync().ConfigureAwait(false);
            var afterCount = getAllAfter.TotalCount;

            addedDevice.Should().NotBeNull();
            addedDevice.Id.Should().NotBeEmpty();
            afterCount.Should().BeGreaterThan(beforeCount);
        }


        [Fact]
        public async Task Delete_Device_should_delete_and_return_deleted_device()
        {
            var currentData = await _database.GetAllDevicesAsync().ConfigureAwait(false);
            var deviceToDelete = currentData.Items.First();

            var getAllBefore = await _database.GetAllDevicesAsync().ConfigureAwait(false);
            var beforeCount = getAllBefore.TotalCount;
            var deletedDevice = await _database.DeleteDeviceAsync(deviceToDelete.Id).ConfigureAwait(false);
            var getAllAfter = await _database.GetAllDevicesAsync().ConfigureAwait(false);
            var afterCount = getAllAfter.TotalCount;

            deletedDevice.Should().NotBeNull();
            deletedDevice.Id.Should().NotBeEmpty().And.Be(deviceToDelete.Id);
            afterCount.Should().BeLessThan(beforeCount);
        }

        [Fact]
        public async Task GetAllDevices_should_be_of_type_paged_results_of_devices()
        {
            var devices = await _database.GetAllDevicesAsync().ConfigureAwait(false);

            devices.Should().BeOfType<PagedResult<DeviceModel>>();
        }

        [Fact]
        public async Task GetAllDevices_should_return_results_with_given_page_size()
        {
            int pageSize = 5;
            var results = await _database.GetAllDevicesAsync(0, pageSize).ConfigureAwait(false);

            results.Should().NotBeNull();
            results.Items.Should().HaveCount(pageSize);
        }

        [Fact]
        public async Task GetAllDevices_should_return_next_page_results()
        {
            var initialResults = await _database.GetAllDevicesAsync(0, 10).ConfigureAwait(false);
            var page1StartItem = initialResults.Items.ToArray()[1];
            var page2StartItem = initialResults.Items.ToArray()[2];

            var page1 = await _database.GetAllDevicesAsync(1, 1).ConfigureAwait(false);
            var page2 = await _database.GetAllDevicesAsync(2, 1).ConfigureAwait(false);

            var item1Page1 = page1.Items.First();
            var item1Page2 = page2.Items.First();

            item1Page1.Should().Be(page1StartItem);
            item1Page2.Should().Be(page2StartItem);

        }

        [Fact]
        public async Task GetDeviceById_should_return_Device_with_given_id()
        {
            var initialResults = await _database.GetAllDevicesAsync(0, 10).ConfigureAwait(false);
            var device = initialResults.Items.First();

            var dbDevice = await _database.GetDeviceByIdAsync(device.Id).ConfigureAwait(false);

            dbDevice.Should().NotBeNull();
            dbDevice.Id.Should().Be(device.Id);
            dbDevice.Name.Should().Be(device.Name);
            dbDevice.Brand.Should().Be(device.Brand);
            dbDevice.CreationTime.Should().Be(device.CreationTime);
        }

        [Fact]
        public async Task SearchDevice_should_return_PagedResult()
        {
            var dbResults = await _database.SearchDeviceAsync(new DeviceModel()).ConfigureAwait(false);

            dbResults.Should().NotBeNull();
            dbResults.Should().BeOfType<PagedResult<DeviceModel>>();
        }

        [Fact]
        public async Task SearchDevice_should_return_Device_when_searched_by_Brand()
        {
            var initialResults = await _database.GetAllDevicesAsync(0, 10).ConfigureAwait(false);
            var device = initialResults.Items.First();

            var searchCriteria = new DeviceModel() { Brand = device.Brand };
            var dbResults = await _database.SearchDeviceAsync(searchCriteria).ConfigureAwait(false);

            dbResults.Should().NotBeNull();
            dbResults.Items.Should().NotBeEmpty();
            dbResults.Items.First().Brand.Should().Be(device.Brand);
        }

        [Fact]
        public async Task SearchDevice_should_return_Device_when_searched_by_Name()
        {
            var initialResults = await _database.GetAllDevicesAsync(0, 10).ConfigureAwait(false);
            var device = initialResults.Items.First();

            var searchCriteria = new DeviceModel() { Name = device.Name };
            var dbResults = await _database.SearchDeviceAsync(searchCriteria).ConfigureAwait(false);

            dbResults.Should().NotBeNull();
            dbResults.Items.Should().NotBeEmpty();
            dbResults.Items.First().Name.Should().Be(device.Name);
        }

        [Fact]
        public async Task SearchDevice_should_return_Device_when_searched_by_CreationTime()
        {
            var initialResults = await _database.GetAllDevicesAsync(0, 10).ConfigureAwait(false);
            var device = initialResults.Items.First();

            var searchCriteria = new DeviceModel() { CreationTime = device.CreationTime };
            var dbResults = await _database.SearchDeviceAsync(searchCriteria).ConfigureAwait(false);

            dbResults.Should().NotBeNull();
            dbResults.Items.Should().NotBeEmpty();
            dbResults.Items.First().CreationTime.Should().Be(device.CreationTime);
        }

        [Fact]
        public async Task SearchDevice_should_return_Device_when_searched_by_Id()
        {
            var initialResults = await _database.GetAllDevicesAsync(0, 10).ConfigureAwait(false);
            var device = initialResults.Items.First();

            var searchCriteria = new DeviceModel() { Id = device.Id };
            var dbResults = await _database.SearchDeviceAsync(searchCriteria).ConfigureAwait(false);

            dbResults.Should().NotBeNull();
            dbResults.Items.Should().NotBeEmpty();
            dbResults.Items.First().Id.Should().Be(device.Id);
        }

        [Fact]
        public async Task UpdateDevice_should_return_updated_device()
        {
            var initialResults = await _database.GetAllDevicesAsync(0, 10).ConfigureAwait(false);
            var device = initialResults.Items.First();

            var dbDevice = await _database.UpateDeviceAsync(device).ConfigureAwait(false);

            dbDevice.Should().NotBeNull();
            dbDevice.Id.Should().Be(dbDevice.Id);
        }

        [Fact]
        public async Task UpdateDevice_should_should_update_device_fields()
        {
            var initialResults = await _database.GetAllDevicesAsync(0, 10).ConfigureAwait(false);
            var device = initialResults.Items.First();

            var toUpdate = new DeviceModel()
            {
                Id = device.Id,
                Name = "updated",
                Brand = "updated",
                CreationTime = DateTime.Now
            };

            await _database.UpateDeviceAsync(toUpdate).ConfigureAwait(false);
            var dbDevice = await _database.GetDeviceByIdAsync(device.Id).ConfigureAwait(false);

            dbDevice.Should().NotBeNull();
            dbDevice.Id.Should().Be(device.Id);
            dbDevice.Name.Should().Be(device.Name);
            dbDevice.Brand.Should().Be(device.Brand);
            dbDevice.CreationTime.Should().Be(device.CreationTime);
        }


    }
}
