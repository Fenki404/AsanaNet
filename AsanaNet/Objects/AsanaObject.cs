using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AsanaNet
{
    /// <summary>
    /// This interface allows objects to specify whether they are 'in tact' and provides the interface for fetching them.
    /// </summary>
    public interface IAsanaData
    {
        bool IsObjectLocal { get; }
        //void Complete();
    }

    [Serializable]
    public abstract class AsanaObject : IEquatable<AsanaObject>
    {

        [AsanaDataAttribute("gid", SerializationFlags.Omit)]
        public Int64 ID { get; protected set; }

        public Asana Host { get; protected set; }

        /// <summary>
        /// A positive response has been received but any object updating has yet to be performed.
        /// </summary>
        public event AsanaResponseEventHandler Saving;

        /// <summary>
        /// The object was saved successfully and changes should be reflected.
        /// </summary>
        public event AsanaResponseEventHandler Saved;

        // memento
        protected Dictionary<string, object> _lastSave;

        internal bool IsDirty(string key, object value)
        {
            if (_lastSave != null)
            {
                var gotValue = _lastSave.TryGetValue(key, out var lastValue);
                if (!gotValue) 
                    return true;

                if (value == null && lastValue == null) 
                    return false;
                if (value == null) 
                    return !lastValue.Equals(null);

                return !value.Equals(lastValue);
            }

            return true;
        }

        /// <summary>
        /// Set all Properties to Unchanged State
        /// </summary>
        public void SetPropertiesUnchanged()
        {
            _lastSave = new Dictionary<string, object>();

            foreach (var property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var customAttributes = property.GetCustomAttributes(typeof(AsanaDataAttribute), false);
                if (customAttributes.Length == 0)
                    continue;

                var dataAttribute = customAttributes[0] as AsanaDataAttribute;
                var value = property.GetValue(this, new object[] { });

                if (dataAttribute != null) _lastSave.Add(dataAttribute.Name, value);
            }
        }

        public void SetPropertiesChanged()
        {
            _lastSave = new Dictionary<string, object>();
        }
        public void SetPropertyChanged(string propertyName)
        { 
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var objectProperty = properties.FirstOrDefault(x => x.Name == propertyName);
            if(objectProperty == null) return;

            var customAttributes = objectProperty.GetCustomAttributes(typeof(AsanaDataAttribute), false);
            if (customAttributes.Length == 0)
                return;

            var dataAttribute = customAttributes[0] as AsanaDataAttribute;
            var name = dataAttribute?.Name;
            
            if(name != null) 
                _lastSave?.Remove(name);
        }

        internal void SavingCallback(Dictionary<string, object> saved)
        {
            _lastSave = saved;

            if (Saving != null)
                Saving(this);
        }

        internal void SavedCallback()
        {
            if (Saved != null)
                Saved(this);
        }

        /// <summary>
        /// Creates a new T without requiring a public constructor
        /// </summary>
        /// <param name="t"></param>
        internal static AsanaObject Create(Type t)
        {
            try
            {
                AsanaObject o = (AsanaObject)Activator.CreateInstance(t, true);
                return o;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a new T without requiring a public constructor
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        internal static AsanaObject Create(Type t, Int64 ID)
        {
            AsanaObject o = Create(t);
            o.ID = ID;
            return o;
        }

        /// <summary>
        /// Parameterless contructor
        /// </summary>
        internal AsanaObject()
        {

        }

        /// <summary>
        /// Overrides the ToString method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ID.ToString();
        }

        public static Dictionary<string, object> SerializePropertiesToArgs()
        {
            return new Dictionary<string, object>();
        }

        public static bool operator ==(AsanaObject a, long id)
        {
            return a.ID == id;
        }

        public static bool operator !=(AsanaObject a, long id)
        {
            return a.ID != id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AsanaObject)obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public virtual Task Refresh(Asana host = null)
        {
            return RefreshAsync(host);
        }

        public virtual Task RefreshAsync(Asana host = null)
        {
            throw new NotImplementedException();
        }

        protected internal void CheckHost(Asana host)
        {
            if ((host ?? Host) == null)
                throw new NullReferenceException("Host not set to remote data update.");
        }


        private sealed class IdEqualityComparer : IEqualityComparer<AsanaObject>
        {
            public bool Equals(AsanaObject x, AsanaObject y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.ID == y.ID;
            }

            public int GetHashCode(AsanaObject obj)
            {
                return obj.ID.GetHashCode();
            }
        }

        public static IEqualityComparer<AsanaObject> IdComparer { get; } = new IdEqualityComparer();

        public bool Equals(AsanaObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID == other.ID;
        }

        public static NullReferenceException HostNullReferenceException()
        {
            return new NullReferenceException(
                "This AsanaObject does not have a host associated with it so you must specify one when saving.");
        }

        public static NullReferenceException ApiKeyNullReferenceException()
        {
            return new NullReferenceException(
                "This AsanaObject does not have a API Key associated with it so you must specify one when saving.");
        }

    }

    public interface IAsanaObjectCollection : IList
    {
    }

    public interface IAsanaObjectCollection<T> : IAsanaObjectCollection, IList<T> where T : AsanaObject
    {
    }

    [Serializable]
    public class AsanaObjectCollection<T> : List<T>, IAsanaObjectCollection<T> where T : AsanaObject
    {
    }

    [Serializable]
    public class AsanaObjectCollection : AsanaObjectCollection<AsanaObject>
    {
    }

    public static class AsanaObjectCollectionExtensions
    {
        public static List<Task> RefreshAll<T>(this IAsanaObjectCollection objects) where T : AsanaObject
        {
            List<Task> workers = new List<Task>();
            foreach (T o in objects)
            {
                if (o.Host == null)
                    throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
                workers.Add(o.Refresh());
            }
            return workers;
        }
    }

    public static class AsanaObjectExtensions
    {
        public static Task Save(this AsanaObject obj, Asana host, AsanaFunction function)
        {
            return host.Save(obj, function);
        }

        public static Task Save(this AsanaObject obj, Asana host)
        {
            return host.Save(obj, null);
        }

        public static Task Save(this AsanaObject obj, AsanaFunction function)
        {
            if (obj.Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return obj.Host.Save(obj, function);
        }

        public static Task Save(this AsanaObject obj)
        {
            if (obj.Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return obj.Host.Save(obj, null);
        }
        public static async Task<T> SaveAsync<T>(this AsanaObject obj, Asana host = null)
        {
            if (obj.Host == null && host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");

            if (obj.Host == null && host != null)
            {
                var response = await host.SaveAsync(obj, null);
                response?.SetPropertiesUnchanged();

                return (T)Convert.ChangeType(response, typeof(T), CultureInfo.InvariantCulture);
            }
            else
            {
                var response = await obj.Host.SaveAsync(obj, null);
                response?.SetPropertiesUnchanged();

                return (T)Convert.ChangeType(response, typeof(T), CultureInfo.InvariantCulture);
            }
        }


        public static Task Delete(this AsanaObject obj, Asana host)
        {
            return host.Delete(obj);
        }

        public static Task Delete(this AsanaObject obj)
        {
            if (obj.Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return obj.Host.Delete(obj);
        }

        public static async Task DeleteAsync(this AsanaObject obj, Asana host = null)
        {
            if (obj.Host == null && host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");


            if (obj.Host == null && host != null)
            {
                var response = await host.DeleteAsync(obj);
                Debug.WriteLine(response);
                //return (T)Convert.ChangeType(response, typeof(T));
            }
            else
            {
                var response = await obj.Host.DeleteAsync(obj);
                Debug.WriteLine(response);
                //return (T)Convert.ChangeType(response, typeof(T));
            }
        }

      
    }
}
