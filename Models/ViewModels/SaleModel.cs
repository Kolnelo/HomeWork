using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HomeWork.Models.ViewModels
{
    public class SaleModel
    {
        [Required]
        public string Name { get; set; }
        public string ClientName { get; set; }
        public string ContactPerson { get; set; }
        public string Saller { get; set; }
        public int? CityId { get; set; }
    }
}
