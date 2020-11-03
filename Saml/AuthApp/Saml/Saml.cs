/*	Jitbit's simple SAML 2.0 component for ASP.NET
	https://github.com/jitbit/AspNetSaml/
	(c) Jitbit LP, 2016
	Use this freely under the MIT license (see http://choosealicense.com/licenses/mit/)
	version 1.2
*/

using System;
using System.Web;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.IO.Compression;
using System.Text;
using System.Security.Cryptography;

namespace Saml
{
	/// <summary>
	/// this class adds support of SHA256 signing to .NET 4.0 and earlier
	/// (you can use it in .NET 4.5 too, if you don't want a "System.Deployment" dependency)
	/// </summary>
	public sealed class RSAPKCS1SHA256SignatureDescription : SignatureDescription
	{
		public RSAPKCS1SHA256SignatureDescription()
		{
			KeyAlgorithm = typeof(RSACryptoServiceProvider).FullName;
			DigestAlgorithm = typeof(SHA256Managed).FullName;   // Note - SHA256CryptoServiceProvider is not registered with CryptoConfig
			FormatterAlgorithm = typeof(RSAPKCS1SignatureFormatter).FullName;
			DeformatterAlgorithm = typeof(RSAPKCS1SignatureDeformatter).FullName;
		}

		public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(key);
			deformatter.SetHashAlgorithm("SHA256");
			return deformatter;
		}

