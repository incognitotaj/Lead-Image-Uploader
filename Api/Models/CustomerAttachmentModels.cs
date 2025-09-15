using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class CustomerAttachmentGetQueryModel
    {
        public Guid Id { get; init; }
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }
    }


    public class CustomerAttachmentCreateModel
    {
        [Required]
        public IFormFile File { get; set; }
    }

    public class CustomerAttachmentUpdateModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
