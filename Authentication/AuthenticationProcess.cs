using Authentication.Common;
using Authentication.Helpers;
using Authentication.Helpers.AppException;
using Authentication.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Authentication
{
    public class AuthenticationProcess : IAuthenticationProcess
    {
        private DataContext _context;

        public AuthenticationProcess(DataContext context)
        {
            this._context = context;
        }
        //public AuthenticationProcess()
        //{
        //}
        public object LoginProcess(User user)
        {
            //call database and get the user
            User existUser = _context.Users.SingleOrDefault(x => x.Username == user.Username && x.Password == user.Password);//UserStorage.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (existUser != null)
            {

                var requestAt = DateTime.Now;
                var expiresIn = requestAt + TokenAuthOption.ExpiresSpan;
                var token = GenerateToken(existUser, expiresIn);

                return JsonConverter(new RequestResult
                {
                    State = RequestState.Success,
                    Data = new
                    {
                        username = user.Username,
                        requertAt = requestAt,
                        expiresIn = TokenAuthOption.ExpiresSpan.TotalSeconds,
                        tokeyType = TokenAuthOption.TokenType,
                        accessToken = token
                    }
                });
            }
            else
            {
                return JsonConverter(new RequestResult
                {
                    State = RequestState.Failed,
                    Msg = "Username or password is invalid"
                });
            }
        }

        public object RegisterProcess(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Username == user.Username))
                throw new AppException("Username " + user.Username + " is already taken");

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }


        private string GenerateToken(User user, DateTime expires)
        {
            var handler = new JwtSecurityTokenHandler();

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(user.Username, "TokenAuth"),
                new[] { new Claim("ID", user.ID.ToString()) }
            );

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = TokenAuthOption.Issuer,
                Audience = TokenAuthOption.Audience,
                SigningCredentials = TokenAuthOption.SigningCredentials,
                Subject = identity,
                Expires = expires
            });
            return handler.WriteToken(securityToken);
        }

        private object JsonConverter(object requestedObject)
        {
            return requestedObject;
        }
    }
}
