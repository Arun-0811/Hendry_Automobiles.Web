using Hendry_Auto.Domain.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hendry_Auto.Domain.ViewModel
{
    public class HomePostVM
    {
        public List<Post> Posts { get; set; }

        public string? SearchBox { get; set; }=string.Empty;

        public Guid? BrandId { get; set; }

        public Guid? VehicleTypeId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> BrandList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> VehicleTypeList { get; set; }
    }
}
