using System;
using System.ComponentModel.DataAnnotations;

namespace MijnWebApi.WebApi.Classes.Models
{
    public class Object2D
    {
        public Guid Id { get; set; }


        [Required]
        public Guid Environment2DID { get; set; }

        [Required]
        public string PrefabId { get; set; }

        [Required]
        public float PositionX { get; set; }

        [Required]
        public float PositionY { get; set; }

        [Required]
        public float ScaleX { get; set; }

        [Required]
        public float ScaleY { get; set; }

        [Required]
        public float RotationZ { get; set; }

        [Required]
        public float SortingLayer { get; set; }
        public Guid UserID { get; set; } // ✅ Nieuw veld toegevoegd!
    }
}
