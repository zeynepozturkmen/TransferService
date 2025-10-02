

namespace TransferService.Infrastructure.Response
{
    public class CustomerResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string NationalId { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
