﻿using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FritzBoxClient
{
    public abstract class BaseAccessor : IDisposable
    {
        public string FritzModel { get; protected set; }
        public string FritzOsVersion { get; protected set; }
        public double PowerConsumptionPercentage { get; protected set; }
        protected string CurrentSid { get; set; } = null!;
        protected DateTime SidTimestamp { get; set; }
        protected bool IsSidValid
        {
            get => (DateTime.Now - SidTimestamp) < TimeSpan.FromMinutes(10);
        }
        protected static string FritzBoxUrl = string.Empty;
        protected string Password = string.Empty;
        protected string FritzUserName = string.Empty;
        /// <summary>
        /// Checks if the given URL starts with "http://" or "https://".
        /// If not, "https://" is added by default.
        /// </summary>
        /// <param name="url">The URL to validate and adjust if necessary.</param>
        /// <returns>The corrected URL with a valid scheme.</returns>
        protected static string EnsureUrlHasScheme(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return "https://" + url.TrimStart('/');
            }
            return url;
        }
        protected async Task InitializeAsync()
        {
            if (!IsSidValid)
                await GenerateSessionIdAsync();
            var content = new StringContent($"&sid={CurrentSid}&page=overview", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = HttpRequestFritzBox("/data.lua", content, HttpRequestMethod.Post);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            FritzOsVersion = json["data"]!["fritzos"]!["nspver"]!.ToObject<string>()!;
            FritzModel = json["data"]!["fritzos"]!["Productname"]!.ToObject<string>()!;
            PowerConsumptionPercentage = json["data"]!["fritzos"]!["energy"]!.ToObject<double>()!;

        }
        protected static string CalculateMD5(string input)
        {
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.Unicode.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
        protected async Task<bool> GenerateSessionIdAsync()
        {
            try
            {
                var response = HttpRequestFritzBox("/login_sid.lua", null, HttpRequestMethod.Get);
                var xml = XDocument.Parse(await response.Content.ReadAsStringAsync());
                var sid = xml.Root!.Element("SID")!.Value;
                if (sid != "0000000000000000")
                    return false;

                var challenge = xml.Root.Element("Challenge")!.Value;
                FritzUserName = FritzUserName is "" ? xml.Root.Element("Users")?.Element("User")!.Value! : FritzUserName;

                var responseHash = CalculateMD5(challenge + "-" + Password);
                var content = new StringContent($"response={challenge}-{responseHash}&username={FritzUserName}&lp=overview&loginView=simple", Encoding.UTF8, "application/x-www-form-urlencoded");

                var loginResponse = HttpRequestFritzBox("/login_sid.lua", content, HttpRequestMethod.Post);
                var loginXml = XDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
                var loginSid = loginXml.Root!.Element("SID")!.Value;

                if (loginSid == "0000000000000000")
                    throw new Exception("Login failed. Ensure (if set) username and password is correct!");

                CurrentSid = loginSid;
                SidTimestamp = DateTime.Now.AddMinutes(10);
                return true;
            }
            catch (XmlException)
            {
                throw new XmlException("Failed to parse xml page. Try a different fritzbox url.");
            }
        }
        protected static HttpResponseMessage HttpRequestFritzBox(string relativeUrl, StringContent? bodyParameters, HttpRequestMethod method)
        {
            using var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => true;
            using var httpClient = new HttpClient(handler) { BaseAddress = new Uri(FritzBoxUrl) };
            if (method is HttpRequestMethod.Post)
            {
                var response = httpClient.PostAsync(relativeUrl, bodyParameters)
                    .GetAwaiter()
                    .GetResult();
                return response;
            }
            else if (method is HttpRequestMethod.Get)
            {
                var response = httpClient.GetAsync(relativeUrl)
                    .GetAwaiter()
                    .GetResult();
                return response;
            }
            throw new NotImplementedException("Only Get and Post methods are supported!");
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //If not valid, the sid gets automatically invalid
                if (IsSidValid)
                {
                    var bodyParams = new StringContent($"xhr=1&sid={CurrentSid}&logout=1&no_sidrenew=1", Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = HttpRequestFritzBox("/index.lua", bodyParams, HttpRequestMethod.Post);
                    response.EnsureSuccessStatusCode();
                }
            }
        }
        ~BaseAccessor()
        {
            Dispose(false);
        }
    }
}
