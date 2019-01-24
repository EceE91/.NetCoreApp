using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
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
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult  GetCities() //JsonResult
        {
            //return new JsonResult(new List<object>(){
            //    new { Id =1, Name="New York City"},
            //    new { Id =2, Name="Istanbul"}
            //});

            // return Ok(CitiesDataStore.Current.Cities);

            // jsonresult dönersen postman requestinden xml dönmez sadece json döner
            //return new JsonResult(CitiesDataStore.Current.Cities);
            var cityEntities = _cityInfoRepository.GetCities();
            var results = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);
            
            return Ok(results);
        }

        [HttpGet("{id}")] // Eg: http://localhost:1028/api/cities/1 ; http://localhost:1028/api/cities/1?includePointsOfInterest=true
        public IActionResult GetCity(int id, bool includePointsOfInterest = false) // JsonResult
        {
            //var entity = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            //if(entity == null)
            //{
            //    return NotFound();
            //}

            //return Ok(entity);

            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {

                var cityResult = Mapper.Map<CityDto>(city);

                return Ok(cityResult);
            }

            var cityWithoutPointsOfInterest = Mapper.Map<CityWithoutPointsOfInterestDto>(city);
            return Ok(cityWithoutPointsOfInterest);
        }
    }
}
