using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Kevin.Models.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public int ProductId { get; set; }

        [JsonIgnore] // This prevents circular reference during serialization
        [ForeignKey("ProductId")]
        public ProductsEntity Product { get; set; }

    }
}
