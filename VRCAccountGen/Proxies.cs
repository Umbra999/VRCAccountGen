using System;
using System.IO;
using System.Net;

namespace VRCAccountGen
{
    class Proxies
    {
        private static string[] AllProxies;
        private static int ProxyIndexPosition = 0;

        public class ProxyWithCredentials
        {
            public WebProxy Proxy { get; set; }
            public string ProxyAsString { get; set; }
        }

        public static ProxyWithCredentials GetProxie()
        {
            if (ProxyIndexPosition == AllProxies.Length) ProxyIndexPosition = 0;
            string Proxy = AllProxies[ProxyIndexPosition++];

            if (Proxy.Contains("@"))
            {
                string[] Splitted = Proxy.Split(':', '@');
                string AuthUser = Splitted[0];
                string AuthPassword = Splitted[1];

                return new ProxyWithCredentials
                {
                    Proxy = new WebProxy { Address = new Uri("http://" + Proxy), Credentials = new NetworkCredential(AuthUser, AuthPassword) },
                    ProxyAsString = Proxy
                };
            }
            else
            {
                return new ProxyWithCredentials
                {
                    Proxy = new WebProxy { Address = new Uri("http://" + Proxy)},
                    ProxyAsString = Proxy
                };
            }
        }

        public static void GetNewList()
        {
            using WebClient client = new WebClient();
            string result = client.DownloadString("https://api.proxyscrape.com/v2/?request=displayproxies&protocol=http&timeout=10000&country=US&ssl=all&anonymity=all");
            AllProxies = result.Split('\n');
            ProxyIndexPosition = 0;
        }

        public static void ReadFile()
        {
            AllProxies = File.ReadAllLines("Proxie.txt");
            ProxyIndexPosition = 0;
        }
    }
}
