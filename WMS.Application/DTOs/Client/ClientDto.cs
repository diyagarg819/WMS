namespace WMS.Application.DTOs.Client
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? ClientAdress { get; set; }
        public decimal? ClientPhoneNumber { get; set; }
        public string? ClientLocation { get; set; }
        public bool Status { get; set; }
    }
}
