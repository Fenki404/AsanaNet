using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsanaNet.Objects;

namespace AsanaNet
{
    [Serializable]
    public class AsanaProject : AsanaObject, IAsanaData
    {
        private AsanaDateTime _startOn;
        private AsanaDateTime _dueOn;

        [AsanaDataAttribute("name", SerializationFlags.Required)] //
        public string Name { get; set; }

        [AsanaDataAttribute("created_at", SerializationFlags.Omit)] //
        public AsanaDateTime CreatedAt { get; private set; }

        [AsanaDataAttribute("modified_at", SerializationFlags.Omit)] //
        public AsanaDateTime ModifiedAt { get; private set; }

        [AsanaDataAttribute("notes")] //
        public string Notes { get; set; }

        [AsanaDataAttribute("archived", SerializationFlags.Omit)] //
        public bool Archived { get; private set; }

        [AsanaDataAttribute("workspace", SerializationFlags.Optional, "ID")] //
        public AsanaWorkspace Workspace { get; private set; }

        [AsanaDataAttribute("members", SerializationFlags.Optional | SerializationFlags.ReadOnly)] //
        public AsanaUser[] Members { get; private set; }

        [AsanaDataAttribute("followers", SerializationFlags.Optional | SerializationFlags.ReadOnly)] //
        public AsanaUser[] Followers { get; private set; }

        [AsanaDataAttribute("team", SerializationFlags.Optional, "ID")] //
        public AsanaTeam Team { get; private set; }

        [AsanaDataAttribute("color", SerializationFlags.ReadOnly)] //
        public string Color { get; private set; }



        [AsanaDataAttribute("start_on", SerializationFlags.Optional | SerializationFlags.DateOnly)]
        public AsanaDateTime StartOn
        {
            get => _startOn;
            set
            {
                if (_dueOn != null)
                    _lastSave?.Remove("due_on");

                if (_dueOn != null && (_dueOn < value || _dueOn == value))
                {
                    _startOn = new AsanaDateTime(_dueOn.DateTime.Date.Subtract(TimeSpan.FromDays(1)));
                }
                else
                {
                    _startOn = value;
                }
            }
        }
        [AsanaDataAttribute("due_on", SerializationFlags.Optional | SerializationFlags.DateOnly)]
        public AsanaDateTime DueOn
        {
            get => _dueOn;
            set => _dueOn = value;
        }


        [AsanaDataAttribute("custom_fields")]
        public AsanaCustomField[] CustomFields { get; set; }

        // ------------------------------------------------------

        //public bool IsObjectLocal { get { return true; } }
        public bool IsObjectLocal { get { return ID == 0; } }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        internal AsanaProject()
        {
        }

        static public implicit operator AsanaProject(Int64 ID)
        {
            return Create(typeof(AsanaProject), ID) as AsanaProject;
        }

        public AsanaProject(Int64 id)
        {
            ID = id;
        }

        public AsanaProject(AsanaWorkspace workspace, long id)
        {
            Workspace = workspace;
            ID = id;
        }

        public AsanaProject(AsanaWorkspace workspace, AsanaTeam team, long id = 0)
        {
            ID = id;
            Workspace = workspace;
            Team = team;
        }


        public void SetWorkspace(AsanaWorkspace workspace)
        {
            Workspace = workspace;
        }

        public new static Dictionary<string, object> SerializePropertiesToArgs()
        {
            return Parsing.SerializePropertiesToArgs(new AsanaProject());
        }

        public override async Task RefreshAsync(Asana host = null)
        {
            CheckHost(host);
            var project = await (host ?? Host).GetProjectByIdAsync(ID);
            Name = project.Name;
            CreatedAt = project.CreatedAt;
            ModifiedAt = project.ModifiedAt;
            Notes = project.Notes;
            Archived = project.Archived;
            Workspace = project.Workspace;
            Followers = project.Followers;
            Team = project.Team;
            Color = project.Color;
        }


        public async Task<AsanaObject> AddCustomFieldAsync(AsanaCustomField customField, long after = 0, long before = 0)
        {
            CheckHostAndKey();

            return await AddCustomFieldAsync(customField, Host, after, before);
        }
        public async Task<AsanaObject> AddCustomFieldAsync(AsanaCustomField customField, Asana host, long after = 0, long before = 0)
        {
            var data = new Dictionary<string, object>
            {
                { "custom_field", customField.ID.ToString() },
                { "is_important", true }
            };

            if (after > 0)
                data.Add("insert_after", after.ToString());
            if (before > 0)
                data.Add("insert_before", before.ToString());

            var result = await host.SaveAsync<AsanaObject>(this, host.AsanaFunction.GetFunction(Function.AddCustomFieldToProject), data);
            return result;
        }

        #region Users
        public async Task<AsanaObject> AddUsersAsync(IEnumerable<AsanaUser> users)
        {
            CheckHostAndKey();
            return await AddUsersAsync(users, Host);
        }
        public async Task<AsanaObject> AddUsersAsync(IEnumerable<AsanaUser> users, Asana host)
        {
            var userIds = string.Join(",", users.Select(x => x.ID.ToString()));
            var data = new Dictionary<string, object>
            {
                { "members", userIds },
            };

            var result = await host.SaveAsync<AsanaObject>(this, host.AsanaFunction.GetFunction(Function.AddUsersToProject), data);
            return result;
        }

        public async Task<AsanaObject> RemoveUsersAsync(IEnumerable<AsanaUser> users)
        {
            CheckHostAndKey();
            return await RemoveUsersAsync(users, Host);
        }
        public async Task<AsanaObject> RemoveUsersAsync(IEnumerable<AsanaUser> users, Asana host)
        {
            var userIds = string.Join(",", users.Select(x => x.ID.ToString()));
            var data = new Dictionary<string, object>
            {
                { "members", userIds },
            };

            var result = await host.SaveAsync<AsanaObject>(this, host.AsanaFunction.GetFunction(Function.RemoveUsersFromProject), data);
            return result;
        }
        #endregion

        #region Followers
        public async Task<AsanaObject> AddFollowersAsync(IEnumerable<AsanaUser> users)
        {
            CheckHostAndKey();
            return await AddFollowersAsync(users, Host);
        }
        public async Task<AsanaObject> AddFollowersAsync(IEnumerable<AsanaUser> users, Asana host)
        {
            var userIds = string.Join(",", users.Select(x => x.ID.ToString()));
            var data = new Dictionary<string, object>
            {
                { "followers", userIds },
            };

            var result = await host.SaveAsync<AsanaObject>(this, host.AsanaFunction.GetFunction(Function.AddFollowersToProject), data);
            return result;
        }

        public async Task<AsanaObject> RemoveFollowersAsync(IEnumerable<AsanaUser> users)
        {
            CheckHostAndKey();
            return await RemoveFollowersAsync(users, Host);
        }
        public async Task<AsanaObject> RemoveFollowersAsync(IEnumerable<AsanaUser> users, Asana host)
        {
            var userIds = string.Join(",", users.Select(x => x.ID.ToString()));
            var data = new Dictionary<string, object>
            {
                { "followers", userIds },
            };

            var result = await host.SaveAsync<AsanaObject>(this, host.AsanaFunction.GetFunction(Function.RemoveFollowerFromProject), data);
            return result;
        }
        #endregion


        private void CheckHostAndKey()
        {
            if (Host == null)
                throw HostNullReferenceException();
            if (string.IsNullOrWhiteSpace(Host.APIKey))
                throw ApiKeyNullReferenceException();
        }
    }
}
