using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiii.DTO;
using WebApiii.Models;

namespace WebApiii.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]
    public class UserController:ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly IConfiguration _configuration;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager , IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")] //KULLANICI OLUŞTURDUM
        public async Task<IActionResult> CreateUser(UserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //user bilgisini oluşturdum
                var user = new AppUser
                {
                    FullName = model.FullName,
                    UserName = model.UserName,
                    Email = model.Email,

                    DateAdded = DateTime.Now

                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return StatusCode(201);

                }
            

            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
            {
                return BadRequest(new { message = "email hatalı"});
        //400 lü hatalar kullanıcı tarafından oluşturululan yanlış api kullanımından olur
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);   

            if (result.Succeeded)
            {
                return Ok(new {token = GenerateJWT(user)}
                );
            }

            return Unauthorized();  //403 yetkimizin olmadığı
        }




        private string GenerateJWT(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Secret").Value ?? "");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Subject, JWT içindeki "claims" bölümünü temsil eder. Bu, kullanıcıyla ilgili bilgiler içerir.
                Subject = new ClaimsIdentity(
                    new Claim[]
                {


                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // Kullanıcının benzersiz kimlik numarası (ID) ekleniyor.
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),  // Kullanıcının adı ekleniyor. Eğer ad bilgisi mevcut değilse boş bir string ekleniyor.

                }),


                Expires = DateTime.UtcNow.AddDays(1),  // Expires, JWT'nin ne kadar süreyle geçerli olacağını belirtir. Bu durumda, token'ın 1 gün (24 saat) süreyle geçerli olacağı belirtilmiştir.

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                // SigningCredentials, token'ın nasıl imzalanacağını belirtir. Bu durumda, simetrik bir anahtar (key) ve HMACSHA256 algoritması kullanılıyor.
                //Issuer = "ozlem.com"

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);  // JWT oluşturuluyor

            var jwt = tokenHandler.WriteToken(token);  // JWT string'e dönüştürülüyor

            return jwt;
        }

    }

}
