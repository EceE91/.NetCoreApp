using AutoMapper;
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
                // return error if cityId (parent) doesn't exist
                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);                
            
                //if (city == null)
                if(!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with cityID {cityId} is not found");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);
                var pointsOfInterestForCityResult = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity);

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

            var pointOfInterestResult = Mapper.Map<PointOfInterestDto>(pointOfInterest);
            return Ok(pointOfInterestResult);   
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

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterestEntity = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);
            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterestEntity);
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            var createdPointOfInterestToReturn = Mapper.Map<Models.PointOfInterestDto>(finalPointOfInterestEntity);

            // 201 successfully created
            return CreatedAtRoute("GetPointOfInterest",
                new { cityId = cityId, id = createdPointOfInterestToReturn.Id }, createdPointOfInterestToReturn);
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

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound(); // 404 not found
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(pointOfInterest, pointOfInterestEntity);

            // save to database (persistency)
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

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

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound(); // 404 not found
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointsOfInterestFromPatch = Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

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

            // map dto to entity
            Mapper.Map(pointsOfInterestFromPatch, pointOfInterestEntity);
            
            // save to database (persistency)
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }
            return NoContent();
        }


        [HttpDelete("{cityId}/pointsofinterest/{id}")] // delete
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound(); // 404 not found
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            // save to database (persistency)
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            // send mail when deleted
            _mailService.Send("Poin of Interest is deleted"
                , $"Point of Interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent(); // return 204 if successful
        }
    }
}
