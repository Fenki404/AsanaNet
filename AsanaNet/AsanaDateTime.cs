using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AsanaNet
{
    [Serializable]
    [TypeConverter(typeof(AsanaDateTimeConverter))]
    public class AsanaDateTime
    {
        public DateTime DateTime { get; set; }

        public AsanaDateTime()
        {
        
        }

        public AsanaDateTime(DateTime dt)
        {
            DateTime = dt;
        }
        public string ToJsonString()
        {
            object obj = new { date = DateTime.ToString("yyyy-MM-dd")};

            var jsonString = JsonSerializer.Serialize(obj);
            return jsonString; //DateTime.ToString("yyyy-MM-dd");
        }
        public string ToDateString()
        {
            return DateTime.ToString("yyyy-MM-dd");
        }
        public override string ToString()
        {
            return DateTime.ToString("yyyy-MM-ddTHH\\:mm\\:sszzzz");
        }


        public void AddHours(double hours)
        {
            DateTime = DateTime.AddHours(hours);
        }
        public void AddDays(double days)
        {
            DateTime = DateTime.AddDays(days);
        }

        public static implicit operator AsanaDateTime(DateTime dt)
        {
            return new AsanaDateTime(dt);
        }

        public static bool operator ==(AsanaDateTime a, DateTime b)
        {
            if (ReferenceEquals(a, null))
            {
                return false;
            }

            return a.DateTime == b;
        }

        public static bool operator ==(AsanaDateTime a, AsanaDateTime b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return false;
            }
            return a.DateTime == b.DateTime;
        }

        public static bool operator !=(AsanaDateTime a, DateTime b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return !ReferenceEquals(a, null);
            }
            return a.DateTime != b;
        }

        public static bool operator !=(AsanaDateTime a, AsanaDateTime b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return !ReferenceEquals(a, null);
            }
            return a.DateTime != b.DateTime;
        }




        public static bool operator <(AsanaDateTime a, DateTime b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return !ReferenceEquals(a, null);
            }
            return a.DateTime < b;
        }

        public static bool operator <(AsanaDateTime a, AsanaDateTime b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return !ReferenceEquals(a, null);
            }
            return a.DateTime < b.DateTime;
        }

        public static bool operator >(AsanaDateTime a, DateTime b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return !ReferenceEquals(a, null);
            }
            return a.DateTime > b;
        }

        public static bool operator >(AsanaDateTime a, AsanaDateTime b)
        {
            if (ReferenceEquals(a, null))
            {
                return !ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return !ReferenceEquals(a, null);
            }
            return a.DateTime > b.DateTime;
        }


        public override bool Equals(object obj)
        {
            if (obj is DateTime)
            {
                return DateTime.Equals((DateTime)obj);
            }
            if (obj is AsanaDateTime)
            {
                return this == (obj as AsanaDateTime);
            }

            return false;
            
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ DateTime.GetHashCode();
        }

        public bool Equals(AsanaDateTime a)
        {
            return DateTime.Equals(a.DateTime);
        }

    }

    public class AsanaDateTimeConverter : TypeConverter
    {
        private readonly TypeConverter _converter;

        public AsanaDateTimeConverter()
        {
            _converter = TypeDescriptor.GetConverter(typeof(DateTime));
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return _converter.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string valueString)) return null;
            var dt = (DateTime)_converter.ConvertFromString(valueString)!;
            return new AsanaDateTime(dt);

        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value.ToString();
            }

            return null;
        }
    }
}
