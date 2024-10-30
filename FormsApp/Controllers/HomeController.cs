using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FormsApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FormsApp.Controllers;

public class HomeController : Controller
{


    public HomeController()
    {
        
    }

    //buraya verdiğim parametreyi doğrudan urlye yazdığım parametreden ya da root içerisinde tanımlanmış olan route parametreinden alır(idnin orası) bu verdiğim parametre ile araam kısımı filtreleme işlemini halledeceğiz
    public IActionResult Index(string searchString, string category)
    {
        var products = Repository.Products;

        if(!String.IsNullOrEmpty(searchString))
        {
            ViewBag.SearchString = searchString;
            products = products.Where(p=>p.Name! .ToLower().Contains(searchString)).ToList();
        }

        if(!String.IsNullOrEmpty(category) && category != "0")
        {
            products= products.Where(p=> p.CategoryId == int.Parse(category)).ToList();
        }


        // ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId" , "Name", category);

        var model = new ProductViewModel{

            Products = products,
            Categories = Repository.Categories,
            SelectedCategory = category
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId" , "Name");
        return View();
    }


    //formun sadece set etmek istediğimiz alanları bind ile alırız [Bind("Name", "Price")] product modelden önce yazarız parantezin içine başka yöntemleri de var video 49 
    [HttpPost]    
    public async Task<IActionResult> Create(Product model, IFormFile imageFile)
    {

        //var allowedExtensions = new[]{".jpg",".jpeg",".png"};


        //var extension = Path.GetExtension(imageFile.FileName); 
        //abc.jpg .jpg kısmını extensiona aktarırım

        //var randomFileName =string.Format($"{Guid.NewGuid().ToString()}{extension}"); //siteden yüklenen imagelere rendom isim atarız çünkü aynı isimli olursa üzerine yazar. sonuna da extensionu ekleriz .jpg vsvs
 
        //var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img", randomFileName);

        var extension= "";
        if(imageFile != null)
        {
            var allowedExtensions = new[]{".jpg",".jpeg",".png"};
            extension = Path.GetExtension(imageFile.FileName); 

            if(!allowedExtensions.Contains(extension)){
                ModelState.AddModelError("","Geçerli Bir Resim Seçiniz");
            }
        }
 
        if(ModelState.IsValid)
        {
            if(imageFile != null)
            {
                var randomFileName =string.Format($"{Guid.NewGuid().ToString()}{extension}"); 
                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img", randomFileName);

                using (var stream = new FileStream(path, FileMode.Create)){
                    await imageFile.CopyToAsync(stream);
                }
                model.Image= randomFileName;
                model.ProductId=Repository.Products.Count+1;
                Repository.CreateProduct(model);
                //bu metot çalıştıktan sonra ındex metodunu çalıştır
                return RedirectToAction("Index");

            }
            
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId" , "Name");
        return View(model);
    }


    public IActionResult Edit(int? id){
        if(id == null){
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p=> p.ProductId == id);
        if(entity== null){
            return NotFound();
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId" , "Name");
        return View(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product model,IFormFile? imageFile)
    {
        if(id != model.ProductId){
            return NotFound();
        }

        if(ModelState.IsValid){

            if(imageFile != null) 
            {
                var extension = Path.GetExtension(imageFile.FileName); 
                var randomFileName =string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img", randomFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.Image = randomFileName;
            }
            Repository.EditProduct(model);
            return RedirectToAction("Index");
        }
         ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId" , "Name");
        return View(model);

    }

    public IActionResult Delete(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p=>p.ProductId == id);
        if(entity==null){
            return NotFound();
        }
        return View("DeleteConfirm", entity);

    }
    
    [HttpPost]
    public IActionResult Delete(int id, int ProductId)
    {
        if(id != ProductId)
        {
            return NotFound();
        }

        var entity = Repository.Products.FirstOrDefault(p=>p.ProductId == ProductId);
        if(entity==null){
            return NotFound();
        }
        Repository.DeleteProduct(entity);
        return RedirectToAction("Index");

    }
 
    [HttpPost]
    public IActionResult EditProducts(List<Product> Products)
    {
        foreach (var product in Products)
        {
            Repository.EditIsActive(product);
        }
        return RedirectToAction("Index");
    }


} 
