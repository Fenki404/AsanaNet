﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using AsanaNet.Objects;

namespace AsanaNet
{
    [Serializable]
    public class AsanaProject : AsanaObject, IAsanaData
    {
        [AsanaDataAttribute("name", SerializationFlags.Required)] //
        public string Name { get; set; }

        [AsanaDataAttribute("created_at", SerializationFlags.Omit)] //
        public AsanaDateTime CreatedAt { get; private set; }

        [AsanaDataAttribute("modified_at", SerializationFlags.Omit)] //
        public AsanaDateTime ModifiedAt { get; private set; }

        [AsanaDataAttribute("notes", SerializationFlags.Optional)] //
        public string Notes { get; set; }

        [AsanaDataAttribute("archived", SerializationFlags.Omit)] //
        public bool Archived { get; private set; }

        [AsanaDataAttribute("workspace", SerializationFlags.Optional, "ID")] //
        public AsanaWorkspace Workspace { get; private set; }

        [AsanaDataAttribute("followers", SerializationFlags.Optional)] //
        public AsanaUser[] Followers { get; private set; }

        [AsanaDataAttribute("team", SerializationFlags.Optional, "ID")] //
        public AsanaTeam Team { get; private set; }

        [AsanaDataAttribute("color", SerializationFlags.Omit)] //
        public string Color { get; private set; }



        [AsanaDataAttribute("start_on", SerializationFlags.Optional)]
        public AsanaDateTime StartOn { get; set; }
        [AsanaDataAttribute("due_on", SerializationFlags.Optional)]
        public AsanaDateTime DueOn { get; set; }


        [AsanaDataAttribute("custom_fields", SerializationFlags.Optional)]
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

    }
}
