using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Saml;
using System.Xml;
using Microsoft.AspNetCore.Http;
using System.Web;

//https://localhost:44365/Account

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {


        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            Console.WriteLine(returnUrl);
            LoginModel model = new LoginModel();
            model.Url = returnUrl;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            
            Console.WriteLine("login");
            
            //User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
            
            if (Check(model))
            {
                await Authenticate(model.Email); // аутентификация

                    
                    
                    

                return Redirect(model.Url);
            }
            
            return View(model);
        }

        private bool Check(LoginModel model)
        {
            Console.WriteLine("-?{0}", model.Url);
            if(model.Url == null || model.Url == "")
            {
                model.Url = "localhost:44365";
            }
            if(model.Email == "mat@mail.ru" && model.Password == "1234")
            {
                /*User user = new User();
                user.Email = model.Email;
                user.Password = model.Password;
                user.Id = 2222;*/
                return true;
            }
            return false;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
               
                if (true)
                {
                    // добавляем пользователя в бд
                    await Authenticate(model.Email);
                }
                
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim

            Console.WriteLine("Authenticate({0})",userName);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "Application1Cookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        [Authorize]
        [HttpGet]
        public IActionResult AuthReqest(AuthRequestModel model)
        {
            
            
            XmlDocument request = new XmlDocument();
            request.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(model.SAMLRequest)));

            XmlDocument doc11 = new XmlDocument();
            doc11.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(model.SAMLRequest)));
            using (XmlTextWriter xmltw = new XmlTextWriter("C:\\Users\\Matvey\\source\\repos\\AuthApp\\AuthApp\\exampleRequest.xml", new UTF8Encoding(false)))
            {
                doc11.WriteTo(xmltw);

                xmltw.Close();
            }

            SamlRequestReader req = new SamlRequestReader(request);
            XmlDocument response = ResponseTool.CreateResponse(req);



            XmlDocument doc = Class2.SignXml(response.OuterXml, Class2.samlCertificate, Class2.privateKey);






            string responseBase64 = Convert.ToBase64String(Class2.StringToByteArray(doc.OuterXml));
            string url = req.GetAssertionConsumerServiceURL();
            

            Response.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.AppendFormat(@"<body onload='document.forms[""form""].submit()'>");
            sb.AppendFormat("<form name='form' action='{0}' method='post'>", url);
            sb.AppendFormat("<input type='hidden' name='SAMLResponse' value='{0}'>", responseBase64);
            // Other params go here
            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");

            Response.WriteAsync(sb.ToString());

            return null;
        }
    }
}
