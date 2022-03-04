using AutoMapper;
using Expenses.Core.Moduels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Expenses.EF;
using Microsoft.AspNetCore.Authorization;
using ExpensesApp.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ExpensesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
            private readonly UserManager<User> _userManager;
            private readonly SignInManager<User> _signInManager;
            private readonly ApplicationDbContext _context;
            private readonly IConfiguration _config;
            private readonly IMapper _mapper;
            public UsersController(
                    IConfiguration config,
                    IMapper mapper,
                    UserManager<User> userManager,
                    SignInManager<User> signInManager,
                    ApplicationDbContext context

                    )
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _context = context;
                _config = config;
                _mapper = mapper;
            }


            //  [Authorize(Roles="Admin")]
            [HttpPost("signup")]
            public async Task<IActionResult> SingUp(SignUpDTO signUpDto)
            {
                var userToCreate = _mapper.Map<User>(signUpDto);
                var result = await _userManager.CreateAsync(userToCreate, signUpDto.Password);
                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "تم انشاء الحساب بنجاح"
                    });
                }
                if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                {
                return Ok(new { Success = false, Message = "أسم المستخدم موجود بالفعل" });
            } else if (result.Errors.Any(x => x.Code == "PasswordRequiresLower")) {
                return Ok(new { Success = false, Message = "كلمه المرور مطلوبه" });
            }
                else return Ok(new { Success = false, Message = result.Errors });
            }

            [AllowAnonymous]
            [HttpPost("signin")]
            public async Task<IActionResult> SingIn([FromBody] SignInDTO signInDto)
            {
                var user = await _userManager.FindByNameAsync(signInDto.UserName);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, signInDto.Password, false);
                    if (result.Succeeded)
                    {
                        var appUser = await _userManager.Users.FirstOrDefaultAsync(
                            u => u.NormalizedUserName == signInDto.UserName.ToUpper()
                        );
                        var userToReturn = _mapper.Map<UserDTO>(appUser);
                        try
                    {
                        var to = GenerateToken(appUser).Result;

                        return Ok(new
                            {
                                success = true,
                                data = new
                                {
                                    token = to,
                                    userId = appUser.Id,
                                    userName = appUser.UserName
                                }

                            });
                        }
                        catch (Exception ex)
                        {
                            return Ok(new
                            {
                                Success = false,
                                Message = ex.Message
                            });
                        }
                    }
                }
                return Ok(new { success = false, message = "المستخدم غير موجود" });
             }

            private async Task<string> GenerateToken(User user)
            {
                var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
                //var roles = await _userManager.GetRolesAsync(user);
                //foreach (var role in roles)
                //{
                //    claims.Add(new Claim (ClaimTypes.Role, role));
                //}
                DateTime expires = DateTime.UtcNow.AddDays(1);
                var sec = _config.GetSection("AppSettings:SecretKey").Value;
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(sec));
                var credintional = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
                var tokenDes = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expires,
                    SigningCredentials = credintional
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDes);
                return tokenHandler.WriteToken(token);
            }

        [HttpGet("GetDashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            string usersId = HttpContext.Request.Headers["userId"];
            try
            {
                var data = _context.Categories.Where(c => c.UserId == int.Parse(usersId)).Select(x => new
                {
                    Name = x.Name,
                    Value = _context.Expenses.Where(e => e.CategoryId == x.Id).Sum(e => e.Amount)
                }).ToList();

                if (data != null)
                {
                    return Ok(new
                    {
                        success = true,
                        data = data
                    });

                }
                return Ok(new { Success = false, Message = "حدث خطأ أثناء جلب القئمه الرئيسيه " });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.Message });
            }
           

        }


        [HttpGet("all")]
            public async Task<IActionResult> GetAllUsers()
            {
                var userList = await (from user in _context.Users
                                      orderby user.UserName
                                      select new
                                      {
                                          Id = user.Id,
                                          UserName = user.UserName
                                      }).ToListAsync();

                if (userList != null)
                {
                    return Ok(new
                    {
                        success = true,
                        users = userList
                    });

                }
                return Ok( new {success = false , message = "حدث خطأ أثناء جلب المستخدمين " });

            }

            //[HttpPost("edit/roles/{userName}")]
            //public async Task<IActionResult> EditRoles(string userName, EditUserRolesDTO roleEditDto)
            //{
            //    var user = await _userManager.FindByNameAsync(userName);
            //    var userRoles = await _userManager.GetRolesAsync(user);
            //    var selectedRoles = roleEditDto.RoleNames;
            //    selectedRoles = selectedRoles ?? new string[] { };
            //    var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            //    if (!result.Succeeded)
            //        return Ok("حدث خطأ أثناء إضافة  الصلاحيه");
            //    result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            //    if (!result.Succeeded)
            //        return Ok("حدث خطأ أثناء حذف الصلاحيه");
            //    return Ok(new
            //    {
            //        success = true,
            //        message = $"{userName} roles is updatea",
            //        roles = await _userManager.GetRolesAsync(user)
            //    });
            //}


            //[HttpDelete("delete/{id}")]
            //public async Task<IActionResult> DeleteRole(int id)
            //{
            //    var user = await _userManager.FindByIdAsync(id.ToString());
            //    if (user != null)
            //    {
            //        if (user.UserName == "Admin")
            //        {
            //            return Ok("cant remove this user");

            //        }
            //        var result = await _userManager.DeleteAsync(user);
            //        if (result.Succeeded)
            //        {
            //            return Ok(new
            //            {
            //                success = true,
            //                message = "Delete user is done"
            //            });
            //        }
            //    }
            //    return Ok("cant find thi role");


            //}

            //[HttpPost("change/password")]
            //public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePassword, int id)
            //{
            //    var user = await _userManager.GetUserAsync(User);
            //    if (user != null)
            //    {
            //        var result = await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
            //        if (result.Succeeded)
            //        {
            //            return Ok(new
            //            {
            //                success = true,
            //                message = "password changed"
            //            });
            //        }
            //        return Ok(new
            //        {
            //            success = false,
            //            errorMessage = "password no't match"
            //        });
            //    }
            //    return Ok("this user no't found");
            //}

            //[HttpPost("change/username")]
            //public async Task<IActionResult> ChangePassword(ChangeUserNameDTO changeUser)
            //{
            //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == changeUser.Id);
            //    if (user != null && user.UserName != "Admin")
            //    {
            //        user.UserName = changeUser.UserName;
            //        user.NormalizedUserName = changeUser.UserName.ToUpper();
            //        await _context.SaveChangesAsync();
            //        return Ok(new
            //        {
            //            success = true,
            //            UserName = user.UserName,
            //            Id = user.Id
            //        });
            //    }
            //    return Ok("can't change this user name");
            //}
        
    }


    }