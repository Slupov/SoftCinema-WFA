﻿using System.ComponentModel.DataAnnotations.Schema;

namespace SoftCinema.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Movie
    {
        public Movie()
        {
            this.Cast = new HashSet<Actor>();
            this.Categories = new HashSet<Category>();
        }

        [Key]
        public int Id { get; set; }

        [Required, Index("IX_NameYear", 1, IsUnique = true), MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int Length { get; set; }

        [Range(0, 10)]
        public float? Rating { get; set; }

        public string Synopsis { get; set; }

        public virtual ICollection<Actor> Cast { get; set; }

        [Required]
        public string DirectorName { get; set; }

        [Required, Index("IX_NameYear", 2, IsUnique = true)]
        public int ReleaseYear { get; set; }

        public string ReleaseCountry { get; set; }

        [Required]
        public AgeRestriction AgeRestriction { get; set; }

        [Required]
        public int ImageId { get; set; }

        public virtual Image Image { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}