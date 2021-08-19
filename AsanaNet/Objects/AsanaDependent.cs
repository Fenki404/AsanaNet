using System;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaDependent : AsanaObject, IAsanaData
    {
        [AsanaData("resource_type", SerializationFlags.Optional)]
        public string ResourceType { get; private set; }

        // ------------------------------------------------------

        public bool IsObjectLocal { get { return ID == 0; } }

        public void Complete()
        {
            throw new NotImplementedException();
        }
    }
}