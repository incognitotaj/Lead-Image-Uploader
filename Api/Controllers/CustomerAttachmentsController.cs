using Api.Entities;
using Api.Infrastructure.Database;
using Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    /// <summary>
    /// Customer Attachment API
    /// </summary>
    /// <param name="context"></param>
    [Route("api/customers/{customerId:guid}/customer-attachments")]
    [ApiController]
    public class CustomerAttachmentsController(ApplicationDbContext context) : ControllerBase
    {
        /// <summary>
        /// Gets all attachments by customer ID
        /// </summary>
        /// <param name="customerId">The ID of the customer</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <response code="200">Returned the attachment</response>
        /// <response code="404">Returned when no attachment with provided ID was found</response>
        [HttpGet()]
        public async Task<Results<NotFound<string>, Ok<List<CustomerAttachmentGetQueryModel>>>> GetById(Guid customerId, CancellationToken cancellationToken)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(p => p.Id == customerId, cancellationToken);
            if (customer == null)
            {
                return TypedResults.NotFound($"Customer {customerId} not found");
            }

            var result = await context.CustomerAttachments
                .Where(p => p.CustomerId == customerId)
                .Select(p => new CustomerAttachmentGetQueryModel
                {
                    Id = p.Id,
                    FileName = p.FileName,
                    ImageData = p.ImageData
                })
                .ToListAsync(cancellationToken);

            return TypedResults.Ok(result);
        }

        /// <summary>
        /// Gets specific attachment by ID
        /// </summary>
        /// <param name="id">The ID of the customer attachment</param>
        /// <param name="customerId">The ID of the customer</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <response code="200">Returned the attachment</response>
        /// <response code="404">Returned when no attachment with provided ID was found</response>
        [HttpGet("{id:guid}")]
        public async Task<Results<NotFound<string>, Ok<CustomerAttachmentGetQueryModel>>> GetById(Guid customerId, Guid id, CancellationToken cancellationToken)
        {
            var campus = await context.Customers
                .FirstOrDefaultAsync(p => p.Id == customerId, cancellationToken);
            if (campus == null)
            {
                return TypedResults.NotFound($"Customer {customerId} not found");
            }

            var attachment = await context.CustomerAttachments
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (attachment == null)
            {
                return TypedResults.NotFound($"Customer attachment {id} not found");
            }

            return TypedResults.Ok(new CustomerAttachmentGetQueryModel
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                ImageData = attachment.ImageData
            });
        }

        /// <summary>
        /// Creates a new application attachment
        /// </summary>
        /// <param name="customerId">The ID of the customer</param>
        /// <param name="model">The model to create a customer attachment from</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <response code="200">Returned the customer attachment ID</response>
        /// <response code="400">Returned the customer attachment can not be created with the provided model</response>
        /// <response code="404">Returned when no customer with provided ID was found</response>
        [HttpPost()]
        [Consumes("multipart/form-data")]
        public async Task<Results<NotFound<string>, BadRequest, ProblemHttpResult, Created<Guid>>> Create(Guid customerId, [FromForm] CustomerAttachmentCreateModel model, CancellationToken cancellationToken)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(p => p.Id == customerId, cancellationToken);
            if (customer == null)
            {
                return TypedResults.NotFound($"Customer {customerId} not found");
            }

            CustomerAttachment attachment;

            using (var memoryStream = new MemoryStream())
            {
                await model.File.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                attachment = new CustomerAttachment
                {
                    CustomerId = customer.Id,
                    FileName = model.File.Name,
                    ImageData = imageBytes
                };
                await context.CustomerAttachments.AddAsync(attachment, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.Created(Url.Action(nameof(this.GetById), new { id = attachment.Id }), attachment.Id);
        }

        /// <summary>
        /// Delete an existing customer attachment
        /// </summary>
        /// <param name="customerId">The ID of the customer</param>
        /// <param name="customerAttachmentId">The ID of the customer attachment to delete</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <response code="204">Returned when the updates are made successfully</response>
        /// <response code="400">Returned the customer can not be updated with the provided model</response>
        /// <response code="404">Returned when no customer with provided ID was found</response>
        [HttpDelete("{customerAttachmentId:guid}")]
        public async Task<Results<NotFound<string>, ProblemHttpResult, NoContent>> Delete(Guid customerId, Guid customerAttachmentId, CancellationToken cancellationToken)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(p => p.Id == customerId, cancellationToken);
            if (customer == null)
            {
                return TypedResults.NotFound($"Customer {customerId} not found");
            }

            var attachment = await context.CustomerAttachments
                .FirstOrDefaultAsync(p => p.Id == customerAttachmentId, cancellationToken);
            if (attachment == null)
            {
                return TypedResults.NotFound($"Customer attachment {customerAttachmentId} not found");
            }

            context.CustomerAttachments.Remove(attachment);

            await context.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        }
    }
}