		public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(key);
			formatter.SetHashAlgorithm("SHA256");
			return formatter;
		}

		private static bool _initialized = false;
		public static void Init()
		{
			if (!_initialized)
				CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
			_initialized = true;
		}
	}

	public class Certificate
	{
		public X509Certificate2 cert;

		public void LoadCertificate(string certificate)
		{
			LoadCertificate(StringToByteArray(certificate));
		}

		public void LoadCertificate(byte[] certificate)
		{
			cert = new X509Certificate2(certificate);
			//cert.Import(certificate);
		}

		private byte[] StringToByteArray(string st)
		{
			byte[] bytes = new byte[st.Length];
			for (int i = 0; i < st.Length; i++)
			{
				bytes[i] = (byte)st[i];
			}
			return bytes;
		}
	}

	public class Response
	{
		private XmlDocument _xmlDoc;
		private Certificate _certificate;
		private XmlNamespaceManager _xmlNameSpaceManager; //we need this one to run our XPath queries on the SAML XML

		public string Xml { get { return _xmlDoc.OuterXml; } }

		public Response(string certificateStr)
		{
			RSAPKCS1SHA256SignatureDescription.Init(); //init the SHA256 crypto provider (for needed for .NET 4.0 and lower)

			_certificate = new Certificate();
			_certificate.LoadCertificate(certificateStr);
		}

		public void LoadXml(string xml) 
		{
			_xmlDoc = new XmlDocument();
			_xmlDoc.PreserveWhitespace = true;
			_xmlDoc.XmlResolver = null;
			_xmlDoc.LoadXml(xml);

			_xmlNameSpaceManager = GetNamespaceManager(); //lets construct a "manager" for XPath queries
		}

		public void LoadXmlFromBase64(string response)
		{
			System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
			LoadXml(enc.GetString(Convert.FromBase64String(response)));
		}

		public bool IsValid()
		{
			XmlNodeList nodeList = _xmlDoc.SelectNodes("//ds:Signature", _xmlNameSpaceManager);

			SignedXml signedXml = new SignedXml(_xmlDoc);

			if (nodeList.Count == 0) return false;

			signedXml.LoadXml((XmlElement)nodeList[0]);
			return ValidateSignatureReference(signedXml) && signedXml.CheckSignature(_certificate.cert, true) && !IsExpired();
		}

		//an XML signature can "cover" not the whole document, but only a part of it
		//.NET's built in "CheckSignature" does not cover this case, it will validate to true.
		//We should check the signature reference, so it "references" the id of the root document element! If not - it's a hack
		private bool ValidateSignatureReference(SignedXml signedXml)
		{
			if (signedXml.SignedInfo.References.Count != 1) //no ref at all
				return false;

			var reference = (Reference)signedXml.SignedInfo.References[0];
			if (reference.Uri == "") return true; // VVK idElement == _xmlDoc.DocumentElement root
			var id = reference.Uri.Substring(1);

			var idElement = signedXml.GetIdElement(_xmlDoc, id);

			if (idElement == _xmlDoc.DocumentElement)
				return true;
			else //sometimes its not the "root" doc-element that is being signed, but the "assertion" element
			{
				var assertionNode = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion", _xmlNameSpaceManager) as XmlElement;
				if (assertionNode != idElement)
					return false;
			}

			return true;
		}

		private bool IsExpired()
		{
			DateTime expirationDate = DateTime.MaxValue;
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Subject/saml:SubjectConfirmation/saml:SubjectConfirmationData", _xmlNameSpaceManager);
			if (node != null && node.Attributes["NotOnOrAfter"] != null)
			{
				DateTime.TryParse(node.Attributes["NotOnOrAfter"].Value, out expirationDate);
			}
			return DateTime.UtcNow > expirationDate.ToUniversalTime();
		}

		public string GetNameID()
		{
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Subject/saml:NameID", _xmlNameSpaceManager);

			if (node == null)
				node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='UserID']/saml:AttributeValue", _xmlNameSpaceManager);

			return node.InnerText;
		}

		public string GetEmail()
		{
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='User.email']/saml:AttributeValue", _xmlNameSpaceManager);

			if (node == null)
				node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='Email']/saml:AttributeValue", _xmlNameSpaceManager);

			//some providers (for example Azure AD) put email into an attribute named "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
			if (node == null)
				node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']/saml:AttributeValue", _xmlNameSpaceManager);

			return node == null ? null : node.InnerText;
		}

		public string GetFirstName()
		{
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='first_name']/saml:AttributeValue", _xmlNameSpaceManager);

			if (node == null)
				node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='FirstName']/saml:AttributeValue", _xmlNameSpaceManager);

			//some providers (for example Azure AD) put email into an attribute named "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
			if (node == null)
				node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname']/saml:AttributeValue", _xmlNameSpaceManager);

			return node == null ? null : node.InnerText;
		}

		public string GetLastName()
		{
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='last_name']/saml:AttributeValue", _xmlNameSpaceManager);

			if (node == null)
				node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='LastName']/saml:AttributeValue", _xmlNameSpaceManager);

			//some providers (for example Azure AD) put email into an attribute named "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"
			if (node == null)
				node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname']/saml:AttributeValue", _xmlNameSpaceManager);
			return node == null ? null : node.InnerText;
		}

		public string GetDepartment()
		{
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/department']/saml:AttributeValue", _xmlNameSpaceManager);
			return node == null ? null : node.InnerText;
		}

		public string GetPhone()
		{
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/homephone']/saml:AttributeValue", _xmlNameSpaceManager);
			if (node == null)
				node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/telephonenumber']/saml:AttributeValue", _xmlNameSpaceManager);
			return node == null ? null : node.InnerText;
		}

		public string GetCompany()
		{
			XmlNode node = _xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/companyname']/saml:AttributeValue", _xmlNameSpaceManager);
			return node == null ? null : node.InnerText;
		}

		//returns namespace manager, we need one b/c MS says so... Otherwise XPath doesnt work in an XML doc with namespaces
		//see https://stackoverflow.com/questions/7178111/why-is-xmlnamespacemanager-necessary
		private XmlNamespaceManager GetNamespaceManager()
		{
			XmlNamespaceManager manager = new XmlNamespaceManager(_xmlDoc.NameTable);
			manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
			manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
			manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

			return manager;
		}
	}

	
}
