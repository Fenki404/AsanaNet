using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using AsanaNet.Objects;
using DAL.Extensions;

namespace AsanaNet
{
    public static class Parsing
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string SafeAssignString(Dictionary<string, object> source, string name)
        {
            if (source.ContainsKey(name))
            {
                return source[name]?.ToString();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static T SafeAssign<T>(Dictionary<string, object> source, string name, Asana host)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            var value = default(T);

            if (source.ContainsKey(name) && source[name] != null)
            {
                if (source[name] is Dictionary<string, object> dict)
                {
                    var firstValue = dict.First(kv => kv.Value != null);
                    return ConvertFromString(converter, (string)firstValue.Value, value);
                }

                var stringValue = source[name].ToString();
                value = ConvertFromString(converter, stringValue, value);
            }
            return value;
        }

        private static T ConvertFromString<T>(TypeConverter converter, string stringValue, T value)
        {
            if (converter.CanConvertFrom(typeof(string)) && !string.IsNullOrWhiteSpace(stringValue))
            {
                value = (T)converter.ConvertFromString(stringValue);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static T[] SafeAssignArray<T>(Dictionary<string, object> source, string name, Asana host)
        {
            T[] value = null;

            if (source.ContainsKey(name) && source[name] != null)
            {
                throw new NotImplementedException();
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static AsanaObject SafeAssignAsanaObject<T>(Dictionary<string, object> source, string name, Asana host) where T : AsanaObject
        {
            T value = null;

            if (source.ContainsKey(name) && source[name] != null)
            {
                var obj = source[name] as Dictionary<string, object>;
                value = (T)AsanaObject.Create(typeof(T));
                Deserialize(obj, value, host);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static T[] SafeAssignAsanaObjectArray<T>(Dictionary<string, object> source, string name, Asana host) where T : AsanaObject
        {
            T[] value = null;

            if (source.ContainsKey(name) && source[name] != null)
            {
                var list = source[name] as List<object>;

                value = new T[list.Count];
                for (var i = 0; i < list.Count; ++i)
                {
                    var newObj = (T)AsanaObject.Create(typeof(T));
                    Deserialize(list[i] as Dictionary<string, object>, newObj, host);
                    value[i] = newObj;
                }
            }

            return value;
        }

        /// <summary>
        /// Deserializes a dictionary based on AsanaDataAttributes
        /// </summary>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        internal static void Deserialize(Dictionary<string, object> data, AsanaObject obj, Asana host)
        {
            if (obj == null)
                return;

            var objectType = obj.GetType();
            var objectProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in objectProperties)
            {
                if (propertyInfo.Name == "Host")
                {
                    if (obj.Host != host)
                        propertyInfo.SetValue(obj, host, new object[] { });
                    continue;
                }

                try
                {
                    var customAttributes = propertyInfo.GetCustomAttributes(typeof(AsanaDataAttribute), false);
                    if (customAttributes.Length == 0)
                        continue;

                    var customAttribute = customAttributes[0] as AsanaDataAttribute;

                    if (!data.ContainsKey(customAttribute.Name))
                        continue;

                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        propertyInfo.SetValue(obj, SafeAssignString(data, customAttribute.Name), null);
                    }
                    else
                    {
                        var typeOfProperty = propertyInfo.PropertyType.IsArray ? propertyInfo.PropertyType.GetElementType() : propertyInfo.PropertyType;
                        MethodInfo method = null;

                        //if(objectType == typeof(AsanaCustomField) && typeOfProperty == typeof(AsanaDateTime))
                        //    Debug.WriteLine("AsanaDateTime");

                        if (typeof(AsanaObject).IsAssignableFrom(typeOfProperty))
                            method = typeof(Parsing).GetMethod(propertyInfo.PropertyType.IsArray ? "SafeAssignAsanaObjectArray" : "SafeAssignAsanaObject", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(typeOfProperty);
                        else
                            method = typeof(Parsing).GetMethod(propertyInfo.PropertyType.IsArray ? "SafeAssignArray" : "SafeAssign", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(typeOfProperty);

                        var methodResult = method.Invoke(null, new object[] { data, customAttribute.Name, host });

                        // this check handle base-class properties
                        if (propertyInfo.DeclaringType != objectType)
                        {
                            var p2 = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name);
                            p2.SetValue(obj, methodResult, null);
                        }
                        else
                        {
                            propertyInfo.SetValue(obj, methodResult, null);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        //static internal T SetValueOnBaseType<T>

        /// <summary>
        /// Deserializes a dictionary based on AsanaDataAttributes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="asString"></param>
        /// <param name="dirtyOnly"></param>
        /// <param name="onWrite"></param>
        internal static Dictionary<string, object> Serialize(AsanaObject obj, bool asString, bool dirtyOnly, bool onWrite = false)
        {
            var dict = new Dictionary<string, object>();

            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    var customAttributes = property.GetCustomAttributes(typeof(AsanaDataAttribute), false);
                    if (customAttributes.Length == 0)
                        continue;

                    var dataAttribute = customAttributes[0] as AsanaDataAttribute;

                    if (dataAttribute.Flags.HasFlag(SerializationFlags.Omit))
                        continue;
                    if (dataAttribute.Flags.HasFlag(SerializationFlags.ReadOnly) && onWrite)
                        continue;

                    var required = dataAttribute.Flags.HasFlag(SerializationFlags.Required);
                    var writeField = dataAttribute.Flags.HasFlag(SerializationFlags.WriteField);
                    var writeNull = dataAttribute.Flags.HasFlag(SerializationFlags.WriteNull);
                    var dateOnly = dataAttribute.Flags.HasFlag(SerializationFlags.DateOnly);
                    var argsSuffixFields = dataAttribute.Flags.HasFlag(SerializationFlags.PropertyArgsSuffixFields);

                    var value = property.GetValue(obj, new object[] { });

                    if (dirtyOnly && !obj.IsDirty(dataAttribute.Name, value))
                        continue;

                    var present = ValidateSerializableValue(ref value, dataAttribute, property);
                    if (present == false)
                    {
                        if (writeNull)
                        {
                            dict.Add(dataAttribute.Name, asString ? "null" : value);
                            continue;
                        }

                        if (!required)
                            continue;
                        throw new MissingFieldException("Couldn't save object because it was missing a required field: " + property.Name);
                    }



                    if (writeField && dataAttribute.Fields.Length == 1 && !argsSuffixFields)
                    {
                        var pInternal = value.GetType().GetProperty(dataAttribute.Fields[0]);
                        if (pInternal != null)
                        {
                            var internalValue = pInternal.GetValue(value, new object[] { });
                            dict.Add(dataAttribute.Name, asString ? internalValue.ToString() : internalValue);
                            continue;
                        }
                    }

                    if (value.GetType().IsArray)
                    {
                        if (value is AsanaCustomField[] customFields)
                        {
                            var entries = new Dictionary<string, object>();
                  
                            foreach (var cf in customFields)
                            {
                                switch (cf.ResourceSubtype)
                                {
                                    case "text":
                                        entries.Add(cf.ID.ToString(), cf.TextValue ?? "null");
                                        break;
                                    case "number":
                                        entries.Add(cf.ID.ToString(), cf.NumberValue.ToString());
                                        break;
                                    case "date":
                                        if (cf.DateValue != null)
                                        {
                                            if (cf.DateValue.DateTime.IsDateOnly())
                                            {
                                                var dateObj = new { date = cf.DateValue.ToDateString() };
                                                entries.Add(cf.ID.ToString(), dateObj);
                                            }
                                            else
                                            {
                                                var dateObj = new
                                                {
                                                    date = cf.DateValue.ToDateString(),
                                                    date_time = cf.DateValue.ToString()
                                                };
                                                entries.Add(cf.ID.ToString(), dateObj);
                                            }
                                        }
                                        else
                                        {
                                            entries.Add(cf.ID.ToString(), "null");
                                        }
                                        break;
                                    case "enum":
                                        entries.Add(cf.ID.ToString(), cf.EnumValue?.ToJsonString() ?? "null");
                                        break;
                                    case "multi_enum":
                                        //entries.Add(cf.ID.ToString(), EnumValue.MultiEnumValueToJsonString(cf.MultiEnumValues) ?? "null");
                                        entries.Add(cf.ID.ToString(), (object)EnumValue.MultiEnumValueToJsonArray(cf.MultiEnumValues) ?? "null");
                                        break;              
                                    case "people":
                                        entries.Add(cf.ID.ToString(),
                                            (object)AsanaReference.MultiToJsonArray(cf.PeopleValue) ?? "null");
                                        break;
                                }
                            }

                            dict.Add(dataAttribute.Name, entries);
                        }
                        else
                        {
                            var count = 0;
                            foreach (var x in (object[])value)
                            {
                                dict.Add(dataAttribute.Name + "[" + count + "]", asString ? x.ToString() : x);
                                count++;
                            }
                        }
                    }
                    else if (value is AsanaDateTime asanaDate && dateOnly)
                    {
                        AddAsanaDateOnly(asString, asanaDate, value, dict, dataAttribute.Name);
                    }
                    else
                    {
                        AddAsanaDateTime(asString, value, dict, dataAttribute.Name);
                    }
                }
                catch (Exception e)
                {
                    throw new SerializationException($"Error in Parsing.Serialize Property {property.Name}, value: {property.GetValue(obj)}", e);
                }
            }

            return dict;
        }

        private static void AddAsanaDateOnly(bool asString, AsanaDateTime asanaDate, object value, IDictionary<string, object> dict,
            string attributeName)
        {
            var quotedString = HttpUtility.JavaScriptStringEncode(asanaDate.ToDateString());
            var valueOrString = asString ? quotedString : value;
            dict.Add(attributeName, valueOrString);
        }

        private static void AddAsanaDateTime(bool asString, object value, IDictionary<string, object> dict, string attributeName)
        {
            var quotedString = HttpUtility.JavaScriptStringEncode(value.ToString());
            var valueOrString = asString ? quotedString : value;
            dict.Add(attributeName, valueOrString);
        }


        /// <summary>
        /// Deserializes a dictionary based on AsanaDataAttributes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> SerializePropertiesToArgs(AsanaObject obj)
        {
            var properties = new List<string>();
 
            foreach (var propertyInfo in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    var customAttributes = propertyInfo.GetCustomAttributes(typeof(AsanaDataAttribute), false);
                    if (customAttributes.Length == 0)
                        continue;

                    var customAttribute = customAttributes[0] as AsanaDataAttribute;

                    var omit = customAttribute?.Flags.HasFlag(SerializationFlags.Omit) ?? false;
                    var required = customAttribute.Flags.HasFlag(SerializationFlags.Required);
                    var argsSuffixFields = customAttribute.Flags.HasFlag(SerializationFlags.PropertyArgsSuffixFields);
                    var additionalArgsSuffixFields = customAttribute.Flags.HasFlag(SerializationFlags.AdditionalPropertyArgsSuffixFields);

                    if (customAttribute == null || omit)
                        continue;

                    if (argsSuffixFields)
                    {
                        properties.AddRange(customAttribute.Fields.Select(field => $"{customAttribute.Name}.{field}"));
                    }
                    else
                    {
                        properties.Add(customAttribute.Name);
                        if(additionalArgsSuffixFields) properties.AddRange(customAttribute.Fields.Select(field => $"{customAttribute.Name}.{field}"));
                    }
                }
                catch (Exception)
                {
                }
            }

            if (properties.Count == 0)
                return null;

            var requestFields = new Dictionary<string, object>
            {
                {"opt_fields", string.Join(",", properties)},
            };

            return requestFields;
        }


        internal static bool ValidateSerializableValue(ref object value, AsanaDataAttribute ca, PropertyInfo p)
        {
            var present = true;
            var argsSuffixFields = ca.Flags.HasFlag(SerializationFlags.PropertyArgsSuffixFields);

            // check we're valid -- edge cases first
            if (value == null)
            {
                present = false;
            }
            else if (value is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                    present = false;
            }
            else if (value is DateTime dateTime)
            {
                if (dateTime == new DateTime())
                    present = false;
            }
            else if (value is AsanaObject _)
            {
                if (ca.Fields.Length == 1 && !argsSuffixFields)
                {
                    var pInternal = value.GetType().GetProperty(ca.Fields[0]);
                    if (pInternal == null)
                        throw new CustomAttributeFormatException(
                            $"The AsanaDataAttribute for '{p.Name}' specifies the Property '{ca.Fields[0]}' as a serialization value but this Property couldn't be found.");

                    value = pInternal.GetValue(value, new object[] { });
                    return ValidateSerializableValue(ref value, ca, p);
                }

                throw new NotImplementedException();
            }

            return present;
        }
    }
}
