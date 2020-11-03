using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// AccountSettings
/// 
/// Replace this class with an interface to your own applications account settings. 
/// 
/// Each account should as a minimum have the following:
///  - A URL pointing to the identity provider for sending Auth Requests
///  - A X.509 certificate for validating the SAML Responses from the identity provider
/// 
/// These should be retrieved from the account store/database on each request in the authentication flow.
/// </summary>
public class AccountSettings
{
    public string certificate = @"-----BEGIN CERTIFICATE-----
MIIDIDCCAgygAwIBAgIQJdd6z34fIoBJQmMnQl4+NTAJBgUrDgMCHQUAMCMxITAfBgNVBAMeGABY
AE0ATABEAFMASQBHAF8AVABlAHMAdDAeFw0wNDEyMzEyMTAwMDBaFw0yNDEyMzEyMTAwMDBaMCMx
ITAfBgNVBAMeGABYAE0ATABEAFMASQBHAF8AVABlAHMAdDCCASIwDQYJKoZIhvcNAQEBBQADggEP
ADCCAQoCggEBAMssC531BzRvpUUbXXRSA2WSxZBGRjsNYjfVGfZNxD0nojxVy3oQRL5V+IbLj7OF
I55p82ygJf81IZf5bMy4+HQ1g6JrBnSDIAyQwwWrAc3m2uJqzgze4gDBYhY1QYQBbsBDNNe78RFz
rj9U8TwYhdfXw9BL5h161tR1u/ihfOduA3pItNGxhXWEFNcgyROnrLVMNrOp6Kp2its30QtIIdma
HD28EOdU5QCDi0X9bbON/vyrDUle5cnuCcS8FphGPCgzd1KIJgTa/p2w676W3GFzVW4uoSFaOddA
gsL3AFbsBTsnwLAvxrdxSXevu8HThk2LNUjI7nL29bpoxVpRlX0CAwEAAaNYMFYwVAYDVR0BBE0w
S4AQUrRdfGlM740goFyCqwKwd6ElMCMxITAfBgNVBAMeGABYAE0ATABEAFMASQBHAF8AVABlAHMA
dIIQJdd6z34fIoBJQmMnQl4+NTAJBgUrDgMCHQUAA4IBAQBct5fdq3PvNo1OhLWe7yvz+6LAdaM/
uaji7B6MrL65APtAJzGGLwRCCrILrdMfqCTf0XFXq9Vf3Fjm2meRsMPEyQ/VauRMUsWc72hV5ZY0
cHiG8PyGG3mv3omo/XR5h24H5VaklrFYBmwVouNhQvqTayad/LeK6wBiJp3MBm3JvtpGyveWoaRC
4XMKpGUKXu2yGm8/NVr6SpEwu3oyAcyMPhdobuU5r3cN2jPJ/h/zOwYsZzn9qTuJNnFenqIj90hS
oNVlZT4m67z8suA10VnnUI7g6jBLoXj+EVHF63kQaRLavbxROgzopOZu96h1/VYzf+a15Z7qgbKA
i5pXyDNc
-----END CERTIFICATE-----";
    
    public string privateKey = @"-----BEGIN RSA PRIVATE KEY-----
MIIEpQIBAAKCAQEAyywLnfUHNG+lRRtddFIDZZLFkEZGOw1iN9UZ9k3EPSeiPFXL
ehBEvlX4hsuPs4UjnmnzbKAl/zUhl/lszLj4dDWDomsGdIMgDJDDBasBzeba4mrO
DN7iAMFiFjVBhAFuwEM017vxEXOuP1TxPBiF19fD0EvmHXrW1HW7+KF8524Deki0
0bGFdYQU1yDJE6estUw2s6noqnaK2zfRC0gh2ZocPbwQ51TlAIOLRf1ts43+/KsN
SV7lye4JxLwWmEY8KDN3UogmBNr+nbDrvpbcYXNVbi6hIVo510CCwvcAVuwFOyfA
sC/Gt3FJd6+7wdOGTYs1SMjucvb1umjFWlGVfQIDAQABAoIBADygNAc6ap/3ALYS
aFyhbGoO1e0rSyGr6LcIW+rnYbtt7Ddc0o7l891oAfUXIRZMkEhhDUZIs43n6NJU
l2ave1QR8+mvTgnOZu3Y9JjoYm1yibYucLXefEoFaqN92MLvOoEcjNQjPNgcUM6N
Jj7sgmPZ+pBZVZ1OXnSffSu/5GmaHjKqmpkpNTpsEekbVjM/9jvrmuW+fg/f1HaA
ro61bO1ZcZXwTP+a+XrHBvitfyw1iZQPLoBgIUNzLnrntZopQzt7HpzykO3RS4u6
4rXo4f+jKlfbyAH5mDB7FQW0p08PlhO+YKLBPaXU7lwnpzGWwvz+MEPSY8G1s1iK
l6Z4AfECgYEA24ZyYF4Gv6ifYgAGRl0FF5cxMypSRr1NJ5wIbEnfXIXas9GVTg3+
nKZXs/wvcY7XRLjI8wIAm4V9g7b+Ck9kzv0PT+rVP084qmZg5TOqcKdmu6ZGn7Py
aMO9UIBq/bSW8dyT3a++MTtZU8QpsGdfqHzUSIefXim6mUhPKczye5cCgYEA7O4D
tinJRMi9XrsSxSCkB3qNs/rzXGkULS2g9C8pQoqyfQJNXb0CYj+rUhUEZDe2SQli
aNQIIebB3p+l2ZLHYlHb0GC4HOZNU63UUEIyiQr8f9/O35QHLf4NrOHqkEc8c05k
SMeNsoF0AqxFcAexwFakbKZRar2ruONweXNMqgsCgYEAuZOcmQ6jkd4Qbp4qr8zv
AxRTCTfbueVJlhR3omOIqQSW77BbEVMPTInqVkL4MH1aScQUTCoDLXXZt0E43Kpl
Q/31tc+FWjG0a4iEnP3iNb2uQS+9QEC0yg++uJD24WaKvAeGEMACfkf3qbKIs5GP
8jUkl/Peq5GHJxFTqriQvB0CgYEAwyNRqUIHRAC1f4VCc1tr3cEBXsAMmgrtlDwl
eZgyOlzznuQ7hj367aKU3vjycfw0xTjWdZJU1F8zQ8Fnnqg2UXMsQRa37Q19mLLt
z+CFsLt8tXFG+Hv54daBuucjAwu47Rsem5bHzMK0ItNyKVAdBVYW/GmLWwe2nIOu
ikj9VnsCgYEAmvN7Zm8E4su48vLeFR7Unlgrp3+XHlepioWO7ktTtc4N7PVVYdwH
opUUsUJXFNCWdUsQBkEX+LkKcFTw1O3ehoQULIUJu4N0nDZN2MOiOcyIm2cN8KnO
s5qIZp6OswuOyfBsdNee1F+FyhueUWcNYFYDm3R4aAyt4zlT7VRpieU=
-----END RSA PRIVATE KEY-----";

    public string idp_sso_target_url = "https://localhost:44365/Account/AuthReqest";
    //public string idp_sso_target_url = "https://capriza.github.io/samling/samling.html";
}
