using System;
using System.Collections.Generic;
using System.Linq;

namespace AsanaNet
{
    public partial class AsanaFunction
    {
        private readonly Dictionary<Function, AsanaFunction> _functions = new Dictionary<Function, AsanaFunction>();
        private readonly Dictionary<Type, AsanaFunctionAssociation> _associations = new Dictionary<Type, AsanaFunctionAssociation>();

        public string Url { get; private set; }
        public string Method { get; private set; }

        public AsanaFunction()
        {
            
        }
        public AsanaFunction(string url, string method)
        {
            Url = url;
            Method = method;
        }

        public AsanaFunction GetFunction(Function en)
        {
            if (_functions.ContainsKey(en)) return _functions[en];

            var str = _functions.Aggregate("", (current, keyValuePair) => current + keyValuePair.Key);
            throw new NotImplementedException($"GetFunction: key {en} not found, all:" + str);

        }

        public AsanaFunctionAssociation GetFunctionAssociation(Type t)
        {
            return _associations[t];
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
