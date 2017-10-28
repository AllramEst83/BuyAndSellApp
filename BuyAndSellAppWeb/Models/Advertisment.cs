using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuyAndSellAppWeb.Models
{
    public class Advertisment
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "En titel behövs")]
        [Display(Name = "Produkttitle")]
        public string ProductTitle { get; set; }

        [Required(ErrorMessage = "En beskrivning behövs")]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Priset har fel format eller saknas")]
        [Display(Name = "Pris")]
        public decimal Price { get; set; }

        [Display(Name = "Skapad")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MMM/dd}")]
        public DateTime Created { get; set; }

        [Display(Name = "Kategori")]
        public string Category { get; set; }

        [Url]
        [Display(Name = "Bildens URL")]
        [Required(ErrorMessage = "Endast en giltig URL")]    
        public string ImageUrl { get; set; }

        [Display(Name = "Säljare")]
        public string Seller { get; set; }

        public bool SellerToken { get; set; }

    }
}