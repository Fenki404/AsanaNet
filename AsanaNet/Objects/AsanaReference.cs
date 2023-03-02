using System;
using System.Collections.Generic;
using System.Linq;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaReference : AsanaObject, IAsanaData
    {
        [AsanaData("resource_type")]
        public string ResourceType { get; private set; }

        [AsanaData("name", SerializationFlags.Required)]
        public string Name { get; set; }

        // ------------------------------------------------------
        public AsanaReference()
        {

        }
        public AsanaReference(long id, string resourceType)
        {
            ID = id;
            ResourceType = resourceType;
        }

        public bool IsObjectLocal { get { return ID == 0; } }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public string ToJsonString()
        {
            return this.ID.ToString();
        }

        public static string MultiToJsonString(IEnumerable<AsanaReference> values)
        {
            if(values == null) return null;
 
            var ids = values.Select(x => x.ToJsonString());
            return $"[{string.Join(",", ids)}]";
        }
        public static string[] MultiToJsonArray(IEnumerable<AsanaReference> values)
        {
            var ids = values?.Select(x => x.ToJsonString());
            return ids?.ToArray();
        }
    }
}