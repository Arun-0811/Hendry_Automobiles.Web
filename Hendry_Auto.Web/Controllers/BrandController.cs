
using Hendry_Auto.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hendry_Auto.Infrastructure.Common;
using Hendry_Auto.Application.ApplicationConstants;
using Hendry_Auto.Application.Contracts.Persistence;
using System.Threading.Tasks;

namespace Hendry_Auto.Web.Controllers
{
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BrandController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = UnitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Brand> brands =await _UnitOfWork.Brand.GetAllAsync();
            return View(brands);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
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
                await _UnitOfWork.Brand.Create(brand);
                await _UnitOfWork.SaveAsync();
                TempData["Success"] = CommonMessage.RecordCreated;
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            Brand brand = await _UnitOfWork.Brand.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Brand brand = await _UnitOfWork.Brand.GetByIdAsync(id);
            if(brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand)
        {
            string WebRootPath = _webHostEnvironment.WebRootPath;
            var file = HttpContext.Request.Form.Files;
            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(WebRootPath, @"images\brand");
                var extention = Path.GetExtension(file[0].FileName);

                // Delete old file if it exists
                var objFromDb = await _UnitOfWork.Brand.GetByIdAsync(brand.Id);

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
                var objFromDb = await _UnitOfWork.Brand.GetByIdAsync(brand.Id);
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
                await _UnitOfWork.Brand.Update(brand);
                await _UnitOfWork.SaveAsync();
                TempData["Warning"] = CommonMessage.RecordUpdated;
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Brand brand = await _UnitOfWork.Brand.GetByIdAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Brand brand)
        {
            string WebRootPath = _webHostEnvironment.WebRootPath;
            if(!string.IsNullOrEmpty(brand.BrandLogo))
            {
                // Delete old file if it exists
                var objFromDb = await _UnitOfWork.Brand.GetByIdAsync(brand.Id);

                if (objFromDb.BrandLogo != null)
                {
                    var oldImagePath = Path.Combine(WebRootPath, objFromDb.BrandLogo.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

            }
            await _UnitOfWork.Brand.Delete(brand);
            await _UnitOfWork.SaveAsync();
            TempData["Error"] = CommonMessage.RecordDeleted;
            return RedirectToAction(nameof(Index));
        }
    }
}
