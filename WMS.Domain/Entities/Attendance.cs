using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [ForeignKey("Employee")]
        public int EmpId { get; set; }
        public Employee? Employee { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CheckIn { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime? CheckOut { get; set; }

        [Column(TypeName = "FLOAT")]
        public float? TotalHours { get; set; }

        [Column(TypeName = "VARCHAR(20)")]
        public string? WorkMode { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime AttendanceDate { get; set; }
    }
}
