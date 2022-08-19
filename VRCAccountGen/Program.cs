using Newtonsoft.Json;
using System;
using System.Net.Http;
using _2CaptchaAPI;
using System.Text;
using System.Net;
using VRCAccountGen.VRChat;
using VRCAccountGen.VRChat.Objects;

namespace VRCAccountGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Log($"1 - to use Proxie.txt");
            Logger.Log($"2 - to use free Proxies");
            string res = Console.ReadLine();
            switch (res)
            {
                case "1":
                    Proxies.ReadFile();
                    break;

                case "2":
                    Proxies.GetNewList();
                    break;
            }

            for (; ; )
            {
                Logger.Log($"1 - to generate random account");
                Logger.Log($"2 - to generate custom account");
                Logger.Log($"3 - to generate invalid account");
                Logger.Log($"4 - to verify email");

                var crl = Console.ReadLine();

                switch (crl.Split(" ")[0])
                {
                    case "1":
                        string RandUser = Wrappers.RandomString(13);
                        string RandPass = Wrappers.RandomString(18);
                        Logger.Log("Enter Email");
                        string RandEmailAddress = Console.ReadLine();
                        if (RandEmailAddress == "")
                        {
                            Logger.LogError("Email is Empty");
                            return;
                        }
                        //if (VRCAPI.EmailExists(RandEmailAddress)) Logger.LogError("Email already exist");
                        //else RegisterAccount(RandEmailAddress, RandUser, RandPass);
                        RegisterAccount(RandEmailAddress, RandUser, RandPass);
                        break;

                    case "2":
                        Logger.Log("Enter Username (max. 12)");
                        string User = Console.ReadLine();
                        if (User.Length > 12)
                        {
                            Logger.LogError("Username is too long (max 11)");
                            return;
                        }
                        string Pass = Wrappers.RandomString(18);
                        Logger.Log("Enter Email");
                        string EmailAddress = Console.ReadLine();
                        if (EmailAddress == "" || User == "")
                        {
                            Logger.LogError("Username or Email is Empty");
                            return;
                        }
                        //if (VRCAPI.EmailExists(EmailAddress)) Logger.LogError("Email already exist");
                        //else RegisterAccount(EmailAddress, $"{User}-{Wrappers.RandomString(2)}", Pass);
                        RegisterAccount(EmailAddress, $"{User}-{Wrappers.RandomString(2)}", Pass);
                        break;

                    case "3":
                        string NumberName = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                        string NumberPass = Wrappers.RandomNumberString(10);
                        Logger.Log("Enter Email");
                        string mailAddress = Console.ReadLine();
                        if (mailAddress == "")
                        {
                            Logger.LogError("Email is Empty");
                            return;
                        }
                        //if (VRCAPI.EmailExists(mailAddress)) Logger.LogError("Email already exist");
                        //else RegisterAccount(mailAddress, NumberName, NumberPass);
                        RegisterAccount(mailAddress, NumberName, NumberPass);
                        break;

                    case "4":
                        Logger.Log("Enter Verify Link");
                        string VerifyLink = Console.ReadLine();
                        VRCAPI.VerifyEmail(VerifyLink);
                        break;
                }
            }
        }

        private static async void RegisterAccount(string email, string username, string password)
        {
            try
            {
                Logger.Log("Sending Captcha to Provider");
                _2Captcha captcha = new _2Captcha("YOUR API KEY HERE");
                _2Captcha.Result _2CapResult = await captcha.SolveHCaptcha("67ab3710-6883-4d15-8614-db0861382a92", "https://vrchat.com/home/register");
                if (_2CapResult.Success == false)
                {
                    Logger.LogError("Failed to solve Captcha, retrying");
                    RegisterAccount(email, username, password);
                    return;
                }
                Logger.Log("Captcha Solved");
                register2(_2CapResult.Response, email, username, password);

            } catch (Exception)
            {
                Logger.LogError($"Captcha Service had an Error, retrying");
                RegisterAccount(email, username, password);
            }
        }

        private static async void register2(string captcha, string email, string username, string password)
        {
            try
            {
                Proxies.ProxyWithCredentials proxy = Proxies.GetProxie();
                HttpClientHandler httpClientHandler = new HttpClientHandler()
                {
                    Proxy = proxy.Proxy,
                };

                HttpClient request = new HttpClient(httpClientHandler);
                request.DefaultRequestHeaders.Add("Origin", "https://vrchat.com");
                request.DefaultRequestHeaders.Add("Referer", "https://vrchat.com/home/register");
                request.DefaultRequestHeaders.Add("Host", "vrchat.com");
                request.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"94\", \"Microsoft Edge\";v=\"94\", \";Not A Brand\";v=\"99\"");
                request.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                request.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                request.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.79 Safari/537.36");
                request.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
                request.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                request.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                request.DefaultRequestHeaders.Add("Cookie", "apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26;");
                RegisterObject payload = GetAccount(captcha, email, username, password);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://vrchat.com/api/1/auth/register?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26");
                string AccPayload = JsonConvert.SerializeObject(payload);
                Logger.Log(AccPayload);

                requestMessage.Content = new StringContent(AccPayload, Encoding.UTF8, "application/json");
                var resp = await request.SendAsync(requestMessage);
                string bodyresp = await resp.Content.ReadAsStringAsync();

                if (bodyresp.Contains("Username or email already exists") || bodyresp.Contains("This email address is not allowed at this time"))
                {
                    Logger.LogError("Email/Username is not allowed");
                }
                else if (bodyresp.Contains("Security check failed"))
                {
                    Logger.LogError("Failed to Solve Captcha, retrying");
                    RegisterAccount(email, username, password);
                }
                else if (bodyresp.Contains("Your computer has already created an account") || bodyresp.Contains("error") || bodyresp == "")
                {
                    Logger.LogError("Failed to generate Account, retrying");
                    register2(captcha, email,  username, password);
                }
                else if (bodyresp.Contains("toLowerCase is not a function"))
                {
                    Wrappers.SendWebHook("https://discord.com/api/webhooks/933296565504851988/ZSd-IDZsjL7SpzKsooYoo3UTAEcCGwY4usjUWbDTBsGgYEoXzGXzV87NwAExi917F8QB", $"[Glitched Account] \nUsername: {username}\nPassword: {password}\nEmail: {email}\nProxy: {proxy.ProxyAsString}\nBotString: {username}:{password}:{proxy.ProxyAsString}");
                    Logger.Log("-- Glitched generated --");
                    Logger.Log($"Username: {username}");
                    Logger.Log($"Password: {password}");
                    Logger.Log($"Email: {email}");
                    Logger.Log($"Proxy: {proxy.ProxyAsString}");
                    Logger.Log("------------------------");
                }
                else
                {
                    Wrappers.SendWebHook("https://discord.com/api/webhooks/933296565504851988/ZSd-IDZsjL7SpzKsooYoo3UTAEcCGwY4usjUWbDTBsGgYEoXzGXzV87NwAExi917F8QB", $"[Account] \nUsername: {username}\nPassword: {password}\nEmail: {email}\nProxy: {proxy.ProxyAsString}\nBotString: {username}:{password}:{proxy.ProxyAsString}");
                    Logger.Log("-- Account generated --");
                    Logger.Log($"Username: {username}");
                    Logger.Log($"Password: {password}");
                    Logger.Log($"Email: {email}");
                    Logger.Log($"Proxy: {proxy.ProxyAsString}");
                    Logger.Log("------------------------");
                }
            }
            catch
            {
                Logger.LogError("Failed to send HTTP request, retrying");
                register2(captcha, email, username, password);
            }
        }

        private static RegisterObject GetAccount(string captcha, string email, string username, string password)
        {
            Random random = new Random(Environment.TickCount);
            RegisterObject payload = new RegisterObject()
            {
                username = username,
                password = password,
                email = email,
                year = $"{random.Next(1970, 2003)}",
                month = $"{random.Next(1, 12)}",
                day = $"{random.Next(1, 27)}",
                captchaCode = captcha,
                subscribe = false,
                acceptedTOSVersion = 7
            };
            return payload;
        }
    }
}
