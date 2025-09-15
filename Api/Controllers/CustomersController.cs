using Api.Entities;
using Api.Infrastructure.Database;
using Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    /// <summary>
    /// Customers API
    /// </summary>
    /// <param name="context"></param>
    [Route("api/customers")]
    [ApiController]
    public class CustomersController(ApplicationDbContext context) : ControllerBase
    {
        /// <summary>
        /// Gets list of the customers
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The list of parents</returns>
        /// <response code="200">Returned the list of customers</response>
        [HttpGet()]
        public async Task<Ok<IEnumerable<CustomerGetQueryModel>>> Get(CancellationToken cancellationToken)
        {
            var result = await context.Customers.ToListAsync(cancellationToken);

            return TypedResults.Ok(result.Select(p => new CustomerGetQueryModel
            {
                Id = p.Id,
                Name = p.Name,
                Email  = p.Email,
            }));
        }

        /// <summary>
        /// Gets customer by id
        /// </summary>
        /// <param name="customerId">The ID of the customer to return</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The list of parents</returns>
        /// <response code="200">Returned the list of customers</response>
        [HttpGet("{customerId:guid}")]
        public async Task<Results<NotFound<string>, Ok<CustomerGetQueryModel>>> GetById(Guid customerId, CancellationToken cancellationToken)
        {
            var entity = await context.Customers
                .FirstOrDefaultAsync(p => p.Id == customerId, cancellationToken);
            if (entity == null)
            {
                return TypedResults.NotFound($"Customer {customerId} not found");
            }

            return TypedResults.Ok(new CustomerGetQueryModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
            });
        }

        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <param name="model">The model to create a customer from</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Returns the ID of the created customer</returns>
        /// <response code="200">Returned the ID of the newly created customer</response>
        /// <response code="400">Returned the customer can not be created with the provided model</response>
        [HttpPost()]
        public async Task<Results<NotFound<string>, BadRequest, ProblemHttpResult, Created<Guid>>> Create([FromBody] CustomerCreateModel model, CancellationToken cancellationToken)
        {
            var entity = new Customer
            {
                Name = model.Name,
                Email = model.Email
            };

            await context.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Created(Url.Action(nameof(this.GetById), new { id = entity.Id }), entity.Id);
        }

        /// <summary>
        /// Updates an existing customer
        /// </summary>
        /// <param name="customerId">The ID of the customer to return</param>
        /// <param name="model">The model to update a customer from</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Returns the ID of the created customer</returns>
        /// <response code="200">Returned the ID of the newly created customer</response>
        /// <response code="400">Returned the customer can not be created with the provided model</response>
        [HttpPut("{customerId:guid}")]
        public async Task<Results<NotFound<string>, BadRequest, ProblemHttpResult, NoContent>> Update(Guid customerId, [FromBody] CustomerUpdateModel model, CancellationToken cancellationToken)
        {
            var entity = await context.Customers
                .FirstOrDefaultAsync(p => p.Id == customerId, cancellationToken);
            if (entity == null)
            {
                return TypedResults.NotFound($"Customer {customerId} not found");
            }

            entity.Name = model.Name;
            entity.Email = model.Email;

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        }

        /// <summary>
        /// Deltes an existing customer
        /// </summary>
        /// <param name="customerId">The ID of the customer to remove</param>
        /// <param name="model">The model to update a customer from</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Returns the ID of the created customer</returns>
        /// <response code="200">Returned the ID of the newly created customer</response>
        /// <response code="400">Returned the customer can not be created with the provided model</response>
        [HttpDelete("{customerId:guid}")]
        public async Task<Results<NotFound<string>, BadRequest, ProblemHttpResult, NoContent>> Delete(Guid customerId, [FromBody] CustomerUpdateModel model, CancellationToken cancellationToken)
        {
            var entity = await context.Customers
                .FirstOrDefaultAsync(p => p.Id == customerId, cancellationToken);
            if (entity == null)
            {
                return TypedResults.NotFound($"Customer {customerId} not found");
            }

            context.Customers.Remove(entity);

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        }
    }
}
