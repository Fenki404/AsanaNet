using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using AsanaNet.Objects;

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
        static public string SafeAssignString(Dictionary<string, object> source, string name)
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
        static private T SafeAssign<T>(Dictionary<string, object> source, string name, Asana host)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            var value = default(T);

            if (source.ContainsKey(name) && source[name] != null)
            {
                if (converter.CanConvertFrom(typeof(string)) && !string.IsNullOrWhiteSpace(source[name].ToString()))
                {
                    value = (T)converter.ConvertFromString(source[name].ToString());
                }
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
        static private T[] SafeAssignArray<T>(Dictionary<string, object> source, string name, Asana host)
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
        static private AsanaObject SafeAssignAsanaObject<T>(Dictionary<string, object> source, string name, Asana host) where T : AsanaObject
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
        static private T[] SafeAssignAsanaObjectArray<T>(Dictionary<string, object> source, string name, Asana host) where T : AsanaObject
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

            foreach (var p in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.Name == "Host")
                {
                    if (obj.Host != host)
                        p.SetValue(obj, host, new object[] { });
                    continue;
                }

                try
                {
                    var cas = p.GetCustomAttributes(typeof(AsanaDataAttribute), false);
                    if (cas.Length == 0)
                        continue;

                    var ca = cas[0] as AsanaDataAttribute;

                    if (!data.ContainsKey(ca.Name))
                        continue;

                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(obj, SafeAssignString(data, ca.Name), null);
                    }
                    else
                    {
                        var t = p.PropertyType.IsArray ? p.PropertyType.GetElementType() : p.PropertyType;
                        MethodInfo method = null;
                        if (typeof(AsanaObject).IsAssignableFrom(t))
                            method = typeof(Parsing).GetMethod(p.PropertyType.IsArray ? "SafeAssignAsanaObjectArray" : "SafeAssignAsanaObject", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(t);
                        else
                            method = typeof(Parsing).GetMethod(p.PropertyType.IsArray ? "SafeAssignArray" : "SafeAssign", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(t);

                        var methodResult = method.Invoke(null, new object[] { data, ca.Name, host });

                        // this check handle base-class properties
                        if (p.DeclaringType != obj.GetType())
                        {
                            var p2 = p.DeclaringType.GetProperty(p.Name);
                            p2.SetValue(obj, methodResult, null);
                        }
                        else
                        {
                            p.SetValue(obj, methodResult, null);
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
        static internal Dictionary<string, object> Serialize(AsanaObject obj, bool asString, bool dirtyOnly, bool onWrite = false)
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



                    if (writeField && dataAttribute.Fields.Length == 1)
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
                            var entries = new Dictionary<string, string>();

                            foreach (var cf in customFields)
                            {
                                if (cf.ResourceSubtype == "text")
                                    entries.Add(cf.ID.ToString(), cf.TextValue ?? "null");
                                if (cf.ResourceSubtype == "number")
                                    entries.Add(cf.ID.ToString(), cf.NumberValue.ToString());

                                if (cf.ResourceSubtype == "enum")
                                    entries.Add(cf.ID.ToString(), cf.EnumValue?.ID.ToString() ?? "null");

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
                        var quotedString = HttpUtility.JavaScriptStringEncode(asanaDate.ToDateString());
                        var valueOrString = asString ? quotedString : value;
                        dict.Add(dataAttribute.Name, valueOrString);
                    }
                    else
                    {
                        var quotedString = HttpUtility.JavaScriptStringEncode(value.ToString());
                        var valueOrString = asString ? quotedString : value;
                        dict.Add(dataAttribute.Name, valueOrString);
                    }
                }
                catch (Exception e)
                {
                    throw new SerializationException($"Error in Parsing.Serialize Property {property.Name}, value: {property.GetValue(obj)}", e);
                }
            }

            return dict;
        }


        public static Dictionary<string, object> SerializePropertiesToArgs(AsanaObject obj)
        {
            var properties = new List<string>();

            foreach (var p in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    var cas = p.GetCustomAttributes(typeof(AsanaDataAttribute), false);
                    if (cas.Length == 0)
                        continue;

                    var ca = cas[0] as AsanaDataAttribute;
                    var omit = ca?.Flags.HasFlag(SerializationFlags.Omit) ?? false;
                    if (ca == null || omit)
                        continue;

                    var required = ca.Flags.HasFlag(SerializationFlags.Required);

                    properties.Add(ca.Name);
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
                if (ca.Fields.Length == 1)
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
