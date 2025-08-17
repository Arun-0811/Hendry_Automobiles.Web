using Hendry_Auto.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hendry_Auto.Domain.Models
{
    public class Brand : BaseModel
    {
        
        [Required]
        public string Name { get; set; }

        [Display(Name = "Established Year")]
        public int EstablishedYear { get; set; }

        [Display(Name = "Brand Logo")]
        public string BrandLogo { get; set; }
    }
}
