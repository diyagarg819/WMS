using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Represents an external client associated with projects.
    /// Note: Column name "ClientAdress" matches the PDF schema spelling exactly.
    /// </summary>
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ClientName { get; set; } = string.Empty;

        // Intentionally spelled "ClientAdress" to match the database schema from the PDF
        public string? ClientAdress { get; set; }

        // 10-digit phone number stored as a numeric value
        [Column(TypeName = "decimal(10,0)")]
        public decimal? ClientPhoneNumber { get; set; }

        [MaxLength(20)]
        public string? ClientLocation { get; set; }

        // Whether this client is currently active
        public bool Status { get; set; } = true;

        // Projects linked to this client
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
