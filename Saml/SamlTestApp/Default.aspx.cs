using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


using OneLogin.Saml;
using System.Xml;
using System.Text;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccountSettings accountSettings = new AccountSettings();
        AppSettings settings = new AppSettings();
        AuthRequest req = new AuthRequest(settings.issuer,settings.assertionConsumerServiceUrl);

        XmlDocument doc11 = new XmlDocument();

        /*doc11.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(req.GetRequest(AuthRequest.AuthRequestFormat.Base64))));
        using (XmlTextWriter xmltw = new XmlTextWriter("C:\\Users\\Matvey\\source\\repos\\SamlTestApp\\SamlTestApp\\exampleRequest.xml", new UTF8Encoding(false)))
        {
            doc11.WriteTo(xmltw);

            xmltw.Close();
        }*/

        Response.Redirect(req.GetRedirectUrl(accountSettings.idp_sso_target_url));
    }

    



}
