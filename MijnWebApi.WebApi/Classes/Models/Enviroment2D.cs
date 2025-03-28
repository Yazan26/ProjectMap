using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MijnWebApi.WebApi.Classes.Models
{
    public class Environment2D
    {
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid(); // ✅ Automatically generate a GUID

        [Required]
        public string Name { get; set; }
        public int? MaxHeight { get; set; } // Y, Nullable
        public int? MaxWidth { get; set; } // X, nullablle

        [Required]
        [Column("OwnerUserID")] // ✅ Ensures correct DB column type
        public Guid OwnerUserID { get; set; }
    }
}