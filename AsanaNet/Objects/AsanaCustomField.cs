using System;
using System.Collections.Generic;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaCustomField : AsanaObject, IAsanaData
    {
        [AsanaData("resource_type", SerializationFlags.Optional)]
        public string ResourceType { get; private set; }

        [AsanaData("name", SerializationFlags.Required)]
        public string Name { get; set; }

        [AsanaData("enabled", SerializationFlags.Optional)]
        public bool Enabled { get; set; }

        [AsanaData("number_value", SerializationFlags.Optional)]
        public int NumberValue { get; set; }

        [AsanaData("precision", SerializationFlags.Optional)]
        public int Precision { get; set; }

        [AsanaData("created_by", SerializationFlags.Optional)]
        public AsanaUser CreatedBy { get; set; }

        [AsanaData("display_value", SerializationFlags.Optional)]
        public string DisplayValue { get; set; }

        [AsanaData("resource_subtype", SerializationFlags.Optional)]
        public string ResourceSubtype { get; set; }


        [AsanaData("type", SerializationFlags.Optional)]
        public string Type { get; set; }


        [AsanaData("enum_options", SerializationFlags.Optional)]
        public EnumValue[] EnumOptions { get; set; }

        [AsanaData("enum_value", SerializationFlags.Optional)]
        public EnumValue EnumValue { get; set; }




        // ------------------------------------------------------
        public AsanaCustomField()
        {

        }
        public AsanaCustomField(long id)
        {
            ID = id;
        }

        public bool IsObjectLocal { get { return ID == 0; } }

        public void Complete()
        {
            throw new NotImplementedException();
        }
    }
}