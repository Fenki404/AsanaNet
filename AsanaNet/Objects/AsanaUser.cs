using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsanaNet
{
    [Serializable]
    public class AsanaUser : AsanaObject, IAsanaData
    {
        [AsanaDataAttribute("name")]
        public string           Name            { get; private set; }

        [AsanaDataAttribute("email")]
        public string           Email           { get; private set; }

        [AsanaDataAttribute("photo", SerializationFlags.Optional)]
        public AsanaUserPhoto Photo { get; private set; }

        [AsanaDataAttribute("workspaces")]
        public AsanaWorkspace[] Workspaces      { get; private set; }

        // ------------------------------------------------------

        public bool IsObjectLocal { get { return ID == 0; } }

        public AsanaUser()
        {
            
        }

        public AsanaUser(long id)
        {
            ID = id;
        }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public new static Dictionary<string, object> SerializePropertiesToArgs()
        {
            var asanaObject = new AsanaUser();
            return Parsing.SerializePropertiesToArgs(asanaObject);
        }

    }

    [Serializable]
    public class AsanaUserPhoto : AsanaObject, IAsanaData
    {
        [AsanaDataAttribute("image_21x21", SerializationFlags.Required)]
        public string Image21X21 { get; private set; }

        [AsanaDataAttribute("image_27x27", SerializationFlags.Required)]
        public string Image27X27 { get; private set; }

        [AsanaDataAttribute("image_36x36", SerializationFlags.Required)]
        public string Image36X36 { get; private set; }

        [AsanaDataAttribute("image_60x60", SerializationFlags.Required)]
        public string Image60X60 { get; private set; }

        [AsanaDataAttribute("image_128x128", SerializationFlags.Required)]
        public string Image128X128 { get; private set; }

        [AsanaDataAttribute("image_1024x1024", SerializationFlags.Required)]
        public string Image1024X1024 { get; private set; }


        public bool IsObjectLocal { get; }
    }
}
