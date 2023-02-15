using System;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaDependent : AsanaObject, IAsanaData
    {
        [AsanaData("resource_type")]
        public string ResourceType { get; private set; }

        // ------------------------------------------------------
        public AsanaDependent()
        {

        }
        public AsanaDependent(long id, string type = null)
        {
            ID = id;
            ResourceType = type;
        }

        public bool IsObjectLocal => ID == 0;

        public void Complete()
        {
            throw new NotImplementedException();
        }
    }
}