using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWork.Models.BD
{
    public class Sale
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? ClientId { get; set; }
        public Client Client { get; set; }

        public int? ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}
