using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace AsanaNet
{
    [Serializable]
    public class @object : AsanaObject, IAsanaData
    {
        [AsanaDataAttribute("name")]
        public string Name { get; private set; }

        [AsanaDataAttribute("is_organization")]
        public bool? IsOrganization { get; private set; }

        // ------------------------------------------------------

        public bool IsObjectLocal { get { return ID == 0; } }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public void SetId(long id)
        {
            ID = id;
        }

        public static implicit operator @object(long ID)
        {
            return Create(typeof(@object), ID) as @object;
        }

        public override async Task RefreshAsync(Asana host = null)
        {
            CheckHost(host);
            var workspace = await (Host ?? host).GetWorkspaceByIdAsync(ID);
            Name = workspace.Name;
            IsOrganization = workspace.IsOrganization;
        }
    }
}
