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
        inbox, //	In the inbox.
        later, //	Scheduled for later.
        today, //	Scheduled for today.
        upcoming //	Marked as upcoming.
    }

    [Serializable]
    public class AsanaTask : BaseAsanaTask
    {
        [AsanaDataAttribute("parent", SerializationFlags.ReadOnly)]
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


        public Task<AsanaTask> AddProjectAsync(AsanaProject proj, Asana host, long after = 0, long before = 0,
            long section = 0)
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

            Projects[^1] = proj;

            return host.SaveAsync(this, Host.AsanaFunction.GetFunction(Function.AddProjectToTask), project);
        }

        public Task<AsanaTask> AddProjectAsync(AsanaProject proj, long after = 0, long before = 0, long section = 0)
        {
            if (Host == null)
                throw new NullReferenceException(
                    "This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddProjectAsync(proj, Host, after, before, section);
        }


        public Task<AsanaTask> AddSubTaskAsync(AsanaTask task)
        {
            if (Host == null)
                throw new NullReferenceException(
                    "This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddSubTaskAsync(task, Host);
        }

        public Task<AsanaTask> AddSubTaskAsync(AsanaTask task, Asana host)
        {
            var notes = string.IsNullOrWhiteSpace(task.Notes) ? string.Empty : task.Notes;
            var data = new Dictionary<string, object>
            {
                { "name", task.Name },
                { "notes", notes },
                { "start_at", task.StartAt },
                { "due_at", task.DueAt },
            };

            task.Workspace ??= Workspace;
            data = Parsing.Serialize(task, true, false, true);

            return host.SaveAsync(this, host.AsanaFunction.GetFunction(Function.AddSubTaskToTask), data);
        }

        public new static Dictionary<string, object> SerializePropertiesToArgs()
        {
            var asanaObject = new AsanaTask();
            var properties = Parsing.SerializePropertiesToArgs(asanaObject);
            //properties.Remove("start_on");
            //properties.Remove("due_on");
            return properties;
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

        public SaveAsanaTask(AsanaWorkspace workspace, long id = 0)
        {
            ID = id;
            Workspace = workspace;
            if (id == 0)
            {
                this.SetPropertiesChanged();
            }
        }


        public Task<SaveAsanaTask> AddProjectAsync(AsanaProject proj, Asana host, long after = 0, long before = 0,
            long section = 0)
        {
            var project = new Dictionary<string, object> { { "project", proj.ID.ToString() } };

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

            return host.SaveAsync(this, host.AsanaFunction.GetFunction(Function.AddProjectToTask), project);
        }

        public Task<SaveAsanaTask> AddProjectAsync(AsanaProject proj, long after = 0, long before = 0, long section = 0)
        {
            if (Host == null)
                throw new NullReferenceException(
                    "This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddProjectAsync(proj, Host, after, before, section);
        }


        public Task<SaveAsanaTask> AddSubTaskAsync(SaveAsanaTask task)
        {
            if (Host == null)
                throw new NullReferenceException(
                    "This AsanaObject does not have a host associated with it so you must specify one when saving.");
            return AddSubTaskAsync(task, Host);
        }

        public Task<SaveAsanaTask> AddSubTaskAsync(SaveAsanaTask task, Asana host)
        {
            var notes = string.IsNullOrWhiteSpace(task.Notes) ? string.Empty : task.Notes;
            var data = new Dictionary<string, object>
            {
                { "name", task.Name },
                { "notes", notes },
                { "start_at", task.StartAt },
                { "due_at", task.DueAt }
            };

            return host.SaveAsync(this, host.AsanaFunction.GetFunction(Function.AddSubTaskToTask), data);
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
        public bool? Completed { get; set; }

        [AsanaDataAttribute("completed_at", SerializationFlags.Omit)]
        public AsanaDateTime CompletedAt { get; protected set; }

        [AsanaDataAttribute("num_subtasks", SerializationFlags.ReadOnly)]
        public int SubTaskCount { get; set; }


        [AsanaDataAttribute("start_on", SerializationFlags.ReadOnly)]
        public AsanaDateTime StartOn
        {
            get => _startOn;
            set
            {
                // force save of due_at
                if (_dueOn != null)
                    SetPropertyChanged(nameof(DueOn));

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

        [AsanaDataAttribute("start_at", SerializationFlags.Optional)]
        public AsanaDateTime StartAt
        {
            get => _startAt;
            set
            {
                // force save of due_at
                if (_dueAt != null)
                    SetPropertyChanged(nameof(DueAt));

                if (_dueAt != null && _dueAt < value)
                {
                    _startAt = new AsanaDateTime(_dueAt.DateTime.Date);
                }
                else
                {
                    _startAt = value;
                }
            }
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
        }


        public void PushTimeRange(int days)
        {
            var start = StartAt ?? StartOn;
            var end = DueAt ?? DueOn;

            SetTimeRange(start.DateTime.AddDays(days), end.DateTime.AddDays(days));
        }
        public void SetTimeRange(DateTime start, DateTime end)
        {
            if (start < end)
            {
                DueAt = end;
                StartAt = start;
            }
            else
            {
                DueAt = end;
                StartAt = end.Date.Subtract(TimeSpan.FromDays(1));
            }

            SetPropertyChanged(nameof(DueAt));
            SetPropertyChanged(nameof(StartAt));
        }


        [AsanaDataAttribute("dependents", SerializationFlags.Optional)]
        public AsanaDependent[] Dependents { get; set; }

        [AsanaDataAttribute("dependencies", SerializationFlags.Optional)]
        public AsanaDependent[] Dependencies { get; set; }


        [AsanaDataAttribute("followers", SerializationFlags.Optional, "ID")]
        public AsanaUser[] Followers { get; set; }

        [AsanaDataAttribute("modified_at", SerializationFlags.ReadOnly)]
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


        public Task AddProject(AsanaProject proj)
        {
            CheckHostAndKey();
            return AddProject(proj, Host);
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
            return host.Save(this, host.AsanaFunction.GetFunction(Function.AddProjectToTask), project);
        }


        public Task RemoveProject(AsanaProject proj)
        {
            CheckHostAndKey();
            return RemoveProject(proj, Host);
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
            return host.Save(this, host.AsanaFunction.GetFunction(Function.RemoveProjectFromTask), project);
        }


        public void SetTags(AsanaTag[] tags)
        {
            Tags = tags;
        }

        public Task AddTag(AsanaTag proj)
        {
            CheckHostAndKey();
            return AddTag(proj, Host);
        }

        public Task AddTag(AsanaTag proj, Asana host)
        {
            var data = new Dictionary<string, object> { { "tag", proj.ID.ToString() } };

            void Callback(AsanaObject s)
            {
                // add it manually
                if (Tags == null)
                    Tags = new AsanaTag[1];
                else
                {
                    var lTags = Tags;
                    Array.Resize(ref lTags, Tags.Length + 1);
                    Tags = lTags;
                }

                Tags[^1] = proj;
                Saving -= Callback;
            }

            Saving += Callback;
            return host.Save(this, host.AsanaFunction.GetFunction(Function.AddTagToTask), data);
        }

        #region Tags

        public Task<AsanaTask> AddTagAsync(AsanaTag tag)
        {
            CheckHostAndKey();
            return AddTagAsync(tag, Host);
        }

        public Task<AsanaTask> AddTagAsync(AsanaTag tag, Asana host)
        {
            var data = new Dictionary<string, object> { { "tag", tag.ID.ToString() } };

            return host.SaveAsync<AsanaTask>(this as AsanaTask, host.AsanaFunction.GetFunction(Function.AddTagToTask), data);
        }


        public Task RemoveTag(AsanaTag proj)
        {
            CheckHostAndKey();
            return RemoveTag(proj, Host);
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
            return host.Save(this, host.AsanaFunction.GetFunction(Function.RemoveTagFromTask), Tag);
        }
        #endregion

        #region Followers

        public Task<AsanaObject> AddFollowersAsync(IEnumerable<AsanaUser> followers)
        {
            CheckHostAndKey();
            return AddFollowersAsync(followers, Host);
        }


        public Task<AsanaObject> AddFollowersAsync(IEnumerable<AsanaUser> followers, Asana host)
        {
            var i = 0;
            var data = followers.ToDictionary<AsanaUser, string, object>(asanaUser => $"followers[{i++}]", asanaUser => asanaUser.ID.ToString());

            return host.SaveAsync<AsanaObject>(this as AsanaTask, host.AsanaFunction.GetFunction(Function.AddFollowersToTask), data);
        }

        public Task<AsanaObject> RemoveFollowersAsync(IEnumerable<AsanaUser> followers)
        {
            CheckHostAndKey();
            return RemoveFollowersAsync(followers, Host);
        }

        public Task<AsanaObject> RemoveFollowersAsync(IEnumerable<AsanaUser> followers, Asana host)
        {
            var i = 0;
            var data = followers.ToDictionary<AsanaUser, string, object>(asanaUser => $"followers[{i++}]", asanaUser => asanaUser.ID.ToString());

            return host.SaveAsync<AsanaObject>(this as AsanaTask, host.AsanaFunction.GetFunction(Function.RemoveFollowersFromTask), data);
        }
        #endregion

        #region Dependents

        public async Task<List<AsanaDependent>> GetDependentsAsync()
        {
            CheckHostAndKey();

            var result = await Host.GetTaskDependentsAsync(this as AsanaTask);
            this.Dependents = result.ToArray();
            return result.ToList();
        }
        public async Task<List<AsanaDependent>> GetDependentsAsync(Asana host)
        {
            CheckHostAndKey();

            var result = await host.GetTaskDependentsAsync(this as AsanaTask);
            this.Dependents = result.ToArray();
            return result.ToList();
        }        

        public async Task<List<AsanaDependent>> GetDependenciesAsync()
        {
            CheckHostAndKey();

            var result = await Host.GetTaskDependenciesAsync(this as AsanaTask);
            this.Dependencies = result.ToArray();
            return result.ToList();
        }
        public async Task<List<AsanaDependent>> GetDependenciesAsync(Asana host)
        {
            CheckHostAndKey();

            var result = await host.GetTaskDependenciesAsync(this as AsanaTask);
            this.Dependencies = result.ToArray();
            return result.ToList();
        }


        public async Task<AsanaTask> SetDependentsAsync(IEnumerable<long> dependentIds)
        {
            CheckHostAndKey();

            return await SetDependentsAsync(dependentIds, Host);
        }
        public async Task<AsanaTask> SetDependentsAsync(IEnumerable<long> dependentIds, Asana host)
        {
            var i = 0;
            var newDependents = dependentIds.Select(x => new AsanaDependent(x, "task")).ToList();
            var data = newDependents
                .ToDictionary<AsanaDependent, string, object>(asanaDependent => $"dependents[{i++}]",
                    asanaDependent => asanaDependent.ID.ToString());

            var setResult = await host.SaveCollectionAsync<AsanaTask, AsanaReference>(this as AsanaTask,
                host.AsanaFunction.GetFunction(Function.SetTaskDependents), data);

            var dependentsList = Dependents?.ToList() ?? new List<AsanaDependent>();
            dependentsList.AddRange(newDependents);
            Dependents = dependentsList.ToArray();

            return this as AsanaTask;
        }

        public Task<AsanaTask> UnlinkDependentsAsync(IEnumerable<long> dependentIds)
        {
            CheckHostAndKey();

            return UnlinkDependentsAsync(dependentIds, Host);
        }
        public async Task<AsanaTask> UnlinkDependentsAsync(IEnumerable<long> dependentIds, Asana host)
        {
            var i = 0;
            var removers = dependentIds.Select(id => new AsanaDependent(id, "task")).ToList();

            var data = removers
                .ToDictionary<AsanaDependent, string, object>(asanaDependent => $"dependents[{i++}]",
                    asanaDependent => asanaDependent.ID.ToString());

            var unlinkResult = await host.SaveCollectionAsync<AsanaTask, AsanaReference>(this as AsanaTask,
                host.AsanaFunction.GetFunction(Function.UnlinkTaskDependents), data);

            var dependentsList = Dependents?.ToList() ?? new List<AsanaDependent>();
            dependentsList.RemoveAll(x => removers.Any(y => y.ID == x.ID));
            Dependents = dependentsList.ToArray();

            return this as AsanaTask;
        }



        public async Task<AsanaTask> SetDependenciesAsync(IEnumerable<long> dependenciesIds)
        {
            CheckHostAndKey();

            return await SetDependenciesAsync(dependenciesIds, Host);
        }
        public async Task<AsanaTask> SetDependenciesAsync(IEnumerable<long> dependenciesIds, Asana host)
        {
            var i = 0;
            var newDependents = dependenciesIds.Select(x => new AsanaDependent(x, "task")).ToList();
            var data = newDependents
                .ToDictionary<AsanaDependent, string, object>(asanaDependent => $"dependencies[{i++}]",
                    asanaDependent => asanaDependent.ID.ToString());

            var setResult = await host.SaveCollectionAsync<AsanaTask, AsanaReference>(this as AsanaTask,
                host.AsanaFunction.GetFunction(Function.SetTaskDependencies), data);

            var dependentsList = Dependents?.ToList() ?? new List<AsanaDependent>();
            dependentsList.AddRange(newDependents);
            Dependents = dependentsList.ToArray();

            return this as AsanaTask;
        }

        public Task<AsanaTask> UnlinkDependenciesAsync(IEnumerable<long> dependenciesIds)
        {
            CheckHostAndKey();

            return UnlinkDependenciesAsync(dependenciesIds, Host);
        }
        public async Task<AsanaTask> UnlinkDependenciesAsync(IEnumerable<long> dependenciesIds, Asana host)
        {
            var i = 0;
            var removers = dependenciesIds.Select(x => new AsanaDependent(x, "task")).ToList();

            var data = removers
                .ToDictionary<AsanaDependent, string, object>(asanaDependent => $"dependencies[{i++}]",
                    asanaDependent => asanaDependent.ID.ToString());

            var unlinkResult = await host.SaveCollectionAsync<AsanaTask, AsanaReference>(this as AsanaTask,
                host.AsanaFunction.GetFunction(Function.UnlinkTaskDependencies), data);

            var dependentsList = Dependents?.ToList() ?? new List<AsanaDependent>();
            dependentsList.RemoveAll(x => removers.Any(y => y.ID == x.ID));
            Dependents = dependentsList.ToArray();

            return this as AsanaTask;
        }

        #endregion




        public async Task<AsanaTask> SetParentAsync(long? parentId, long? after = null, long? before = null)
        {
            CheckHostAndKey();


            return await SetParentAsync(Host, parentId, after, before);
        }
        public async Task<AsanaTask> SetParentAsync(Asana host, long? parentId, long? after = null, long? before = null)
        {
            var data = new Dictionary<string, object> {
            { "parent", parentId },
            { "insert_after", after },
            { "insert_before", before },
            };

            var result = await host.SaveAsync<AsanaTask>(this as AsanaTask, host.AsanaFunction.GetFunction(Function.TaskSetParent), data);
            return result;
        }


        public async Task<AsanaTask> SetCustomFieldsAsync(IEnumerable<AsanaCustomField> customFields)
        {
            CheckHostAndKey();

            return await SetCustomFieldsAsync(Host, customFields);
        }
        public async Task<AsanaTask> SetCustomFieldsAsync(Asana host, IEnumerable<AsanaCustomField> customFields)
        {
            var newValues = customFields.ToList();
            var adders = newValues.Where(x => !CustomFields.Contains(x, AsanaCustomField.NameComparer)).ToList();
            if (!newValues.Any() && !CustomFields.Any())
                return this as AsanaTask;

            foreach (var customField in this.CustomFields)
            {
                var updateValue = newValues.FirstOrDefault(x => x.ID == customField.ID);
                if (updateValue == null)
                    continue;

                customField.SetValue(updateValue);
            }

            if (adders.Any())
            {
                var existing = CustomFields.ToList();
                foreach (var asanaCustomField in adders)
                {
                    existing.Add(asanaCustomField);
                }
                CustomFields = existing.ToArray();
            }

            SetPropertyChanged(nameof(this.CustomFields));
            var response = await this.SaveAsync<AsanaTask>(host);
            return response;
        }

        public string GetCustomFieldColor()
        {
            return CustomFields?.FirstOrDefault(x => x.Name == "Color")?.EnumValue?.Color;
        }

        public long GetAssigneeReservationId()
        {
            var idStr = CustomFields?.FirstOrDefault(x => x.Name == "AssigneeReservation")?.TextValue;
            var ok = long.TryParse(idStr, out var reservationId);

            return ok ? reservationId : 0;
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


        private sealed class NameEqualityComparer : IEqualityComparer<BaseAsanaTask>
        {
            public bool Equals(BaseAsanaTask x, BaseAsanaTask y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(BaseAsanaTask obj)
            {
                return (obj.Name != null ? obj.Name.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<BaseAsanaTask> NameComparer { get; } = new NameEqualityComparer();


        private void CheckHostAndKey()
        {
            if (Host == null)
                throw HostNullReferenceException();
            if (string.IsNullOrWhiteSpace(Host.APIKey))
                throw ApiKeyNullReferenceException();
        }
    }
}