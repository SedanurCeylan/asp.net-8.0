using System.ComponentModel.DataAnnotations;

namespace FormsApp.Models{


    public class Product{

        [Display(Name="Urun Id")]
         public int ProductId { get; set; }

         [Display(Name="Urun Adı")]
         [Required]
         [StringLength(15)]
         public string? Name { get; set; }

         [Required(ErrorMessage ="gerekli bir alan")]
         [Range(0,100000)]
         [Display(Name="Fiyat")]
         public decimal? Price { get; set; }
         //decimal default olarak 0 değerini alır required kullanmam için null değerini alabilmesi lazım ki hata döndürsün o yüzden soru işareti koyarım ki null olabilsin

         
         [Display(Name="Resim")]
         public string? Image { get; set; }
         public bool IsActive { get; set; }

         [Required]
         [Display(Name="Category")]
         public int? CategoryId { get; set; }
 


    }
}

//1 numaralı ürün  iphone 14 1 id kategorisi mesela