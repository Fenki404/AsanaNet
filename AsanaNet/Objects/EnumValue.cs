using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;

namespace AsanaNet.Objects
{
    [Serializable]
    public class EnumValue : AsanaObject, IAsanaData
    {
        [AsanaData("resource_type")]
        public string ResourceType { get; private set; }

        [AsanaData("name", SerializationFlags.Required)]
        public string Name { get; set; }

        [AsanaData("color")]
        public string Color { get; set; }

        [AsanaData("enabled")]
        public bool Enabled { get; set; }



        // ------------------------------------------------------
        public EnumValue()
        {

        }
        public EnumValue(long id)
        {
            ID = id;
        }

        public bool IsObjectLocal { get { return ID == 0; } }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public bool Is(long id)
        {
            return Enabled && id == ID;
        }

        public string ToJsonString()
        {
            return this.ID.ToString();
        }

        public static string[] MultiEnumValueToJsonArray(IEnumerable<EnumValue> values)
        {
            var ids = values?.Select(x => x.ToJsonString());
            return ids?.ToArray();
        }
        public static string MultiEnumValueToJsonString(IEnumerable<EnumValue> values)
        {
            if(values == null) return null;
 
            var ids = values.Select(x => x.ToJsonString());
            return $"[{string.Join(",", ids)}]";
        }
    }
}