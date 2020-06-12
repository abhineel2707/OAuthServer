using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace OAuthServer.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type, // authorization flow type; like enum
            string client_id, // client identifier
            string redirect_uri, // redirection url
            string scope, // what information I am requesting like email, tel, grandma cookie
            string state // random string generated to confirm that we are going back to the same client
        )
        {
            // ?a=foo&b=bar
            var query = new QueryBuilder();
            query.Add("redirect_uri", redirect_uri);
            query.Add("state", state);
            return View(model: query.ToString());
        }
        [HttpPost]
        public IActionResult Authorize(
            string username,
            string redirect_uri,
            string state)
        {
            const string code = "BABABABABA";

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);
            return Redirect($"{redirect_uri}{query.ToString()}");
        }
        public async Task<IActionResult> Token(
            string grant_type, // flow of access_token request
            string code, // confirmation of the authentication process
            string redirect_uri,
            string client_id
        )
        {
            var claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub,"some_id"),
                new Claim("Granny","cookie")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);
            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials
            );

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            };

            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);
            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            // some mechanism for validating the code
            return Redirect(redirect_uri);
        }
    }
}