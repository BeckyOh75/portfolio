using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests.ScheduleAvailability;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/scheduleavailabilities")]
    [ApiController]
    public class ScheduleAvailabilityApiController : BaseApiController
    {
        private IScheduleAvailabilityService _service = null;
        private IAuthenticationService<int> _authService = null;
        public ScheduleAvailabilityApiController(IScheduleAvailabilityService service
            , ILogger<ScheduleAvailabilityApiController> logger
            , IAuthenticationService<int> authService) : base(logger)
            
        {
            _service = service;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(ScheduleAvailabilityAddRequest model){
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };

                result = Created201(response);
            }
            catch (SqlException error)
            {
                BaseResponse response = new ErrorResponse(error.Message);

                result = StatusCode(501, response);

            }
            catch (Exception error)
            {
                Logger.LogError(error.ToString());
                ErrorResponse response = new ErrorResponse(error.Message);

                result = StatusCode(500, response);
            }
            return result;
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(ScheduleAvailabilityUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception error)
            {
                code = 500;
                response = new ErrorResponse(error.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<ScheduleAvailability>> Get(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                ScheduleAvailability schedule = _service.Get(id);

                if (schedule == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource not found.");
                }
                else
                {
                    response = new ItemResponse<ScheduleAvailability> { Item = schedule };
                }
            }
            catch (Exception error)
            {
                code = 500;
                base.Logger.LogError(error.ToString());
                response = new ErrorResponse(error.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<ScheduleAvailability>>> Get(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<ScheduleAvailability> page = _service.GetAll(pageIndex, pageSize);
                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource Not Found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ScheduleAvailability>> { Item = page };
                }
            }
            catch (Exception error)
            {
                code = 500;
                response = new ErrorResponse(error.Message);
                base.Logger.LogError(error.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("createdBy")]
        public ActionResult<ItemResponse<Paged<ScheduleAvailability>>> GetCreatedBy(int pageIndex, int pageSize, int createdBy)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<ScheduleAvailability> page = _service.GetByCreatedBy(pageIndex, pageSize, createdBy);
                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource Not Found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ScheduleAvailability>> { Item = page };
                }
            }
            catch (Exception error)
            {
                code = 500;
                response = new ErrorResponse(error.Message);
                base.Logger.LogError(error.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet]
        public ActionResult<ItemsResponse<ScheduleAvailability>> Get()
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<ScheduleAvailability> list = _service.Get();

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource Not Found.");
                }
                else
                {
                    response = new ItemsResponse<ScheduleAvailability> { Items = list };
                }
            }
            catch (Exception error)
            {
                code = 500;
                response = new ErrorResponse(error.Message);
                base.Logger.LogError(error.ToString());
            }
            return StatusCode(code, response);
        }
    }
}
