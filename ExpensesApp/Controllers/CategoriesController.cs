using AutoMapper;
using Expenses.Core;
using Expenses.Core.Moduels;
using ExpensesApp.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ExpensesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _autoMapper;
        private IConfiguration _configuration;
        public CategoriesController(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper autoMapper)
        {
            _unitOfWork = unitOfWork;
            _autoMapper = autoMapper;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
 
            string usersId = HttpContext.Request.Headers["userId"];
            var listOfData = await _unitOfWork.Categories.GetAllAsync(c=>c.UserId == int.Parse(usersId));
             try
            {
                if (listOfData != null)
                {
                    return Ok(new { success = true, data = listOfData });
                }
                return Ok(new { success = false, message = "تعزر جلب الداتا الخاصه بك من فضلك حاول لاحقا" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var data = await _unitOfWork.Categories.GetByIdAsync(id);
                if (data != null)
                {
                    return Ok(new { success = true, data = data });
                }
                return Ok(new { success = false, message = "لا يوجد داتا بهذا الكود" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(CategoryDTO data)
        {
            try
            {
                string usersId = HttpContext.Request.Headers["userId"];
                data.UserId = int.Parse(usersId);
                data.Name = data.Name.Trim();
                var dataAfterMaped = _autoMapper.Map<Category>(data);
                 var addedData = await _unitOfWork.Categories.AddAsync(dataAfterMaped);
                await _unitOfWork.Complete();
                if (addedData != null)
                {
                    return Ok(new { success = true, data = addedData, message = "تم الحفظ بنجاح" });
                }
                return Ok(new { success = false, message = "لم يتمكن من الحفظ الرجاء المحاوله مره اخرى لاحقا" });

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return Ok(new { success = false, message = ex.InnerException.Message });

                }
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryDTO data)
        {
            try
            {
                var dataAfterMaped = _autoMapper.Map<Category>(data);
                dataAfterMaped.Id = id;
                var dataById = await _unitOfWork.Categories.FindAsync(x => x.Id == id);
                if (dataById != null)
                {
                    dataById.Name = dataAfterMaped.Name.Trim(); ;
                      await _unitOfWork.Complete();
                    return Ok(new { success = true, message = "تم التعديل بنجاح", data = dataById });
                }
                return Ok(new { success = false, message = "لم نتمكن من العصور على المصروف لحفظ التعديل عليها" });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return Ok(new { success = false, message = ex.InnerException.Message });

                }
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var data = await _unitOfWork.Categories.GetByIdAsync(id);
                if (data == null)
                {
                    return Ok(new { success = false, message = "هذا المصروف غير موجود" });
                }
                else
                {
                    var deletedData = await _unitOfWork.Categories.Delete(data);
                    await _unitOfWork.Complete();
                    return Ok(new { success = true, message = "تم الحذف بنجاح", data = data });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}