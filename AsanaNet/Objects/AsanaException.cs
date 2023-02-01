using System;
using System.Xml.Linq;

namespace AsanaNet.Objects
{
    public class AsanaException : Exception
    {
        private Uri Uri;
        private string Payload = "";
        private string ResponseFromServer = "";

        public AsanaException()
        {
        }

        public AsanaException(string message)
            : base($"Asana API request failed: {message}")
        {
        }

        public AsanaException(string message, Exception inner)
            : base($"Asana API request failed: {message}", inner)
        {
        }

        public AsanaException(Uri uri, string paylod, string message, Exception inner)
            : base($"Asana API request {uri} [{paylod}] failed: {message}", inner)
        {
            Uri = uri;
            Payload = paylod;
        }
        public AsanaException(string response, Uri uri, string paylod, string message, Exception inner)
            : base($"Asana API request {uri} [{paylod}] failed: {message}  {Environment.NewLine} RESPONSE: {response}", inner)
        {
            ResponseFromServer = response;
            Uri = uri;
            Payload = paylod;
        }
    }
}