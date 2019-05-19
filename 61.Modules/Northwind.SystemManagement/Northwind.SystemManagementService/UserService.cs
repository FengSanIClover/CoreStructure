using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Northwind.Entities;
using Northwind.Entities.BusinessModels.PublicCloud;
using Northwind.SystemManagementInterface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Northwind.SystemManagementService
{
    /*
 一個典型的JWT 字符串由三部分组成:
header: 頭部,meta信息和算法說明
payload: 負荷(Claims ), 可在其中放入自定議内容, 比如, 用户身份等
signature: 簽名, 數字簽名, 用來保正前兩者的有效性
     */
    public class UserService : IUserService
    {   // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<UserBM> _users = new List<UserBM>()
        {
            new UserBM{ Id = 1,FirstName = "Test",LastName = "User",UserName = "test",Password = "test"}
        };
        private readonly AppSettings _appSettings;
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public UserBM Authenticate(string username, string password)
        {   // using System.Linq;記得引用
            var user = _users.SingleOrDefault(x => x.UserName == username && x.Password == password);
            // return null if user not found
            if (user == null)
            {
                return null;
            }
            // 安裝System.IdentityModel.Tokens.Jwt
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            // 安裝Microsoft.IdentityModel.Tokens
            var tokenDescriptor = new SecurityTokenDescriptor()
            {  // 安裝System.Security.Claims
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;
            return user;
        }

        public IEnumerable<UserBM> GetAll()
        {
            // return users without passwords
            return _users.Select(x =>
            {
                x.Password = null;
                return x;
            });
        }
    }
}
