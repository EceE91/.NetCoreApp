using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        // constuctor injection
        private CityInfoContext _ctx;

        public CityInfoRepository(CityInfoContext context)
        {
            _ctx = context;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            // added only to object not to database yet
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool CityExists(int cityId)
        {
            return _ctx.Cities.Any(c => c.Id == cityId);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _ctx.PointsOfInterest.Remove(pointOfInterest); // remove from dbset
        }

        public IEnumerable<City> GetCities()
        {
            return _ctx.Cities.OrderBy(m => m.Name).ToList(); //immediate execution
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _ctx.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId).FirstOrDefault();
            }
            return _ctx.Cities.Where(c => c.Id == cityId).FirstOrDefault();
        }


        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _ctx.PointsOfInterest.Where(c => c.Id == pointOfInterestId && c.CityId == cityId)
                .FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _ctx.PointsOfInterest.Where(c => c.CityId == cityId).ToList();
        }

        public bool Save()
        {
            return (_ctx.SaveChanges() >= 0);
        }
    }
}
