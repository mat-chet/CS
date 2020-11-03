using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Text;

//https://localhost:44318/Consume
namespace SamlTestApp
{
    public partial class Consume : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            AccountSettings accountSettings = new AccountSettings();

            OneLogin.Saml.Response samlResponse = new OneLogin.Saml.Response(accountSettings.certificate);
            string xml = Request.Form["SAMLResponse"];
            samlResponse.LoadXmlFromBase64(xml);

            
            if (samlResponse.IsValid())
            {
                Response.Write("OK!\n");
                Response.Write(samlResponse.GetNameID());
                
            }
            else
            {
                Response.Write("Failed");
                //Response.Write(xml);
            }
        }
    }
}
