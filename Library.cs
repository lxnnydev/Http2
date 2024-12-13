using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RuriLib.Attributes;
using RuriLib.Legacy.Blocks;
using RuriLib.Logging;
using RuriLib.Models.Bots;
using HttpMethod = System.Net.Http.HttpMethod;

namespace Http2;

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
        string ContentType = "application/x-www-form-urlencoded",
        string Version = "2.0")
    {
        Version HttpVersion = Version switch
        {
            "3.0" => new Version(3, 0),
            "2.0" => new Version(2, 0),
            _ => new Version(1, 1),
        };

        HttpClientHandler? Handler = new HttpClientHandler { };
        if (Data.Proxy != null)
        {
            WebProxy? Proxy = new WebProxy(Data.Proxy.Host, Data.Proxy.Port);

            if (Data.Proxy.NeedsAuthentication)
                Proxy.Credentials = new NetworkCredential(Data.Proxy.Username, Data.Proxy.Password);

            Handler.Proxy = Proxy;
        }

        using HttpClient? Client = new HttpClient(Handler)
        {
            DefaultRequestVersion = HttpVersion,
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
        };

        HttpRequestMessage? Request = new HttpRequestMessage(new HttpMethod(Method.ToString()), Url);

        if (Body != string.Empty)
            Request.Content = new StringContent(Body, System.Text.Encoding.UTF8, ContentType);

        if (Headers != null)
            foreach (KeyValuePair<string, string> Header in Headers)
                Request.Headers.Add(Header.Key, Header.Value);

        if (Cookies != null)
            foreach (KeyValuePair<string, string> Cookie in Cookies)
                Request.Headers.Add("Cookie", $"{Cookie.Key}={Cookie.Value}");

        if (Timeout > 0)
            Client.Timeout = TimeSpan.FromSeconds(Timeout);

        if (!AutoRedirect)
            Handler.AllowAutoRedirect = false;

        HttpResponseMessage? Response = await Client.SendAsync(Request);
        string? Content = await Response.Content.ReadAsStringAsync();

        Data.Logger.Log(">> Http2Request\n", LogColors.DarkOrchid);

        #region Data
        Data.Logger.Log($"{Method} / HTTP/{HttpVersion}", LogColors.WhiteSmoke);
        Data.Logger.Log($"Host: {Url.Replace("https://", string.Empty)}", LogColors.WhiteSmoke);
        Data.Logger.Log($"Connection: {Request.Headers.Connection}", LogColors.WhiteSmoke);
        Data.Logger.Log($"User-Agent: {Request.Headers.UserAgent}", LogColors.WhiteSmoke);
        Data.Logger.Log($"Pragma: {Request.Headers.Pragma}", LogColors.WhiteSmoke);
        Data.Logger.Log($"Accept: {Request.Headers.Accept}", LogColors.WhiteSmoke);
        Data.Logger.Log($"Accept-Language: {Request.Headers.AcceptLanguage}\n\n", LogColors.WhiteSmoke);
        #endregion

        #region Status
        Data.Logger.Log($"Response Code: {Response.StatusCode}\n", LogColors.Yellow);
        #endregion

        #region Headers
        Data.Logger.Log("Received Headers:\n", LogColors.BluePurple);

        if (Response.Headers != null)
            foreach (KeyValuePair<string, IEnumerable<string>> Header in Response.Headers)
            {
                string HeaderValue = string.Empty;

                foreach (string Value in Header.Value)
                    HeaderValue += $"{Value} ";

                Data.Logger.Log($"{Header.Key}: {HeaderValue}", LogColors.PurplePizzazz);
            }
        else Data.Logger.Log("No Headers Received", LogColors.Pink);
        #endregion

        #region Payload
        Data.Logger.Log("Received Payload:\n", LogColors.AndroidGreen);
        Data.Logger.Log($"{Content}", LogColors.CaribbeanGreen);
        #endregion

        return Content;
    }
}
