using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        [HttpGet("{cityId}/pointsOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            // return error if cityId (parent) doesn't exist
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{cityId}/pointsOfInterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointsOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);

            if (pointsOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointsOfInterest);
        }


        [HttpPost("{cityId}/pointsofinterest")]
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

        [HttpPut("{cityId}/pointsofinterest/{id}")]
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


        [HttpPatch("{cityId}/pointsofinterest/{id}")]
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


        [HttpDelete("{cityId}/pointsofinterest")]
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
            return NoContent(); // return 204 if successful
        }
    }
}
