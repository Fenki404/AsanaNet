using System;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaDuplicateProjectJob : AsanaJob
    {
        [AsanaDataAttribute("new_project", SerializationFlags.Required)] //
        public AsanaProject NewProject { get; set; }
    }

    [Serializable]
    public class AsanaDuplicateTaskJob : AsanaJob
    {
        [AsanaDataAttribute("new_task", SerializationFlags.Required)] //
        public AsanaTask NewTask { get; set; }
    }
}
