using System;
using System.IO;
using System.Net;

namespace VRCAccountGen.VRChat
{
    static class VRCAPI
    {
        public static bool EmailExists(string Email)
        {
            string URL = $"https://vrchat.com/api/1/auth/exists?email={Uri.EscapeUriString(Email)}&apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26";
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(URL);

            Request.Headers.Add("cookie", "apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26;");
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.79 Safari/537.36";
            Request.Method = "GET";
            Request.Proxy = Proxies.GetProxie().Proxy;

            try
            {
                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                if (Response.StatusCode == HttpStatusCode.OK || Response.StatusCode == HttpStatusCode.Accepted || Response.StatusCode == HttpStatusCode.Created || Response.StatusCode == HttpStatusCode.NotModified)
                {
                    StreamReader webSource = new StreamReader(Response.GetResponseStream());
                    string response = webSource.ReadToEnd();
                    return response == "{\"userExists\":true}";
                }
            }
            catch
            {
                Console.WriteLine($"Failed to Check Email");
            }
            return false;
        }

        public static string VerifyEmail(string VerifyEmailURL)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(VerifyEmailURL);

            Request.Headers.Add("cookie", "apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26;");
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.79 Safari/537.36";
            Request.Method = "GET";
            Request.Proxy = Proxies.GetProxie().Proxy;

            try
            {
                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                if (Response.StatusCode == HttpStatusCode.OK || Response.StatusCode == HttpStatusCode.Accepted || Response.StatusCode == HttpStatusCode.Created || Response.StatusCode == HttpStatusCode.NotModified)
                {
                    StreamReader webSource = new StreamReader(Response.GetResponseStream());
                    string response = webSource.ReadToEnd();
                    return response;
                }
            }
            catch
            {
                Logger.LogError("Failed to Verify Email");
            }
            return "Unknown";
        }
    }
}
