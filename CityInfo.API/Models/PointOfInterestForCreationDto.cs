using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class PointOfInterestForCreationDto
    {
        // no id field in here, since the id wont be created by the client

        [Required(ErrorMessage ="Enter a valid name.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
