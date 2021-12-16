using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsanaNet.Extensions;
using AsanaNet.Objects;
using Microsoft.CSharp.RuntimeBinder;

namespace AsanaNet
{
    public enum AssigneeStatus
    {
        inbox,      //	In the inbox.
        later,      //	Scheduled for later.
        today,      //	Scheduled for today.
        upcoming        //	Marked as upcoming.
    }

    [Serializable]
    public class AsanaTask : BaseAsanaTask
    {
        [AsanaDataAttribute("parent", SerializationFlags.Optional)]
        public AsanaReference Parent { get; set; }

        public AsanaTask()
        {

        }

        public AsanaTask(AsanaWorkspace workspace) : base(workspace)
        {
        }

        public AsanaTask(AsanaWorkspace workspace, long id = 0) : base(workspace, id)
        {
        }


        public Task<AsanaTask> AddProjectAsync(AsanaProject proj, Asana host, long after = 0, long before = 0, long section = 0)
        {
            Dictionary<string, object> project = new Dictionary<string, object>();

            project.Add("project", proj.ID.ToString());

            if (after > 0)
                project.Add("insert_after", after.ToString());
            if (before > 0)
                project.Add("insert_before", before.ToString());
            if (section > 0)
                project.Add("section", section.ToString());

            // add it manually
            if (Projects == null)
                Projects = new AsanaProject[1];
            else
            {
                AsanaProject[] lProjects = Projects;
                Array.Resize(ref lProjects, Projects.Length + 1);
                Projects = lProjects;
            }
            Projects[Projects.Length - 1] = proj;

            return host.SaveAsync(this, AsanaFunction.GetFunction(Function.AddProjectToTask), project);
        }

