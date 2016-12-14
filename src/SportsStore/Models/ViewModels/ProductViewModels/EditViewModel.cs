using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models.Domain;

namespace SportsStore.Models.ViewModels.ProductViewModels
{
    public class EditViewModel
    {
        [HiddenInput]
        public int ProductId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(1, 3000, ErrorMessage = "{0} must be positive")]
        public int Price { get; set; }
        [Display(Name="In stock")]
        public bool InStock { get; set; }
        [Required]
        public Availability Availability { get; set; }
        [Display(Name="Available till")]
        [DataType(DataType.Date)]
        public DateTime? AvailableTill { get; set; }
        [Required]
        [Display(Name="Category")]
        public int CategoryId { get; set; }

        public EditViewModel()
        {
            
        }

        public EditViewModel(Product p)
        {
            ProductId = p.ProductId;
            Name = p.Name;
            Description = p.Description;
            Price = p.Price;
            InStock = p.InStock;
            Availability = p.Availability;
            AvailableTill = p.AvailableTill;
            if (p.Category!=null)
            CategoryId = p.Category.CategoryId;
        }
    }
}