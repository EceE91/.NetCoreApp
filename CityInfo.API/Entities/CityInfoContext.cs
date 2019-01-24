using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class CityInfoContext :DbContext
    {
        // for EF core we use code-first approach. 
        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
        {
            // request instance from container through dependency injection
            //Database.EnsureCreated(); // database has to be generated if it doesnt exist yet
            Database.Migrate();
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }
    }
}