        public Task<AsanaTask> AddProjectAsync(AsanaProject proj, long after = 0, long before = 0, long section = 0)
        {
            if (Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddProjectAsync(proj, Host, after, before, section);
        }



        public Task<AsanaTask> AddSubTaskAsync(AsanaTask task)
        {
            if (Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddSubTaskAsync(task, Host);
        }
        public Task<AsanaTask> AddSubTaskAsync(AsanaTask task, Asana host)
        {
            var notes = string.IsNullOrWhiteSpace(task.Notes) ? string.Empty : task.Notes;
            var data = new Dictionary<string, object>
            {
                {"name", task.Name},
                {"notes", notes},
                {"start_at", task.StartAt},
                {"due_at", task.DueAt}
            };

            return host.SaveAsync(this, AsanaFunction.GetFunction(Function.AddSubTaskToTask), data);
        }
    }


    [Serializable]
    public class SaveAsanaTask : BaseAsanaTask
    {
        [AsanaDataAttribute("parent", SerializationFlags.Optional)]
        public string Parent { get; set; }


        public SaveAsanaTask()
        {

        }

        public SaveAsanaTask(AsanaWorkspace workspace)
        {
            Workspace = workspace;
        }

        public SaveAsanaTask(AsanaWorkspace workspace, Int64 id = 0)
        {
            ID = id;
            Workspace = workspace;
        }


        public Task<SaveAsanaTask> AddProjectAsync(AsanaProject proj, Asana host, long after = 0, long before = 0, long section = 0)
        {
            Dictionary<string, object> project = new Dictionary<string, object>();

            project.Add("project", proj.ID.ToString());

            if (after > 0)
                project.Add("insert_after", after.ToString());
            if (before > 0)
                project.Add("insert_before", before.ToString());
            if (section > 0)
                project.Add("section", section.ToString());

            // add it manually
            if (Projects == null)
                Projects = new AsanaProject[1];
            else
            {
                AsanaProject[] lProjects = Projects;
                Array.Resize(ref lProjects, Projects.Length + 1);
                Projects = lProjects;
            }
            Projects[Projects.Length - 1] = proj;

            return host.SaveAsync(this, AsanaFunction.GetFunction(Function.AddProjectToTask), project);
        }

        public Task<SaveAsanaTask> AddProjectAsync(AsanaProject proj, long after = 0, long before = 0, long section = 0)
        {
            if (Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddProjectAsync(proj, Host, after, before, section);
        }



        public Task<SaveAsanaTask> AddSubTaskAsync(SaveAsanaTask task)
        {
            if (Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddSubTaskAsync(task, Host);
        }
        public Task<SaveAsanaTask> AddSubTaskAsync(SaveAsanaTask task, Asana host)
        {
            var notes = string.IsNullOrWhiteSpace(task.Notes) ? string.Empty : task.Notes;
            var data = new Dictionary<string, object>
            {
                {"name", task.Name},
                {"notes", notes},
                {"start_at", task.StartAt},
                {"due_at", task.DueAt}
            };

            return host.SaveAsync(this, AsanaFunction.GetFunction(Function.AddSubTaskToTask), data);
        }
    }


    [Serializable]
    public class BaseAsanaTask : AsanaObject, IAsanaData
    {
        private AsanaDateTime _startAt;
        private AsanaDateTime _startOn;
        private AsanaDateTime _dueAt;
        private AsanaDateTime _dueOn;

        //[AsanaDataAttribute("resource_type", SerializationFlags.Required)]
        //public string ResourceType => AsanaNet.Objects.ResourceType.Project;

        [AsanaDataAttribute("name", SerializationFlags.Required)]
        public string Name { get; set; }

        [AsanaDataAttribute("assignee", SerializationFlags.Optional | SerializationFlags.WriteNull, "ID")]
        public AsanaUser Assignee { get; set; }

        [AsanaDataAttribute("assignee_status", SerializationFlags.Omit)]
        public AssigneeStatus AssigneeStatus { get; set; }

        [AsanaDataAttribute("created_at", SerializationFlags.Omit)]
        public AsanaDateTime CreatedAt { get; protected set; }

        [AsanaDataAttribute("completed", SerializationFlags.Optional)]
        public bool Completed { get; set; }

        [AsanaDataAttribute("completed_at", SerializationFlags.Omit)]
        public AsanaDateTime CompletedAt { get; protected set; }

        [AsanaDataAttribute("num_subtasks", SerializationFlags.Optional)]
        public int SubTaskCount { get; set; }




        [AsanaDataAttribute("start_on", SerializationFlags.ReadOnly)]
        public AsanaDateTime StartOn
        {
            get => _startOn;
            set => _startOn = value;
        }

        [AsanaDataAttribute("start_at", SerializationFlags.Optional)]
        public AsanaDateTime StartAt
        {
            get => _startAt;
            set => _startAt = value;
            //if (value > DueAt && DueAt != null)
            //{
            //    DueAt = DueAt.DateTime.AddDays(1);
            //}
        }


        [AsanaDataAttribute("due_on", SerializationFlags.ReadOnly)]
        public AsanaDateTime DueOn
        {
            get => _dueOn;
            set => _dueOn = value;
        }

        [AsanaDataAttribute("due_at", SerializationFlags.Optional)]
        public AsanaDateTime DueAt
        {
            get => _dueAt;
            set => _dueAt = value;
            //if (value < StartAt && StartAt != null)
            //{
            //    StartAt = StartAt.DateTime.AddDays(-1);
            //}
        }


        [AsanaDataAttribute("dependents", SerializationFlags.Optional)]
        public AsanaDependent[] Dependents { get; protected set; }

        [AsanaDataAttribute("dependencies", SerializationFlags.Optional)]
        public AsanaDependent[] Dependencies { get; protected set; }


        [AsanaDataAttribute("followers", SerializationFlags.Optional)]
        public AsanaUser[] Followers { get; protected set; }

        [AsanaDataAttribute("modified_at", SerializationFlags.Omit)]
        public AsanaDateTime ModifiedAt { get; protected set; }

        [AsanaDataAttribute("notes", SerializationFlags.Optional)]
        public string Notes { get; set; }

        //[AsanaDataAttribute("parent", SerializationFlags.Optional)]
        //public AsanaReference Parent { get; set; }

        [AsanaDataAttribute("projects", SerializationFlags.Optional, "ID")]
        public AsanaProject[] Projects { get; set; }

        [AsanaDataAttribute("tags", SerializationFlags.Optional, "ID")]
        public AsanaTag[] Tags { get; protected set; }

        [AsanaDataAttribute("workspace", SerializationFlags.Required, "ID")]
        public AsanaWorkspace Workspace { get; protected set; }


        [AsanaDataAttribute("custom_fields", SerializationFlags.Optional)]
        public AsanaCustomField[] CustomFields { get; set; }

        // ------------------------------------------------------

        public bool IsObjectLocal => ID == 0;

        public void Complete()
        {
            throw new NotImplementedException();
        }

        // ------------------------------------------------------

        internal BaseAsanaTask()
        {

        }

        public BaseAsanaTask(AsanaWorkspace workspace)
        {
            Workspace = workspace;
        }

        public BaseAsanaTask(AsanaWorkspace workspace, long id = 0)
        {
            ID = id;
            Workspace = workspace;
        }

        public new static Dictionary<string, object> SerializePropertiesToArgs()
        {
            var asanaObject = new BaseAsanaTask();
            return Parsing.SerializePropertiesToArgs(asanaObject);
        }


        public void SetWorkspace(AsanaWorkspace workspace)
        {
            if (workspace == null) return;
            Workspace = workspace;

            if (Projects == null) return;
            foreach (var asanaProject in Projects)
            {
                asanaProject?.SetWorkspace(workspace);
            }
        }


        public Task AddProject(AsanaProject proj, Asana host)
        {
            Dictionary<string, object> project = new Dictionary<string, object>();
            project.Add("project", proj.ID);
            AsanaResponseEventHandler savedCallback = null;
            savedCallback = (s) =>
            {
                // add it manually
                if (Projects == null)
                    Projects = new AsanaProject[1];
                else
                {
                    AsanaProject[] lProjects = Projects;
                    Array.Resize(ref lProjects, Projects.Length + 1);
                    Projects = lProjects;
                }

                Projects[Projects.Length - 1] = proj;
                Saving -= savedCallback;
            };
            Saving += savedCallback;
            return host.Save(this, AsanaFunction.GetFunction(Function.AddProjectToTask), project);
        }

        public Task AddProject(AsanaProject proj)
        {
            if (Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddProject(proj, Host);
        }


        public Task RemoveProject(AsanaProject proj, Asana host)
        {
            Dictionary<string, object> project = new Dictionary<string, object>();
            project.Add("project", proj.ID);
            AsanaResponseEventHandler savedCallback = null;
            savedCallback = (s) =>
            {
                // add it manually
                int index = Array.IndexOf(Projects, proj);
                AsanaProject[] lProjects = new AsanaProject[Projects.Length - 1];
                if (index != 0)
                    Array.Copy(Projects, lProjects, index);
                Array.Copy(Projects, index + 1, lProjects, index, lProjects.Length - index);

                Projects = lProjects;
                Saving -= savedCallback;
            };
            Saving += savedCallback;
            return host.Save(this, AsanaFunction.GetFunction(Function.RemoveProjectFromTask), project);
        }

        public Task RemoveProject(AsanaProject proj)
        {
            if (Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return RemoveProject(proj, Host);
        }


        //public void SetDependencies(IEnumerable<AsanaDependent> dependents)
        //{
        //    this.Dependencies = dependents.ToArray();
        //}    
        //public void AddDependency(AsanaDependent dependency)
        //{
        //    var temp = new[] { dependency };
        //    Dependencies = Dependencies.Concat(temp).ToArray();
        //}


        public Task AddTag(AsanaTag proj, Asana host)
        {
            Dictionary<string, object> Tag = new Dictionary<string, object>();
            Tag.Add("tag", proj.ID);
            AsanaResponseEventHandler savedCallback = null;
            savedCallback = (s) =>
            {
                // add it manually
                if (Tags == null)
                    Tags = new AsanaTag[1];
                else
                {
                    AsanaTag[] lTags = Tags;
                    Array.Resize(ref lTags, Tags.Length + 1);
                    Tags = lTags;
                }

                Tags[Tags.Length - 1] = proj;
                Saving -= savedCallback;
            };
            Saving += savedCallback;
            return host.Save(this, AsanaFunction.GetFunction(Function.AddTagToTask), Tag);
        }

        public Task AddTag(AsanaTag proj)
        {
            if (Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddTag(proj, Host);
        }

        public Task RemoveTag(AsanaTag proj, Asana host)
        {
            Dictionary<string, object> Tag = new Dictionary<string, object>();
            Tag.Add("tag", proj.ID);
            AsanaResponseEventHandler savedCallback = null;
            savedCallback = (s) =>
            {
                // add it manually
                int index = Array.IndexOf(Tags, proj);
                AsanaTag[] lTags = new AsanaTag[Tags.Length - 1];
                if (index != 0)
                    Array.Copy(Tags, lTags, index);
                Array.Copy(Tags, index + 1, lTags, index, lTags.Length - index);

                Tags = lTags;
                Saving -= savedCallback;
            };
            Saving += savedCallback;
            return host.Save(this, AsanaFunction.GetFunction(Function.RemoveTagFromTask), Tag);
        }

        public Task RemoveTag(AsanaTag proj)
        {
            if (Host == null)
                throw new NullReferenceException("This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return RemoveTag(proj, Host);
        }

        public string GetCustomFieldColor()
        {
            return CustomFields?.FirstOrDefault(x => x.Name == "Color")?.EnumValue?.Color;
        }

        public void SetAtTimes()
        {
            if (StartAt == null && StartOn != null)
                StartAt = StartOn;
            if (DueAt == null && DueOn != null)
                DueAt = DueOn;
        }

        public bool AtTimesExist()
        {
            return StartAt?.DateTime != null && DueAt?.DateTime != null;
        }
    }
}
