using System.Collections.Generic;
using System.Dynamic;

namespace AsanaNet.Objects
{
    public class DynamicDictionary : DynamicObject
    {
        Dictionary<string, object> dict;

        public DynamicDictionary(Dictionary<string, object> dict)
        {
            this.dict = dict;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dict[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return dict.TryGetValue(binder.Name, out result);
        }
    }
}