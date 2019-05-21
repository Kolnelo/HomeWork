using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWork.Models.BD
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? CityId { get; set; }
        public City City { get; set; }
    }
}
