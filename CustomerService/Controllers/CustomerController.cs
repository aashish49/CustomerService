using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using CustomerService.Entities;
using CustomerService.Business;

namespace CustomerService.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController
        : ControllerBase
    {
        private readonly IBizManager<Customer> _ibizManager;

        public CustomerController(IBizManager<Customer> ibizManager)
        {
            _ibizManager = ibizManager;
        }

        // GET: api/Customer
        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var response = _ibizManager.GetAll();

            if (response != null)
            {
                return Ok(response);        // 200 - OK
            }

            return NotFound();              // 404 - Not Found.
        }

        // GET: api/Customer/123456
        [HttpGet("{id}", Name = "GetCustomerByID")]
        public IActionResult GetCustomerByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Invalid Customer Id");           //400 - Bad request.
            }
            var response = _ibizManager.GetByID(id);

            if (response == null)
            {
                return NotFound();                                  // 404 - Not Found.
            }

            return Ok(response);                                    // 200 - OK
        }

        // POST: api/Customer
        [Authorize(Policy = Policies.ADMIN_ROLE)]
        [HttpPost]
        public IActionResult AddCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return base.BadRequest();                           // 400 - Bad Request.
            }

            _ibizManager.Add(customer);
            return CreatedAtRoute("GetCustomerByID", new { id = customer.Id }, customer);
        }

        // PUT: api/Customer/123-12-12456
        [HttpPut("{id}")]
        public IActionResult UpdateCustomerByID(string id, [FromBody] Customer customer)
        {
            if (string.IsNullOrWhiteSpace(id) || id != customer.Id || customer == null)
            {
                return BadRequest();                                // 400 - Bad Request.
            }

            var customerById = _ibizManager.GetByID(id);

            if (customerById == null)
            {
                return NotFound();                                  // 404 - Not found
            }

            _ibizManager.UpdateByID(id, customer);
            return new NoContentResult();                           // 204 - No Content
        }

        // DELETE: api/Customer/123456
        [Authorize(Policy = Policies.ADMIN_ROLE)]
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomerByID(string id)
        {
            if (string.IsNullOrWhiteSpace (id))
            {
                return BadRequest();
            }

            var wasDeleted  = _ibizManager.DeleteByID(id);

            if (!wasDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
