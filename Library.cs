using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RuriLib.Attributes;
using RuriLib.Legacy.Blocks;
using RuriLib.Logging;
using RuriLib.Models.Bots;
using HttpMethod = System.Net.Http.HttpMethod;

namespace Http2
{
    [BlockCategory("Http", "Blocks For Different Http Operations", "#CD3255")]
    public class Library : BlockRequest
    {
        [Block("Performs A Custom Http Request With A Specific Version", name = "Http2Request")]
        public static async Task<string> Http2Request(
            BotData Data,
            string Url,
            RuriLib.Functions.Http.HttpMethod Method,
            bool AutoRedirect,
            string Body,
            Dictionary<string, string> Headers,
            Dictionary<string, string> Cookies,
            int Timeout,
            bool OutputRaw,
            string ContentType = "application/x-www-form-urlencoded",
            string Version = "2.0")
        {
            var httpVersion = Version switch
            {
                "3.0" => new Version(3, 0),
                "2.0" => new Version(2, 0),
                _ => new Version(1, 1),
            };

            using var handler = new HttpClientHandler
            {
                AllowAutoRedirect = AutoRedirect
            };

            // Proxy setup
            if (Data.Proxy != null)
            {
                var proxy = new WebProxy(Data.Proxy.Host, Data.Proxy.Port);

                if (Data.Proxy.NeedsAuthentication)
                    proxy.Credentials = new NetworkCredential(Data.Proxy.Username, Data.Proxy.Password);

                handler.Proxy = proxy;
            }

            using var client = new HttpClient(handler)
            {
                DefaultRequestVersion = httpVersion,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
                Timeout = Timeout > 0 ? TimeSpan.FromSeconds(Timeout) : TimeSpan.FromSeconds(100) // Default timeout
            };

            // Request message
            var request = new HttpRequestMessage(new HttpMethod(Method.ToString()), Url);

            if (!string.IsNullOrEmpty(Body))
                request.Content = new StringContent(Body, Encoding.UTF8, ContentType);

            // Add headers
            foreach (var header in Headers ?? new Dictionary<string, string>())
                request.Headers.Add(header.Key, header.Value);

            // Consolidated cookie header
            if (Cookies?.Count > 0)
                request.Headers.Add("Cookie", string.Join("; ", Cookies.Select(c => $"{c.Key}={c.Value}")));

            // Send request and get response
            var response = await client.SendAsync(request);
            var buffer = await response.Content.ReadAsByteArrayAsync();
            var content = Encoding.UTF8.GetString(buffer);

            #region Request Logging
            Data.Logger.Log(">> Http2Request\n", LogColors.DarkOrchid);
            Data.Logger.Log($"Request Method: {Method} / HTTP/{httpVersion}", LogColors.WhiteSmoke);
            Data.Logger.Log($"URL: {Url}", LogColors.WhiteSmoke);

            if (Headers?.Count > 0)
            {
                Data.Logger.Log("Request Headers:", LogColors.WhiteSmoke);
                foreach (var header in Headers)
                    Data.Logger.Log($"  {header.Key}: {header.Value}", LogColors.WhiteSmoke);
            }

            if (Cookies?.Count > 0)
            {
                Data.Logger.Log("Request Cookies:", LogColors.WhiteSmoke);
                foreach (var cookie in Cookies)
                    Data.Logger.Log($"  {cookie.Key}={cookie.Value}", LogColors.WhiteSmoke);
            }

            if (!string.IsNullOrEmpty(Body))
                Data.Logger.Log($"Request Body:\n{Body}", LogColors.WhiteSmoke);
            #endregion

            #region Response Logging
            Data.Logger.Log($"Response Code: {response.StatusCode}\n", LogColors.Yellow);

            if (response.Headers?.Count() > 0)
            {
                Data.Logger.Log("Received Headers:", LogColors.BluePurple);
                foreach (var header in response.Headers)
                    Data.Logger.Log($"  {header.Key}: {string.Join(" ", header.Value)}", LogColors.PurplePizzazz);
            }
            else
            {
                Data.Logger.Log("No Headers Received", LogColors.Pink);
            }

            Data.Logger.Log("Received Payload:", LogColors.AndroidGreen);
            if (OutputRaw)
                Data.Logger.Log(string.Join(", ", buffer.Select(b => $"0x{b:X2}")), LogColors.Amber);

            Data.Logger.Log($"{content}", LogColors.CaribbeanGreen);
            #endregion

            return content;
        }
    }
}
