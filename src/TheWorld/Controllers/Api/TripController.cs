using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Server.Kestrel.Http;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips")]
    public class TripController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<TripController> _logger;

        public TripController(IWorldRepository repository, ILogger<TripController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public JsonResult Get()
        {
            var trips = _repository.GetUserTripsWithStops(User.Identity.Name);
            var results = Mapper.Map<IEnumerable<TripViewModel>>(trips);
            
            return Json(results);
        }

        [HttpPost("")]
        public JsonResult Post([FromBody]TripViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newTrip = Mapper.Map<Trip>(vm);

                    newTrip.UserName = User.Identity.Name;

                    //DB insert
                    _logger.LogInformation("Attempting to save a new trip");

                    _repository.AddTrip(newTrip);

                    if (_repository.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;
                        return Json(Mapper.Map<TripViewModel>(newTrip));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Falied to save new trip", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new {MessageBody = ex.Message});
            }


            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Failed", ModelState = ModelState} );
        }

        [HttpDelete("/api/trips/{tripName}")]
        public JsonResult Delete(string tripName)
        {
            try
            {
                var results = _repository.GetTripByName(tripName, User.Identity.Name);

                if (results == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json("Failed to delete trip");
                }

                _repository.DeleteTrip(results);

                if (_repository.SaveAll())
                {
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json("Deleted Trip");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete trip", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed to delete trip");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json("Failed to delete trip");
        }

        [HttpPut("/api/trips/{tripName}")]
        public JsonResult Put(string tripName, [FromBody]string newTripName)
        {
            try
            {
                return Json("true");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete trip", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed to update trip");
            }
        }
    }
}
