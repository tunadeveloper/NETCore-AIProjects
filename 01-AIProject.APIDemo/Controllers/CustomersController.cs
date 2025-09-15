using _01_AIProject.APIDemo.DataAccess.Context;
using _01_AIProject.APIDemo.DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _01_AIProject.APIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApiContext _context;

        public CustomersController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet("CustomerList")]
        public IActionResult CustomerList()
        {
            var values = _context.Customers.ToList();
            return Ok(values);
        }

        [HttpPost("CreateCustomer")]
        public IActionResult CreateCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return Ok("Müşteri Başarıyla Eklendi!");
        }

        [HttpPut("UpdateCustomer")]
        public IActionResult UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
            return Ok("Müşteri Başarıyla Güncellendi!");
        }

        [HttpDelete("DeleteCustomer")]
        public IActionResult DeleteCustomer(int id)
        {
            var value = _context.Customers.Find(id);
            _context.Customers.Remove(value);
            _context.SaveChanges();
            return Ok("Müşteri Başarıyla Silindi!");
        }

        [HttpGet("GetById")]
        public IActionResult GetByIdCustomer(int id)
        {
            var value = _context.Customers.Find(id);
            return Ok(value);
        }
    }
}
