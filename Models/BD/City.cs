using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWork.Models.BD
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Client> Clients { get; set; }
    }
}
