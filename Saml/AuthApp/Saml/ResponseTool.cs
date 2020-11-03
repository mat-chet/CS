using Saml;
using System;
using System.Xml;

namespace AuthApp.Controllers
{
    public class ResponseTool
    {
        public static XmlDocument CreateResponse(SamlRequestReader req)
        {
            //SamlRequestReader req = new SamlRequestReader(request);
            XmlDocument response = new SamlResponseXMLBuilder()
                .SetInResponseTo(req.GetAuthnRequestID())
                .SetDestination(req.GetAssertionConsumerServiceURL())
                .SetIssuer(req.GetIssuer())
                .SetNameID(req.GetNameID())
                .SetAudience(req.GetIssuer())
                .SetNotOnAfter(DateTime.Now.AddDays(365).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .SetIdResponse("_" + System.Guid.NewGuid().ToString())
                .SetIdAssertion("_" + System.Guid.NewGuid().ToString())
                .SetNotBefore(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .SetSessionIndex("_samlong_")
                .SetIssueInstantAssertion(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .SetIssueInstantResponse(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .SetSessionNotOnOrAfter(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                .Build();


            return response;
        }
    }
}