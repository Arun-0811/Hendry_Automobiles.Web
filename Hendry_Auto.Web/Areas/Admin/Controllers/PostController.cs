


using Hendry_Auto.Application.ApplicationConstants;
using Hendry_Auto.Application.Contracts.Persistence;
using Hendry_Auto.Application.Services.Interface;
using Hendry_Auto.Domain.ApplicationEnums;
using Hendry_Auto.Domain.Models;
using Hendry_Auto.Domain.ViewModel;
using Hendry_Auto.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;



namespace Hendry_Auto.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = CustomRole.MasterAdmin + "," + CustomRole.Admin)]
    public class PostController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUserNameService _userNameService;
        public PostController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment, IUserNameService userNameService)
        {
            _UnitOfWork = UnitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userNameService = userNameService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Domain.Models.Post> Posts =await _UnitOfWork.Post.GetAllPost();
            return View(Posts);
        }
        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> BrandList = _UnitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> VehicleTypeList = _UnitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> EngineAndFuelTypeList = Enum.GetValues(typeof(EngineAndFuelType)).Cast<EngineAndFuelType>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });

            IEnumerable<SelectListItem> TransmissionList = Enum.GetValues(typeof(Transmission)).Cast<Transmission>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });


            PostVM postVM = new PostVM
            {
                Post = new Domain.Models.Post(),
                BrandList = BrandList,
                VehicleTypeList = VehicleTypeList,
                EngineAndFuelTypeList = EngineAndFuelTypeList,
                TransmissionList = TransmissionList
            };                
                
            return View(postVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(PostVM PostVM)
        {
            string WebRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if(file.Count>0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(WebRootPath, @"images\Post");
                var extention = Path.GetExtension(file[0].FileName);
                using(var fileStream = new FileStream(Path.Combine(upload, newFileName + extention), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                PostVM.Post.VehicleImage = @"\images\Post\" + newFileName + extention;

            }
            if(ModelState.IsValid)
            {
                await _UnitOfWork.Post.Create(PostVM.Post);
                await _UnitOfWork.SaveAsync();
                TempData["Success"] = CommonMessage.RecordCreated;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            Domain.Models.Post Post = await _UnitOfWork.Post.GetPostById(id);
            Post.CreatedBy = await _userNameService.GetUserName(Post.CreatedBy);
            Post.ModifiedBy = await _userNameService.GetUserName(Post.ModifiedBy);
            if (Post == null)
            {
                return NotFound();
            }
            return View(Post);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Domain.Models.Post Post = await _UnitOfWork.Post.GetPostById(id);
            IEnumerable<SelectListItem> BrandList = _UnitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> VehicleTypeList = _UnitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> EngineAndFuelTypeList = Enum.GetValues(typeof(EngineAndFuelType)).Cast<EngineAndFuelType>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });

            IEnumerable<SelectListItem> TransmissionList = Enum.GetValues(typeof(Transmission)).Cast<Transmission>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });

            PostVM postVM = new PostVM
            {
                Post = Post,
                BrandList = BrandList,
                VehicleTypeList = VehicleTypeList,
                EngineAndFuelTypeList = EngineAndFuelTypeList,
                TransmissionList = TransmissionList
            };

            return View(postVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PostVM PostVM)
        {
            string WebRootPath = _webHostEnvironment.WebRootPath;
            var file = HttpContext.Request.Form.Files;
            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(WebRootPath, @"images\Post");
                var extention = Path.GetExtension(file[0].FileName);

                // Delete old file if it exists
                var objFromDb = await _UnitOfWork.Post.GetByIdAsync(PostVM.Post.Id);

                if(objFromDb.VehicleImage != null)
                {
                    var oldImagePath = Path.Combine(WebRootPath, objFromDb.VehicleImage.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extention), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                PostVM.Post.VehicleImage = @"\images\Post\" + newFileName + extention;
            }
            if (ModelState.IsValid)
            {
                await _UnitOfWork.Post.Update(PostVM.Post);
                await _UnitOfWork.SaveAsync();
                
                TempData["Warning"] = CommonMessage.RecordUpdated;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Domain.Models.Post Post = await _UnitOfWork.Post.GetPostById(id);
            IEnumerable<SelectListItem> BrandList = _UnitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> VehicleTypeList = _UnitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> EngineAndFuelTypeList = Enum.GetValues(typeof(EngineAndFuelType)).Cast<EngineAndFuelType>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });

            IEnumerable<SelectListItem> TransmissionList = Enum.GetValues(typeof(Transmission)).Cast<Transmission>().Select(x => new SelectListItem
            {
                Text = x.ToString().ToUpper(),
                Value = ((int)x).ToString()
            });

            PostVM postVM = new PostVM
            {
                Post = Post,
                BrandList = BrandList,
                VehicleTypeList = VehicleTypeList,
                EngineAndFuelTypeList = EngineAndFuelTypeList,
                TransmissionList = TransmissionList
            };
            return View(postVM);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(PostVM PostVM)
        {
            string WebRootPath = _webHostEnvironment.WebRootPath;
            if(!string.IsNullOrEmpty(PostVM.Post.VehicleImage))
            {
                // Delete old file if it exists
                var objFromDb = await _UnitOfWork.Post.GetByIdAsync(PostVM.Post.Id);

                if (objFromDb.VehicleImage != null)
                {
                    var oldImagePath = Path.Combine(WebRootPath, objFromDb.VehicleImage.Trim('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

            }
            await _UnitOfWork.Post.Delete(PostVM.Post);
            await _UnitOfWork.SaveAsync();
            TempData["Error"] = CommonMessage.RecordDeleted;
            return RedirectToAction(nameof(Index));
        }
    }
}
