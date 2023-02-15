using System;
using System.Collections.Generic;
using System.Linq;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaCustomField : AsanaObject, IAsanaData, IEquatable<AsanaCustomField>
    {
        [AsanaData("name", SerializationFlags.Required)]
        public string Name { get; set; }


        [AsanaData("resource_type")]
        public string ResourceType { get; private set; }
        
        [AsanaData("resource_subtype")]
        public string ResourceSubtype { get; set; }

        [AsanaData("type")]
        public string Type { get; set; }



        [AsanaData("enabled")]
        public bool Enabled { get; set; }

        [AsanaData("has_notifications_enabled", SerializationFlags.Optional | SerializationFlags.ReadOnly)]
        public bool HasNotificationsEnabled { get; set; }

        [AsanaData("is_global_to_workspace", SerializationFlags.Optional | SerializationFlags.ReadOnly)]
        public bool IsGlobalToWorkspace { get; set; }

        [AsanaData("format", SerializationFlags.Optional | SerializationFlags.ReadOnly)]
        public string Format { get; set; }

        [AsanaData("description")]
        public string Description { get; set; }

        [AsanaData("asana_created_field")]
        public string AsanaCreatedField { get; set; }


        [AsanaData("precision", SerializationFlags.Optional | SerializationFlags.ReadOnly))]
        public int Precision { get; set; }


        [AsanaData("currency_code", SerializationFlags.Optional | SerializationFlags.ReadOnly)]
        public string CurrencyCode { get; set; }

        [AsanaData("custom_label", SerializationFlags.Optional | SerializationFlags.ReadOnly)]
        public string CustomLabel { get; set; }

        [AsanaData("custom_label_position", SerializationFlags.Optional | SerializationFlags.ReadOnly)]
        public string CustomLabelPosition { get; set; }



        [AsanaData("enum_options")]
        public EnumValue[] EnumOptions { get; set; }


        [AsanaData("number_value")]
        public int NumberValue { get; set; }

        [AsanaData("text_value")]
        public string TextValue { get; set; }

        [AsanaData("display_value")]
        public string DisplayValue { get; set; }

        [AsanaData("enum_value")]
        public EnumValue EnumValue { get; set; }

        [AsanaData("multi_enum_values")]
        public EnumValue[] MultiEnumValues { get; set; }

        [AsanaData("people_value")]
        public AsanaReference[] PeopleValue { get; set; }

        [AsanaDataAttribute("date_value")]
        public AsanaDateTime DateValue { get; protected set; }


        [AsanaData("created_by")]
        public AsanaUser CreatedBy { get; set; }



        // ------------------------------------------------------
        public AsanaCustomField()
        {
        }

        public AsanaCustomField(long id)
        {
            ID = id;
        }

        public bool IsObjectLocal
        {
            get { return ID == 0; }
        }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public bool EnumValueIs(long id)
        {
            if (EnumValue == null)
                return false;

            return Enabled && EnumValue.Is(id);
        }

        public void SetValue(AsanaCustomField value)
        {
            switch (ResourceSubtype)
            {
                case "enum":
                    SetEnumValue(value.EnumValue);
                    break;
                case "text":
                    SetTextValue(value.TextValue);
                    break;
                case "number":
                    SetNumberValue(value.NumberValue);
                    break;
            }

        }
        private void SetEnumValue(EnumValue value)
        {
            if (value == null)
            {
                EnumValue = null;
                DisplayValue = null;
                return;
            }

            var selectedOption = EnumOptions?.ToList()
                .FirstOrDefault(x => x?.ID == value?.ID && value?.ID != null) ?? value;

            EnumValue = selectedOption;
            DisplayValue = selectedOption.Name;
            Enabled = true;
        }
        private void SetTextValue(string value)
        {
            TextValue = value;
            Enabled = value != null;
        }
        private void SetNumberValue(int value)
        {
            NumberValue = value;
            Enabled = true;
        }

        private sealed class EnumValueEqualityComparer : IEqualityComparer<AsanaCustomField>
        {
            public bool Equals(AsanaCustomField x, AsanaCustomField y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.EnumValue, y.EnumValue);
            }

            public int GetHashCode(AsanaCustomField obj)
            {
                return (obj.EnumValue != null ? obj.EnumValue.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<AsanaCustomField> EnumValueComparer { get; } = new EnumValueEqualityComparer();


        public bool Equals(AsanaCustomField other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(EnumValue, other.EnumValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AsanaCustomField)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (EnumValue != null ? EnumValue.GetHashCode() : 0);
            }
        }

        public static bool operator ==(AsanaCustomField left, AsanaCustomField right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AsanaCustomField left, AsanaCustomField right)
        {
            return !Equals(left, right);
        }


        private sealed class NameEqualityComparer : IEqualityComparer<AsanaCustomField>
        {
            public bool Equals(AsanaCustomField x, AsanaCustomField y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(AsanaCustomField obj)
            {
                return (obj.Name != null ? obj.Name.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<AsanaCustomField> NameComparer { get; } = new NameEqualityComparer();

    }
}