using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        [HttpGet()]
        public IActionResult  GetCities() //JsonResult
        {
            //return new JsonResult(new List<object>(){
            //    new { Id =1, Name="New York City"},
            //    new { Id =2, Name="Istanbul"}
            //});

            // return Ok(CitiesDataStore.Current.Cities);

            // jsonresult dönersen postman requestinden xml dönmez sadece json döner
            return new JsonResult(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")] // Eg: http://localhost:1028/api/cities/1
        public IActionResult GetCity(int id) // JsonResult
        {
            var entity = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            if(entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }
    }
}
