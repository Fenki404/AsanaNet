using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsanaNet.Objects;

namespace AsanaNet.Interfaces
{
    public interface IAsana
    {
        /// <summary>
        /// The Authentication Type used for API access
        /// </summary>
        AuthenticationType AuthType { get; }

        /// <summary>
        /// The API Key assigned object
        /// </summary>
        string APIKey { get; }

        /// <summary>
        /// The API Key, but base-64 encoded
        /// </summary>
        string EncodedAPIKey { get; }

        /// <summary>
        /// The OAuth Bearer Token assigned object
        /// </summary>
        string OAuthToken { get; set; }

        Task GetUsers(AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaUser>> GetUsersAsync();
        Task GetMe(AsanaResponseEventHandler callback);
        Task<AsanaUser> GetMeAsync();
        Task GetUserById(Int64 int64, AsanaResponseEventHandler callback);
        Task<AsanaUser> GetUserByIdAsync(Int64 int64, Dictionary<string, object> args = null);
        Task GetWorkspaces(AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaWorkspace>> GetWorkspacesAsync(Dictionary<string, object> args = null);
        Task GetWorkspaceById(Int64 int64, AsanaResponseEventHandler callback);
        Task<AsanaWorkspace> GetWorkspaceByIdAsync(Int64 int64, Dictionary<string, object> args = null);
        Task GetUsersInWorkspace(AsanaWorkspace asanaWorkspace, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaUser>> GetUsersInWorkspaceAsync(AsanaWorkspace asanaWorkspace, Dictionary<string, object> args = null);
        Task GetTasksInWorkspace(AsanaWorkspace asanaWorkspace, AsanaUser asanaUser, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaTask>> GetTasksInWorkspaceAsync(AsanaWorkspace asanaWorkspace, AsanaUser asanaUser, Dictionary<string, object> args = null);
        Task GetProjectsInWorkspace(AsanaWorkspace asanaWorkspace, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaProject>> GetProjectsInWorkspaceAsync(AsanaWorkspace asanaWorkspace, Dictionary<string, object> args = null);
        Task GetTagsInWorkspace(AsanaWorkspace asanaWorkspace, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaTag>> GetTagsInWorkspaceAsync(AsanaWorkspace asanaWorkspace, Dictionary<string, object> args = null);
        Task GetTaskById(Int64 int64, AsanaResponseEventHandler callback);
        Task<AsanaTask> GetTaskByIdAsync(Int64 int64, Dictionary<string, object> args = null);
        Task GetStoriesInTask(AsanaTask asanaTask, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaStory>> GetStoriesInTaskAsync(AsanaTask asanaTask);
        Task GetProjectsOnATask(AsanaTask asanaTask, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaProject>> GetProjectsOnATaskAsync(AsanaTask asanaTask);
        Task GetTasksByTag(AsanaTag asanaTag, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaTask>> GetTasksByTagAsync(AsanaTag asanaTag);
        Task GetStoryById(Int64 int64, AsanaResponseEventHandler callback);
        Task<AsanaStory> GetStoryByIdAsync(Int64 int64);
        Task GetProjectById(Int64 int64, AsanaResponseEventHandler callback);
        Task GetJobById(Int64 int64, AsanaResponseEventHandler callback);
        Task GetTeamById(Int64 int64, AsanaResponseEventHandler callback);
        Task DuplicateProjectById(Int64 int64, AsanaDuplicateProjectSettings settings, AsanaResponseEventHandler callback);
        Task<AsanaProject> GetProjectByIdAsync(Int64 int64, Dictionary<string, object> args = null);
        Task GetTasksInAProject(AsanaProject asanaProject, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaTask>> GetTasksInAProjectAsync(AsanaProject asanaProject, Dictionary<string, object> args = null);
        Task GetTagById(Int64 int64, AsanaResponseEventHandler callback);
        Task<AsanaTag> GetTagByIdAsync(Int64 int64, Dictionary<string, object> args = null);
        Task GetTeamsInWorkspace(AsanaWorkspace asanaWorkspace, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaTeam>> GetTeamsInWorkspaceAsync(AsanaWorkspace asanaWorkspace, Dictionary<string, object> args = null);
        Task GetProjectsInTeam(AsanaTeam asanaTeam, AsanaCollectionResponseEventHandler callback);
        Task<IAsanaObjectCollection<AsanaProject>> GetProjectsInTeamAsync(AsanaTeam asanaTeam, Dictionary<string, object> args = null);
        Task GetEventsInAProject(AsanaProject asanaProject, String string1, AsanaResponseEventHandler callback);
        Task<AsanaEvent> GetEventsInAProjectAsync(AsanaProject asanaProject, String string1, Dictionary<string, object> args = null);
        Task GetEventsInTask(AsanaTask asanaTask, String string1, AsanaResponseEventHandler callback);
        Task<AsanaEvent> GetEventsInTaskAsync(AsanaTask asanaTask, String string1, Dictionary<string, object> args = null);

        Task Get<AsanaT>(AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(AsanaResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(Int64 arg1, AsanaResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(AsanaWorkspace arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(AsanaWorkspace arg1,  AsanaUser arg2, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(AsanaTask arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(AsanaTag arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(AsanaProject arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(AsanaTeam arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(AsanaProject arg1,  String arg2, AsanaResponseEventHandler callback) where AsanaT : AsanaObject;
        Task Get<AsanaT>(AsanaTask arg1,  String arg2, AsanaResponseEventHandler callback) where AsanaT : AsanaObject;
    }
}