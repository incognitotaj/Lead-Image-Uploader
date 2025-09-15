namespace Api.Models
{
    public class CustomerGetQueryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class CustomerCreateModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class CustomerUpdateModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
