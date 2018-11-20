using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    public class CitiesController : Controller
    {
        [HttpGet("api/cities")]
        public JsonResult GetCities()
        {
            return new JsonResult(new List<object>(){
                new { Id =1, Name="New York"},
                new { Id =2, Name="Istanbul"}
            });
        }
    }
}
