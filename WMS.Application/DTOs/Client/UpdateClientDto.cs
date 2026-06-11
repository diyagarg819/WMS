using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Client
{
    public class UpdateClientDto
    {
        [Required]
        [MaxLength(100)]
        public string ClientName { get; set; } = string.Empty;
        [Required]
        public string ClientAdress { get; set; } = string.Empty;
        
        [Required]
        public decimal ClientPhoneNumber { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string ClientLocation { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
