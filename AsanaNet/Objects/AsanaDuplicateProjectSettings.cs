using System;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaDuplicateProjectSettings : AsanaObject
    {
        [AsanaDataAttribute("name", SerializationFlags.Required)] 
        public string Name { get; set; }

        [AsanaDataAttribute("include")] 
        public string Include { get; set; }

        [AsanaDataAttribute("schedule_dates")] 
        public AsanaScheduleDates ScheduleDates { get; set; }
    }
}
