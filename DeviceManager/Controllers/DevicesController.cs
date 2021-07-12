using DeviceManager.Business.Core.Common;
using DeviceManager.Business.Models;
using DeviceManager.Business.UseCases.Device.AddDevice;
using DeviceManager.Business.UseCases.Device.DeleteDevice;
using DeviceManager.Business.UseCases.Device.GetAllDevices;
using DeviceManager.Business.UseCases.Device.GetDeviceById;
using DeviceManager.Business.UseCases.Device.SearchDevice;
using DeviceManager.Business.UseCases.Device.UpdateDevice;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DevicesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Add Device
        /// </summary>
        /// <param name="deviceCommand">Device data to create.</param>
        /// <returns>Created Device</returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(ApiResult<DeviceModel>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(IList<string>))]
        public async Task<ActionResult> AddDevice(AddDeviceCommand deviceCommand)
        {
            var response = await _mediator.Send(deviceCommand).ConfigureAwait(false);

            return response.Success ?
                CreatedAtAction(nameof(GetById), new { Id = response.Data?.Id }, response.Data) :
                BadRequest(response.Errors);
        }

        /// <summary>
        /// Get Device by Id.
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>Device</returns>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(DeviceModel))]
        [SwaggerResponse(StatusCodes.Status404NotFound, type: typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(IList<string>))]
        public async Task<ActionResult> GetById(Guid id)
        {
            var response = await _mediator.Send(new GetDeviceByIdQuery() { Id = id }).ConfigureAwait(false);

            if (!response.Success)
                return BadRequest(response.Data);

            return (response.Data?.Id).GetValueOrDefault() == Guid.Empty ?
                                NotFound($"Device with id {id} not found.") :
                                Ok(response.Data);
        }

        /// <summary>
        /// Delete Device.
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>Deleted Device</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(DeviceModel))]
        [SwaggerResponse(StatusCodes.Status404NotFound, type: typeof(IList<string>))]
        public async Task<ActionResult> Delete(Guid id)
        {
            var response = await _mediator.Send(new DeleteDeviceCommand() { Id = id }).ConfigureAwait(false);

            return (response.Data?.Id).GetValueOrDefault() != Guid.Empty ?
                Ok(response.Data) :
                NotFound(response.Errors);
        }


        /// <summary>
        /// Get all devices with paging ordered by most recent Creation Time.
        /// </summary>
        /// <param name="query">paging filter</param>
        /// <returns>Devices with paging</returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(PagedResult<DeviceModel>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(IList<string>))]
        public async Task<ActionResult> GetAll([FromQuery] GetAllDevicesQuery query)
        {
            var response = await _mediator.Send(query).ConfigureAwait(false);

            return response.Success ? Ok(response.Data) : BadRequest(response.Errors);

        }

        /// <summary>
        /// Search devices.
        /// </summary>
        /// <param name="query">paging filter</param>
        /// <returns>Filtered Devices with paging</returns>
        [HttpGet]
        [Route("Search")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(PagedResult<DeviceModel>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(IList<string>))]
        public async Task<ActionResult> Search([FromQuery] SearchDeviceQuery search)
        {
            var response = await _mediator.Send(search).ConfigureAwait(false);

            return response.Success ? Ok(response.Data) : BadRequest(response.Errors);
        }


        /// <summary>
        /// Partial Update Device.
        /// </summary>
        /// <param name="device">Device to update.</param>
        /// <returns>Updated device.</returns>
        [HttpPatch]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(DeviceModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(IList<string>))]
        public async Task<ActionResult> PartialUpdate(DeviceModel device)
        {
            var command = new UpdateDeviceCommand()
            {
                Id = device.Id,
                Name = device.Name,
                Brand = device.Brand,
                CreationTime = device.CreationTime,
                IsPartialUpdate = true,
            };

            var response = await _mediator.Send(command).ConfigureAwait(false);

            return response.Success ? Ok(response.Data) : BadRequest(response.Errors);
        }


        /// <summary>
        /// Full Update Device.
        /// </summary>
        /// <param name="device">Device to update.</param>
        /// <returns>Updated device.</returns>
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(DeviceModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, type: typeof(IList<string>))]
        public async Task<ActionResult> FullUpdate(DeviceModel device)
        {
            var command = new UpdateDeviceCommand()
            {
                Id = device.Id,
                Name = device.Name,
                Brand = device.Brand,
                CreationTime = device.CreationTime,
                IsPartialUpdate = false,
            };

            var response = await _mediator.Send(command).ConfigureAwait(false);

            return response.Success ? Ok(response.Data) : BadRequest(response.Errors);
        }


    }
}
