using System;

namespace AsanaNet.Objects
{
    [Serializable]
    public class AsanaMembership : AsanaObject, IAsanaData
    {
        [AsanaData("project")]
        public AsanaReference Project { get; set; }

        [AsanaData("section")]
        public AsanaReference Section { get; set; }

        public bool IsObjectLocal => ID == 0;
    }
}