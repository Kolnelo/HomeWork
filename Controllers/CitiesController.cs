using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeWork.Models.BD;
using Microsoft.EntityFrameworkCore;

namespace HomeWork.Controllers
{
    [Route("api/[controller]")]
    public class CitiesController : Controller
    {
        readonly private ApplicationContext _appContext;
        public CitiesController(ApplicationContext applicationContext)
        {
            _appContext = applicationContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!await _appContext.Cities.AnyAsync())
            {
                await _appContext.Cities.AddRangeAsync(
                    new City { Name = "Сыктывкар" },
                    new City { Name = "Москва" },
                    new City { Name = "Санкт-Петербург" },
                    new City { Name = "Ухта" },
                    new City { Name = "Воркута" }
                );
                await _appContext.SaveChangesAsync();
            }
            IList<City> cities = await _appContext.Cities.ToListAsync();

            return Ok(cities);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
