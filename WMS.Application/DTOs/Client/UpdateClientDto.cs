using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Client
{
    public class UpdateClientDto
    {
        [Required]
        [MaxLength(100)]
        public string ClientName { get; set; } = string.Empty;
        public string? ClientAdress { get; set; }
        public decimal? ClientPhoneNumber { get; set; }
        
        [MaxLength(20)]
        public string? ClientLocation { get; set; }
        public bool Status { get; set; }
    }
}
