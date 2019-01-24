using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {

        private ILogger<PointsOfInterestController> _logger; // dependency injection
        private IMailService _mailService; // dependency injection
        private ICityInfoRepository _cityInfoRepository;// inject repository

        // constructor injection
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }


        [HttpGet("{cityId}/pointsOfInterest")] // fetch by id
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                // throw new Exception("Exception Sample");

                // return error if cityId (parent) doesn't exist
                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);                
            
                //if (city == null)
                if(!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with cityID {cityId} is not found");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                var pointsOfInterestForCityResult = new List<PointOfInterestDto>();
                foreach (var item in pointsOfInterestForCity)
                {
                    pointsOfInterestForCityResult.Add(new PointOfInterestDto() {
                        Id = item.Id,
                        Name= item.Name,
                        Description=item.Description
                    });
                }

                return Ok(pointsOfInterestForCityResult);
                //return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{cityId}/pointsOfInterest/{id}", Name = "GetPointOfInterest")] // fetch 
        public IActionResult GetPointsOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if(pointOfInterest == null)
            {
                return NotFound();
            }

            var pointOfInterestResult = new PointOfInterestDto() {
                Id = pointOfInterest.Id,
                Name= pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            return Ok(pointOfInterest);
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            //var pointsOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            //if (pointsOfInterest == null)
            //{
            //    return NotFound();
            //}

            //return Ok(pointsOfInterest);
        }


        [HttpPost("{cityId}/pointsofinterest")] // create new one
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest(); // 400 error, url doesnt exist
            }

            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "Name and Desc cannot be the same");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound(); // 404 error
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities
                .SelectMany(c => c.PointsOfInterest).Max(p => p.Id); // use selectMany if you wanna return a list

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            // 201 successfully created
            return CreatedAtRoute("GetPointOfInterest",
                new { cityId = cityId, id = finalPointOfInterest.Id }, finalPointOfInterest);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")] // full update
        public IActionResult UpdatePointOfInterest(int cityId, int id,
            [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest(); // 400 error, url doesnt exist
            }

            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "Name and Desc cannot be the same");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound(); // 404 error
            }

            var oldPointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (oldPointOfInterest == null)
            {
                return NotFound();
            }

            oldPointOfInterest.Name = pointOfInterest.Name;
            oldPointOfInterest.Description = pointOfInterest.Description;

            return NoContent(); // save successfully but dont return te updated value
        }


        [HttpPatch("{cityId}/pointsofinterest/{id}")] // partial update
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
           [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(); // 400 error, url doesnt exist
            }


            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound(); // 404 error
            }

            var oldPointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (oldPointOfInterest == null)
            {
                return NotFound();
            }

            var pointsOfInterestFromPatch = new PointOfInterestForUpdateDto
            {
                Name = oldPointOfInterest.Name,
                Description = oldPointOfInterest.Description
            };

            // at this point, applyto operation maps new values in patchDoc to pointsOfInterestFromPatch (replace)
            // postman body:
            // [
            //      {
            //          "op": "replace", // update operation
            //          "path": "/name",
            //          "value": "Updated - Central Park"
            //      }]
            patchDoc.ApplyTo(pointsOfInterestFromPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(pointsOfInterestFromPatch.Name == pointsOfInterestFromPatch.Description)
            {
                ModelState.AddModelError("Description", "Name and Desc cannot be the same");
            }

            // örneğin  "op": "remove","path": "/name" dediğimiz zaman ve TryValidateModel'i kullanmazsak
            // required olmasına rağmen name'i validate etmeden siler. Bunu yapmasın diye TryValidateModel ekleriz
            TryValidateModel(pointsOfInterestFromPatch);

            oldPointOfInterest.Name = pointsOfInterestFromPatch.Name;
            oldPointOfInterest.Description = pointsOfInterestFromPatch.Description;

            return NoContent();
        }


        [HttpDelete("{cityId}/pointsofinterest/{id}")] // delete
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound(); // 404 error
            }


            var oldPointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (oldPointOfInterest == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(oldPointOfInterest);

            // send mail when deleted
            _mailService.Send("Poin of Interest is deleted"
                , $"Point of Interest {oldPointOfInterest.Name} with id {oldPointOfInterest.Id} was deleted.");

            return NoContent(); // return 204 if successful
        }
    }
}
