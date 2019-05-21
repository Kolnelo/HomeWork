using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeWork.Models.BD;
using HomeWork.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HomeWork.Controllers
{
    [Route("api/[controller]")]
    public class SalesController : Controller
    {
        readonly private ApplicationContext _appContext;
        public SalesController(ApplicationContext applicationContext)
        {
            _appContext = applicationContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Object>> Get()
        {
            return await _appContext.Sales
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    ClientName = s.Client != null ? s.Client.Name != null ? s.Client.Name : "" : "",
                    ContactName = s.Contact != null ? s.Contact.ContactPerson != null ? s.Contact.ContactPerson : "" : "",
                    Saller = s.Contact != null ? s.Contact.Saler != null ? s.Contact.Saler : "" : "",
                    City = new
                    {
                        id = s.Client != null ? s.Client.CityId : null,
                        Name = s.Client != null ? s.Client.City != null ? s.Client.City.Name : "" : ""
                    }
                }).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Sale sale = await _appContext.Sales
                .Include(s => s.Contact)
                .Include(s => s.Client)
                .FirstOrDefaultAsync(s => s.Id == id);
            if(sale == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                sale.Id,
                sale.Name,
                ClientName = sale.Client?.Name,
                ContactName = sale.Contact?.ContactPerson,
                Saller = sale.Contact?.Saler,
                City = new { sale.Client?.City?.Id, sale.Client?.City?.Name }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SaleModel sale)
        {
            Sale newSale = new Sale { Name = sale.Name };

            if (sale.ClientName != null || sale.CityId != null)
            {
                Client newClient = null;

                if (sale.ClientName != null && sale.CityId != null)
                {
                    City city = await _appContext.Cities.FirstOrDefaultAsync(c => c.Id == sale.CityId);
                    newClient = new Client { Name = sale.ClientName, City = city };
                }
                else if (sale.ClientName != null)
                {
                    newClient = new Client { Name = sale.ClientName };
                }
                else if (sale.CityId != null)
                {
                    City city = await _appContext.Cities.FirstOrDefaultAsync(c => c.Id == sale.CityId);
                    if (city != null)
                    {
                        newClient = new Client { City = city };
                    }
                }

                if (newClient != null)
                {
                    _appContext.Clients.Add(newClient);
                    newSale.Client = newClient;
                }
            }

            if (sale.ContactPerson != null || sale.Saller != null)
            {
                Contact newContact = new Contact { ContactPerson = sale.ContactPerson, Saler = sale.Saller };
                _appContext.Contacts.Add(newContact);
                newSale.Contact = newContact;
            }

            _appContext.Sales.Add(newSale);
            await _appContext.SaveChangesAsync();

            return Ok(new {
                newSale.Id,
                newSale.Name,
                ClientName = newSale.Client?.Name ?? "",
                ContactName = newSale.Contact?.ContactPerson ?? "",
                Saller = newSale.Contact?.Saler,
                City = new { newSale.Client?.City?.Id, Name = newSale.Client?.City?.Name ?? "" }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]SaleModel sale)
        {
            Sale oldSale = await _appContext.Sales
                .Include(s => s.Client)
                .Include(s => s.Contact)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (oldSale == null)
            {
                return NotFound();
            }

            oldSale.Name = sale.Name;

            if (oldSale.Client == null && (sale.ClientName != null || sale.CityId != null))
            {
                Client newClient = new Client
                {
                    Name = sale.ClientName,
                    City = sale.CityId != null ? await _appContext.Cities.FirstOrDefaultAsync(c => c.Id == sale.CityId) : null
                };
                _appContext.Clients.Add(newClient);
                oldSale.Client = newClient;
            } else if (oldSale.Client != null)
            {
                oldSale.Client.Name = sale.ClientName;
                oldSale.Client.City = sale.CityId != null ? await _appContext.Cities.FirstOrDefaultAsync(c => c.Id == sale.CityId) : null;
                oldSale.Client.CityId = oldSale.Client.City?.Id;
            }

            if (oldSale.Contact == null && (sale.ContactPerson != null || sale.Saller != null))
            {
                Contact newContact = new Contact
                {
                    ContactPerson = sale.ContactPerson,
                    Saler = sale.Saller
                };
                _appContext.Contacts.Add(newContact);
                oldSale.Contact = newContact;
            } else if (oldSale.Contact != null)
            {
                oldSale.Contact.ContactPerson = sale.ContactPerson;
                oldSale.Contact.Saler = sale.Saller;
            }

            await _appContext.SaveChangesAsync();

            return Ok(new
            {
                oldSale.Id,
                oldSale.Name,
                ClientName = oldSale.Client?.Name ?? "",
                ContactName = oldSale.Contact?.ContactPerson ?? "",
                Saller = oldSale.Contact?.Saler,
                City = new { oldSale.Client?.City?.Id, Name = oldSale.Client?.City?.Name ?? "" }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Sale sale = await _appContext.Sales.FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null)
            {
                return NotFound();
            }
            if (sale.ClientId != null)
            {
                Client client = await _appContext.Clients.FirstOrDefaultAsync(c => c.Id == sale.ClientId);
                if (client != null)
                {
                    _appContext.Clients.Remove(client);
                }
            }
            if (sale.ContactId != null)
            {
                Contact contact = await _appContext.Contacts.FirstOrDefaultAsync(c => c.Id == sale.ContactId);
                if (contact != null)
                {
                    _appContext.Contacts.Remove(contact);
                }
            }
            _appContext.Sales.Remove(sale);
            await _appContext.SaveChangesAsync();
            return Ok();
        }
    }
}
