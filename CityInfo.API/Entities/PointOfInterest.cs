using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key] // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // identity
        public int Id { get; set; }

        [Required] // not null
        [MaxLength(50)]
        public string Name { get; set; }

        [ForeignKey("CityId")] // navigation property
        public City City { get; set; }
        public int CityId { get; set; }
    }
}
