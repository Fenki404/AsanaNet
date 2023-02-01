using System;
using System.Linq;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaDuplicateTaskSettings : AsanaObject
    {
        [AsanaData("name", SerializationFlags.Required)] 
        public string Name { get; set; }

        [AsanaData("include", SerializationFlags.Optional)] 
        public string[] Include { get; set; }


        public static AsanaDuplicateTaskSettings GetDefault(string name)
        {
            return new AsanaDuplicateTaskSettings()
            {
                Name = name,
                Include = new[]
                {
                    "notes",
                    "tags",
                    "subtasks",
                    "dates",
                    "dependencies"
                }
            };
        }

        public void AddProperty(string propertyName)
        {
            var includes = Include.ToList();
            includes.Add(propertyName);
            Include = includes.ToArray();
        }

        public override string ToString()
        {
            return $"AsanaDuplicateTaskSettings {Name} includes: {string.Join(" ", Include)}";
        }
    }
}