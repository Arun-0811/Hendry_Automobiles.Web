
using Hendry_Auto.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hendry_Auto.Infrastructure.Common;
using Hendry_Auto.Application.ApplicationConstants;

namespace Hendry_Auto.Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BrandController(ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _applicationDbContext=applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Brand> brands = _applicationDbContext.Brands.ToList();
            return View(brands);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            string WebRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if(file.Count>0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(WebRootPath, @"images\brand");
                var extention = Path.GetExtension(file[0].FileName);
                using(var fileStream = new FileStream(Path.Combine(upload, newFileName + extention), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                brand.BrandLogo = @"\images\brand\" + newFileName + extention;

            }
            if(ModelState.IsValid)
            {
                _applicationDbContext.Brands.Add(brand);
                _applicationDbContext.SaveChanges();
                TempData["Success"] = CommonMessage.RecordCreated;
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }
        [HttpGet]
        public IActionResult Details(Guid id)
        {
            Brand brand = _applicationDbContext.Brands.FirstOrDefault(x => x.Id == id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            Brand brand = _applicationDbContext.Brands.FirstOrDefault(x => x.Id == id);
            if(brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }
        [HttpPost]
        public IActionResult Edit(Brand brand)
        {
            string WebRootPath = _webHostEnvironment.WebRootPath;
            var file = HttpContext.Request.Form.Files;
            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(WebRootPath, @"images\brand");
                var extention = Path.GetExtension(file[0].FileName);

                // Delete old file if it exists
                var objFromDb = _applicationDbContext.Brands.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id);

                if(objFromDb.BrandLogo != null)
                {
                    var oldImagePath = Path.Combine(WebRootPath, objFromDb.BrandLogo.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extention), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                brand.BrandLogo = @"\images\brand\" + newFileName + extention;
            }
            if (ModelState.IsValid)
            {
                var objFromDb = _applicationDbContext.Brands.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id);
                objFromDb.Name = brand.Name;
                objFromDb.EstablishedYear = brand.EstablishedYear;
                if (brand.BrandLogo != null) 
                {
                    objFromDb.BrandLogo = brand.BrandLogo;
                }
                else
                {
                    objFromDb.BrandLogo = objFromDb.BrandLogo; // Keep the old logo if no new one is provided
                }
                _applicationDbContext.Brands.Update(objFromDb);
                _applicationDbContext.SaveChanges();
                TempData["Warning"] = CommonMessage.RecordUpdated;
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            Brand brand = _applicationDbContext.Brands.FirstOrDefault(x => x.Id == id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }
        [HttpPost]
        public IActionResult Delete(Brand brand)
        {
            string WebRootPath = _webHostEnvironment.WebRootPath;
            if(!string.IsNullOrEmpty(brand.BrandLogo))
            {
                // Delete old file if it exists
                var objFromDb = _applicationDbContext.Brands.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id);

                if (objFromDb.BrandLogo != null)
                {
                    var oldImagePath = Path.Combine(WebRootPath, objFromDb.BrandLogo.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

            }
            _applicationDbContext.Brands.Remove(brand);
            _applicationDbContext.SaveChanges();
            TempData["Error"] = CommonMessage.RecordDeleted;
            return RedirectToAction(nameof(Index));
        }
    }
}
