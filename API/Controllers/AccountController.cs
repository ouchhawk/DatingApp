using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        // DataContext içinde user listesi var
        // token service'i 
        public AccountController(DataContext context, ITokenService tokenService){
            _tokenService = tokenService;
            _context = context;
        }


        // hashlenmiş şifreler bilinen şifre haslerini barındıran dictionary attack ile kırılabilir. 
        // bunu salt kullanarak çözebiliriz ama bu da yeterli değil 
        // property leri doğrudan göndermek güvenli olmayabilir. dto ile encapsulate ederek göndermek daha güvenli
        // json web tokenlarını da dto ile gönderebilirsin
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register( RegisterDto registerDto)
        {
            // async fonksiyonlarda await task tamamlanana kadar bekler ve sonuç döndürür 
            if(await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            //  IDisposable object oluşturduğunda "using" kullanmak zorundasın. 
            //  HMACSHA512(), IDisposable'ın torununun torunu
            using var hmac = new HMACSHA512();

            var user = new AppUser{
                Username = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.Username,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login( LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x=>x.Username == loginDto.Username);

            if(user==null) return Unauthorized("--Invalid username --");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0; i< computedHash.Length; i++){
                if(computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("--Invalid password --");
                }
            }

            return new UserDto
            {
                Username = user.Username,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username.ToLower());
        }
    }
}