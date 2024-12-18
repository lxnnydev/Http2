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

namespace Http2
{
    [BlockCategory("http", "Blocks for different http operations", "#CD3255")]
    public class library : BlockRequest
    {
        private static readonly string default_user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";

        [Block("Performs a custom HTTP request with a specific version", name = "http2_request")]
        public static async Task<string> http2_request(
            BotData data,
            string url,
            RuriLib.Functions.Http.HttpMethod method,
            bool auto_redirect,
            string body,
            Dictionary<string, string> headers,
            Dictionary<string, string> cookies,
            List<string> proxy_list, // Advanced proxy rotation
            int timeout,
            int retry_count,
            bool output_raw,
            string user_agent = null,
            string content_type = "application/x-www-form-urlencoded",
            string version = "2.0")
        {
            var http_version = version switch
            {
                "3.0" => new Version(3, 0),
                "2.0" => new Version(2, 0),
                _ => new Version(1, 1),
            };

            var active_proxy = get_random_proxy(proxy_list);

            using var handler = new HttpClientHandler
            {
                AllowAutoRedirect = auto_redirect,
                Proxy = active_proxy != null ? new WebProxy(active_proxy) : null
            };

            using var client = new HttpClient(handler)
            {
                DefaultRequestVersion = http_version,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
                Timeout = timeout > 0 ? TimeSpan.FromSeconds(timeout) : TimeSpan.FromSeconds(100)
            };

            user_agent ??= default_user_agent;

            // Prepare the request
            var request = new HttpRequestMessage(new HttpMethod(method.ToString()), url);
            if (!string.IsNullOrEmpty(body))
                request.Content = new StringContent(body, Encoding.UTF8, content_type);

            // Add headers
            request.Headers.Add("User-Agent", user_agent);
            foreach (var header in headers ?? new Dictionary<string, string>())
                request.Headers.Add(header.Key, header.Value);

            // Consolidate cookies
            if (cookies?.Count > 0)
                request.Headers.Add("Cookie", string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}")));

            HttpResponseMessage response = null;
            for (int i = 0; i < retry_count; i++)
            {
                try
                {
                    response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode) break;
                }
                catch (Exception ex) when (i < retry_count - 1)
                {
                    data.logger.Log($"Retrying request due to: {ex.Message}. Attempt {i + 1}/{retry_count}", LogColors.Yellow);
                }
            }

            if (response == null)
                throw new Exception("Failed after all retries");

            var buffer = await response.Content.ReadAsByteArrayAsync();
            var content = Encoding.UTF8.GetString(buffer);

            #region Logging
            data.logger.Log(">> http2_request\n", LogColors.DarkOrchid);
            data.logger.Log($"Request Method: {method} / HTTP/{http_version}", LogColors.WhiteSmoke);
            data.logger.Log($"URL: {url}", LogColors.WhiteSmoke);

            if (headers?.Count > 0)
            {
                data.logger.Log("Request Headers:", LogColors.WhiteSmoke);
                foreach (var header in headers)
                    data.logger.Log($"  {header.Key}: {header.Value}", LogColors.WhiteSmoke);
            }

            if (cookies?.Count > 0)
            {
                data.logger.Log("Request Cookies:", LogColors.WhiteSmoke);
                foreach (var cookie in cookies)
                    data.logger.Log($"  {cookie.Key}={cookie.Value}", LogColors.WhiteSmoke);
            }

            if (!string.IsNullOrEmpty(body))
                data.logger.Log($"Request Body:\n{body}", LogColors.WhiteSmoke);

            data.logger.Log($"Response Code: {response.StatusCode}\n", LogColors.Yellow);

            if (response.Headers?.Count() > 0)
            {
                data.logger.Log("Received Headers:", LogColors.BluePurple);
                foreach (var header in response.Headers)
                    data.logger.Log($"  {header.Key}: {string.Join(" ", header.Value)}", LogColors.PurplePizzazz);
            }
            else
            {
                data.logger.Log("No Headers Received", LogColors.Pink);
            }

            data.logger.Log("Received Payload:", LogColors.AndroidGreen);
            if (output_raw)
                data.logger.Log(string.Join(", ", buffer.Select(b => $"0x{b:X2}")), LogColors.Amber);

            data.logger.Log($"{content}", LogColors.CaribbeanGreen);
            #endregion

            return content;
        }

        [Block("Performs multiple HTTP requests concurrently", name = "multi_threaded_requests")]
        public static async Task<List<string>> multi_threaded_requests(
            BotData data,
            List<string> urls,
            RuriLib.Functions.Http.HttpMethod method,
            bool auto_redirect,
            string body,
            Dictionary<string, string> headers,
            Dictionary<string, string> cookies,
            List<string> proxy_list,
            int timeout,
            int retry_count,
            bool output_raw,
            string user_agent = null,
            string content_type = "application/x-www-form-urlencoded",
            string version = "2.0")
        {
            var tasks = urls.Select(url =>
                http2_request(
                    data, url, method, auto_redirect, body, headers, cookies, proxy_list, timeout, retry_count, output_raw, user_agent, content_type, version)
            ).ToList();

            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        private static string get_random_proxy(List<string> proxy_list)
        {
            if (proxy_list == null || proxy_list.Count == 0) return null;
            var random = new Random();
            return proxy_list[random.Next(proxy_list.Count)];
        }
    }
}
