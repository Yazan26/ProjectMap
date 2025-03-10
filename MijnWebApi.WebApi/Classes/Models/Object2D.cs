using System.ComponentModel.DataAnnotations;

namespace MijnWebApi.WebApi.Classes
{
    public class Object2D
    {
        public Guid Id { get; set; }

        [Required]
        public String PrefabId { get; set; }

        [Required]
        public int PositionX { get; set; }

        [Required]
        public int PositionY { get; set; }

        [Required]
        public int ScaleX { get; set; }

        [Required]
        public int ScaleY { get; set; }

        [Required]
        public int RotationZ { get; set; }

        [Required]
        public int SortingLayer { get; set; }

        public Guid? Environment2DID { get; set; } // Nullable because `worldId` allows NULL in your schema
    }
}
