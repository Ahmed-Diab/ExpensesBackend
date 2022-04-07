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

            [HttpPost("signup")]
            public async Task<IActionResult> SingUp(SignUpDTO signUpDto)
            {
                var userToCreate = _mapper.Map<User>(signUpDto);
            if (signUpDto.Password.Length < 4 || signUpDto.UserName.Length < 4) return Ok(new { Success = false, Message = "password and username most be atlest 4 chart" });
 
            var result = await _userManager.CreateAsync(userToCreate, signUpDto.Password);
                if (result.Succeeded)
                {
                    return Ok(new {  success = true, message = "Success Signup" });
                }
                if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                {
                     return Ok(new { Success = false, Message = "username is already taken" });
                } else if (result.Errors.Any(x => x.Code == "PasswordRequiresLower")) {
                return Ok(new { Success = false, Message = "Requierd Password" });
                }
                else return Ok(new { Success = false, Message = result.Errors });
            }

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
                return Ok(new { success = false, message = "Can't find user" });
             }

            private async Task<string> GenerateToken(User user)
            {
                var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
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
                return Ok(new { Success = false, Message = "Can't get the main Dashboard" });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.Message });
            }
 
        }             
    }


    }