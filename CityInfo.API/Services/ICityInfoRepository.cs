using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();
        City GetCity(int cityId);
        IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId, bool includePointsOfInterest);
        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);

    }
}
