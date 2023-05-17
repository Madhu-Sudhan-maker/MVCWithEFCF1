using MVCWithEFCF1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;

using PagedList.Mvc;
using PagedList;

namespace MVCWithEFCF1.Controllers
{

    public class ProductController : Controller
    {
        StoreDB dc = new StoreDB();
        public ViewResult DisplayProducts(int? i)
        {
            dc.Configuration.LazyLoadingEnabled = false;
            var products = dc.Products.Include(P => P.Category).Where(P => P.Discontinued == false).OrderBy(P =>P.Discontinued).ToPagedList(i ?? 1,1);
            return View(products);
        }
        public ViewResult DisplayProduct(int Id)
        {
            dc.Configuration.LazyLoadingEnabled = false;
            Product product = (dc.Products.Include(P => P.Category).Where(P => P.Id == Id && P.Discontinued == false)).Single();
            return View(product);
        }
        public ViewResult AddProduct()
        {
            ViewBag.CategoryId = new SelectList(dc.Categories, "CategoryId", "CategoryName");
            Product product = new Product();
            return View(product);
        }
        [HttpPost]
        public RedirectToRouteResult AddProduct(Product product, HttpPostedFileBase selectedFile)
        {
            if (selectedFile != null)
            {
                string DirectoryPath = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }
                selectedFile.SaveAs(DirectoryPath + selectedFile.FileName);
                BinaryReader br = new BinaryReader(selectedFile.InputStream);
                product.ProductImage = br.ReadBytes(selectedFile.ContentLength);
                product.ProductImageName = selectedFile.FileName;
            }
            dc.Products.Add(product);
            dc.SaveChanges();
            return RedirectToAction("DisplayProducts");
        }
        public ViewResult EditProduct(int Id)
        {
            Product product = dc.Products.Find(Id);
            TempData["ProductImage"] = product.ProductImage;
            TempData["ProductImageName"] = product.ProductImageName;
            ViewBag.CategoryId = new SelectList(dc.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
        public RedirectToRouteResult UpdateProduct(Product product, HttpPostedFileBase selectedFile)
        {
            if (selectedFile != null)
            {
                string DirectoryPath = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }
                selectedFile.SaveAs(DirectoryPath + selectedFile.FileName);
                BinaryReader br = new BinaryReader(selectedFile.InputStream);
                product.ProductImage = br.ReadBytes(selectedFile.ContentLength);
                product.ProductImageName = selectedFile.FileName;
            }
            else if (TempData["ProductImage"] != null && TempData["ProductImageName"] != null)
            {
                product.ProductImage = (byte[])TempData["ProductImage"];
                product.ProductImageName = (string)TempData["ProductImageName"];
            }
            dc.Entry(product).State = EntityState.Modified;
            dc.SaveChanges();
            return RedirectToAction("DisplayProducts");
        }
        public RedirectToRouteResult DeleteProduct(int Id)
        {
            Product product = dc.Products.Find(Id);
            product.Discontinued = true;
            dc.Entry(product).State = EntityState.Modified;
            dc.SaveChanges();
            return RedirectToAction("DisplayProducts");
        }
    }
}