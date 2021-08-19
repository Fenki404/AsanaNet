using System;
using System.Collections.Generic;
using System.Net;

namespace AsanaNet.Extensions
{
    public static class HttpWebRequestExtension
    {
        public static void AddArgs(this HttpWebRequest request, Dictionary<string, object> args)
        {
            var url = request.RequestUri.ToString();

            if (args.Count <= 0) return;

            if (url.Contains("?"))
            {
                url += "&";
            }
            else
            {
                url += "?";
            }

            foreach (var kv in args)
            {
                if (url.Contains(kv.Key + "="))
                    throw new ArgumentException($"Argument already exist {kv.Key}");

                url += kv.Key + "=" + Uri.EscapeUriString(kv.Value.ToString()) + "&";
            }
            url = url.TrimEnd('&');
        }
    }
}