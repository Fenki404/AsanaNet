using System;
using System.Collections.Generic;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaSection : AsanaObject, IAsanaData
    {
        [AsanaDataAttribute("name", SerializationFlags.Required)]
        public string Name { get; set; }

        [AsanaDataAttribute("project", SerializationFlags.Omit, "ID")]
        public AsanaProject Project { get; set; }

        [AsanaDataAttribute("projects", SerializationFlags.ReadOnly)]
        public AsanaProject[] Projects { get; set; }

        [AsanaDataAttribute("target", SerializationFlags.Omit)]
        public AsanaProject Target { get; private set; }

        public bool IsObjectLocal => ID == 0;



        //
        internal AsanaSection()
        {
        }

        //
        public AsanaSection(long id)
        {
            ID = id;
        }
        //
        public AsanaSection(AsanaProject project) : this("", project)
        {
        }

        //
        public AsanaSection(string name, AsanaProject project)
        {
            Name = name;
            Target = project;
        }


        public new static Dictionary<string, object> SerializePropertiesToArgs()
        {
            return Parsing.SerializePropertiesToArgs(new AsanaSection());
        }
    }

    [Serializable]
    public class AsanaSectionTask : AsanaObject, IAsanaData
    {
        [AsanaDataAttribute("task", SerializationFlags.Required)]
        public string Task { get; set; }

        [AsanaDataAttribute("target", SerializationFlags.Omit, "ID")]
        public AsanaSection Target { get; private set; }

        [AsanaDataAttribute("insert_after", SerializationFlags.Optional)]
        public string InsertAfter { get; set; }
        [AsanaDataAttribute("insert_before", SerializationFlags.Optional)]
        public string InsertBefore { get; set; }

        public bool IsObjectLocal => ID == 0;



        //
        internal AsanaSectionTask()
        {
        }

        //
        public AsanaSectionTask(AsanaSection targetSection, string task, string insertAfter = null, string insertBefore = null)
        {
            Target = targetSection;
            Task = task;
            InsertAfter = insertAfter;
            InsertBefore = insertBefore;
        }


        public new static Dictionary<string, object> SerializePropertiesToArgs()
        {
            return Parsing.SerializePropertiesToArgs(new AsanaSection());
        }
    }
}