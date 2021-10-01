using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AsanaNet
{
    public partial class AsanaFunction
    {
        private static Dictionary<Function, AsanaFunction> Functions = new Dictionary<Function, AsanaFunction>();
        private static Dictionary<Type, AsanaFunctionAssociation> Associations = new Dictionary<Type, AsanaFunctionAssociation>();

        public string Url { get; private set; }
        public string Method { get; private set; }

        public AsanaFunction(string url, string methd)
        {
            Url = url;
            Method = methd;
        }

        static public AsanaFunction GetFunction(Function en)
        {
            if (!Functions.ContainsKey(en))
            {
                var str = "";
                foreach (var keyValuePair in Functions)
                {
                    str += keyValuePair.Key.ToString();
                }

                throw new NotImplementedException($"GetFunction: key {en} not found, all:" + str);
            }

            return Functions[en];
        }

        static public AsanaFunctionAssociation GetFunctionAssociation(Type t)
        {
            return Associations[t];
        }

        public void AddArgs(Dictionary<string, object> args)
        {
            if (args.Count <= 0) return;

            if (Url.Contains("?"))
            {
                Url += "&";
            }
            else
            {
                Url += "?";
            }

            foreach (var kv in args)
            {
                if (Url.Contains(kv.Key + "="))
                    throw new ArgumentException($"Argument already exist {kv.Key}");

                Url += kv.Key + "=" + Uri.EscapeUriString(kv.Value.ToString()) + "&";
            }
            Url = Url.TrimEnd('&');
        }
    }

    public class AsanaFunctionAssociation
    {
        public AsanaFunction Create { get; private set; }
        public AsanaFunction Update { get; private set; }
        public AsanaFunction Delete { get; private set; }

        public AsanaFunctionAssociation(AsanaFunction create, AsanaFunction update, AsanaFunction delete)
        {
            Create = create;
            Update = update;
            Delete = delete;
        }
    }
}
