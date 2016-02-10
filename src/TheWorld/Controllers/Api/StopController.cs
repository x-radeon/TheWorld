using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using System.Net;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using TheWorld.Services;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips/{tripName}/stops")]
    public class StopController : Controller
    {
        private ILogger<StopController> _logger;
        private IWorldRepository _repository;
        private CoordService _coordService;

        public StopController(IWorldRepository repository, ILogger<StopController> logger, CoordService coordService)
        {
            _repository = repository;
            _logger = logger;
            _coordService = coordService;
        }

        [HttpGet("")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var results = _repository.GetTripByName(tripName, User.Identity.Name);

                if (results == null)
                {
                    return Json(null);
                }

                return Json(Mapper.Map<IEnumerable<StopViewModel>>(results.Stops.OrderBy(s => s.Order)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get stop for trip {tripName}", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { MessageBody = ex.Message });
            }
        }

        public async Task<JsonResult> Post(string tripName, [FromBody] StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Map to the Enity
                    var newStop = Mapper.Map<Stop>(vm);
                    
                    //Looking up Geocoordinates
                    var coordResults = await _coordService.Lookup(newStop.Name);

                    if (!coordResults.Success)
                    {
                        Response.StatusCode = (int) HttpStatusCode.BadRequest;
                        Json(coordResults.Message);
                    }

                    newStop.Latitude = coordResults.Latitude;
                    newStop.Longitude = coordResults.Longitude;

                    //save to db
                    _repository.AddStop(tripName, User.Identity.Name, newStop);

                    if (_repository.SaveAll())
                    {
                        Response.StatusCode = (int) HttpStatusCode.Created;
                        return Json(Mapper.Map<StopViewModel>(newStop));
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save new stop", ex);
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json("Failed to save new stop");
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json("Validation failed; failed to save new stop");
        }
    }
}
