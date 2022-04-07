using AutoMapper;
using Expenses.Core;
using Expenses.Core.Moduels;
using ExpensesApp.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Expenses.Core.Consts;
using Microsoft.AspNetCore.Hosting;

namespace ExpensesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _autoMapper;
        private readonly IWebHostEnvironment _environment;

        public ExpensesController(IUnitOfWork unitOfWork, IMapper autoMapper, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _autoMapper = autoMapper;
            _environment = environment;
         }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string usersId = HttpContext.Request.Headers["userId"];
            var listOfData = await _unitOfWork.Expenses.GetAllAsync(e => e.UserId == int.Parse(usersId), new string[] { "Category" });
            try
            {
                var data = listOfData.Select(data => new {
                    Id = data.Id,
                    Amount = data.Amount,
                    VoiceNote = data.VoiceNote,
                    ImageNote = data.ImageNote,
                    ImageNoteFormat = data.ImageNoteFormat,
                    VoiceNoteFormat = data.VoiceNoteFormat,
                    CategoryId = data.CategoryId,
                    Category = data.Category,
                    TextNote = data.TextNote,
                    CreatedAt = data.CreatedAt,
                });

                if (data != null)
                {
                    return Ok(new { success = true, data = data });
                }
                return Ok(new { success = false, message = "can't find any data" });
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
                var data = await _unitOfWork.Expenses.GetByIdAsync(id);
                if (data != null)
                {
                    return Ok(new { success = true, data = data });
                }
                return Ok(new { success = false, message = "can't find any data by this id" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(ExpenseDTO data)
        {
            //if (data.ImageNote.Length > Extensions.AllowedImageMaxSize)
            //{
            //    return Ok(new { success = false, message = "يجب ان يكون حجم الصوره المرفوعه اقل من 2 ميجا" });
            //}
            //if (data.VoiceNote.Length > Extensions.AllowedVoiceMaxSize)
            //{
            //    return Ok(new { success = false, message = "يجب ان يكون حجم التسجيل المرفوعه اقل من 2 ميجا" });
            //}
            //if (!Extensions.AllowedVoiceExtanchans.Contains(Path.GetExtension(data.VoiceNote.FileName).ToLower()))
            //{
            //    return Ok(new { success = false, message = "يجب ان يكون نوع التسجيل .mp3" });
            //}
            //if (!Extensions.AllowedVoiceExtanchans.Contains(Path.GetExtension(data.VoiceNote.FileName).ToLower()))
            //{
            //    return Ok(new { success = false, message = "يجب ان يكون نوع الصوره   .png او .jpg او .jpeg" });
            //}
            //var voiceStream = new MemoryStream();
            //var imageStream = new MemoryStream();
            //await data.VoiceNote.CopyToAsync(voiceStream); 
            //await data.ImageNote.CopyToAsync(imageStream);

            try
            {
                string usersId = HttpContext.Request.Headers["userId"];
                data.UserId = int.Parse(usersId);
                var imagePath = "";
                byte[] imageNote = new byte[0];
                if (!String.IsNullOrEmpty(data.ImageNote))
                {
                    var imageAfterRezize = Helper.Helper.ResizeBase64ImageString(data.ImageNote, 400, 400);
                    imageNote = System.Convert.FromBase64String(imageAfterRezize);
                    imagePath = Helper.Helper.SaveImageToDirectory(imageAfterRezize, data.ImageNoteFormat, _environment.ContentRootPath);
                }

                //var imageNote = System.Convert.FromBase64String(data.ImageNote);
                var voiceNote = System.Convert.FromBase64String(data.VoiceNote);
                //var expenseCreatedAt = DateTime.Parse(data.CreatedAt., null);
                var dataToSave = new Expense
                {
                    Amount = data.Amount,
                    VoiceNote = voiceNote,
                    ImageNote = imageNote,
                    ImageNoteFormat = data.ImageNoteFormat,
                    VoiceNoteFormat = data.VoiceNoteFormat,
                    //VoiceNote = voiceStream.ToArray(),
                    //ImageNote = imageStream.ToArray(),
                    CreatedAt = data.CreatedAt,
                    CategoryId = data.CategoryId,
                    TextNote = data.TextNote,
                    UserId = data.UserId
                };

                var addedData = await _unitOfWork.Expenses.AddAsync(dataToSave);
                await _unitOfWork.Complete();
                if (addedData != null)
                {

                    return Ok(new { success = true, data = addedData, message = "Success Saved" });
                }
                return Ok(new { success = false, message = "Can't save data try again later" });

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return Ok(new { success = false, message = ex.InnerException.Message + data.CreatedAt });

                }
                return Ok(new { success = false, message = ex.Message + data.CreatedAt });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateExpenseDTO data)
        {
            
            try
            {
                var dataById = await _unitOfWork.Expenses.FindAsync(x => x.Id == id);
                var imageNote = System.Convert.FromBase64String(data.ImageNote);
                var voiceNote = System.Convert.FromBase64String(data.VoiceNote);
                if (dataById != null)
                {
                    dataById.CategoryId = data.CategoryId;
                    dataById.Amount = data.Amount;
                    dataById.TextNote = data.TextNote.Trim(); ;
                    dataById.VoiceNote = voiceNote;
                    dataById.ImageNote = imageNote;
                    dataById.ImageNoteFormat = data.ImageNoteFormat;
                    dataById.VoiceNoteFormat = data.VoiceNoteFormat;
                     await _unitOfWork.Complete();
                    return Ok(new { success = true, message = "Success Update", data = dataById });
                }
                return Ok(new { success = false, message = "Can't find expense" });
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
                var data = await _unitOfWork.Expenses.GetByIdAsync(id);
                if (data == null)
                {
                    return Ok(new { success = false, message = "Can't find expense" });
                }
                else
                {
                    var deletedData = await _unitOfWork.Expenses.Delete(data);
                    await _unitOfWork.Complete();
                    return Ok(new { success = true, message ="Success Delete", data = data });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}
