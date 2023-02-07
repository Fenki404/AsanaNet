﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsanaNet.Objects;

/*
* THIS FILE IS GENERATED! DO NOT EDIT!
* REFER TO AsanaFunctionDefinitions.xml
*/
namespace AsanaNet
{
    // Enums for all functions
    public enum Function
    {
        GetUsers,
        GetMe,
        GetUserById,
        GetWorkspaces,
        GetWorkspaceById,
        GetUsersInWorkspace,
        GetProjectsInWorkspace,
        GetTagsInWorkspace,

        GetTaskById,
        GetTasksInWorkspace,
        GetTasksBySearch,
        GetTasksInAProject,
        GetTasksInASection,
        GetTasksByTag,
        GetTasksByAssignee,
        GetSubTasksInATask,
        GetDependenciesTasks,
        DuplicateTask,
        GetTaskDependencies,
        SetTaskDependencies,
        UnlinkTaskDependencies,
        GetTaskDependents,
        SetTaskDependents,
        UnlinkTaskDependents,
        AddTaskToSection,
        AddSubTaskToTask,
        AddProjectToTask,
        TaskSetParent,
        RemoveProjectFromTask,
        AddStoryToTask,
        AddTagToTask,
        RemoveTagFromTask,
        AddFollowersToTask,
        RemoveFollowersFromTask,
        UpdateTask,
        DeleteTask,
        GetStoriesInTask,
        GetProjectsOnATask,

        GetStoryById,
        GetProjectById,
        GetJobById,
        GetTeamById,
        GetSection,
        GetSectionById,
        GetSectionsInAProject,
        
        CreateSectionInAProject,
        UpdateSection,
        DeleteSection,
        GetTagById,
        GetTeamsInWorkspace,
        GetProjectsInTeam,
        GetEventsInAProject,
        GetEventsInTask,
        CreateWorkspaceTask,

        CreateWorkspaceProject,
        CreateWorkspaceTag,
        UpdateTag,
        UpdateProject,
        UpdateWorkspace,
        DeleteProject,
        DuplicateProject,
        AddCustomFieldToProject,
        AddUsersToProject,
        RemoveUsersFromProject,
        AddFollowersToProject,
        RemoveFollowerFromProject,
    }

    // Function definitions specifically for the GET functions.
    public partial class Asana
    {
        public Task GetUsers(AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetUsers));
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaUser>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaUser>> GetUsersAsync()
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetUsers));
            return request.GoCollectionAsync<AsanaUser>();
        }

        public Task GetMe(AsanaResponseEventHandler callback)
        {

            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetMe));
            return request.Go((o, h) => PackAndSendResponse<AsanaUser>(o, callback), ErrorCallback);
        }

        public Task<AsanaUser> GetMeAsync()
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetMe));
            return request.GoAsync<AsanaUser>();
        }

        public Task GetUserById(long int64, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetUserById), int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaUser>(o, callback), ErrorCallback);
        }

        public Task<AsanaUser> GetUserByIdAsync(long int64, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetUserById), args, int64);
            return request.GoAsync<AsanaUser>();
        }

        public Task GetWorkspaces(AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetWorkspaces));
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaWorkspace>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaWorkspace>> GetWorkspacesAsync(Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetWorkspaces), args);
            return request.GoCollectionAsync<AsanaWorkspace>(GoCollectionMaxRecursiveCount);
        }

        public Task GetWorkspaceById(long int64, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetWorkspaceById), int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaWorkspace>(o, callback), ErrorCallback);
        }

        public Task<AsanaWorkspace> GetWorkspaceByIdAsync(long int64, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetWorkspaceById), args, int64);
            return request.GoAsync<AsanaWorkspace>();
        }

        public Task GetUsersInWorkspace(AsanaWorkspace asanaWorkspace, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetUsersInWorkspace), asanaWorkspace);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaUser>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaUser>> GetUsersInWorkspaceAsync(AsanaWorkspace asanaWorkspace, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetUsersInWorkspace), args, asanaWorkspace);
            return request.GoCollectionAsync<AsanaUser>(GoCollectionMaxRecursiveCount);
        }

        public Task GetTasksInWorkspace(AsanaWorkspace asanaWorkspace, AsanaUser asanaUser, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTasksInWorkspace), asanaWorkspace, asanaUser);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaTask>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaTask>> GetTasksInWorkspaceAsync(AsanaWorkspace asanaWorkspace, AsanaUser asanaUser, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetTasksInWorkspace), args, asanaWorkspace, asanaUser);
            return request.GoCollectionAsync<AsanaTask>(GoCollectionMaxRecursiveCount);
        }

        public Task<IAsanaObjectCollection<AsanaTask>> GetTasksBySearchAsync(AsanaWorkspace asanaWorkspace, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetTasksBySearch), args, asanaWorkspace);
            return request.GoCollectionAsync<AsanaTask>(GoCollectionMaxRecursiveCount);
        }

        public Task GetProjectsInWorkspace(AsanaWorkspace asanaWorkspace, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetProjectsInWorkspace), asanaWorkspace);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaProject>(o, callback), ErrorCallback);
        }



        public Task<IAsanaObjectCollection<AsanaProject>> GetProjectsInWorkspaceAsync(AsanaWorkspace asanaWorkspace, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetProjectsInWorkspace), args, asanaWorkspace);
            return request.GoCollectionAsync<AsanaProject>(GoCollectionMaxRecursiveCount);
        }

        public Task GetTagsInWorkspace(AsanaWorkspace asanaWorkspace, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTagsInWorkspace), asanaWorkspace);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaTag>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaTag>> GetTagsInWorkspaceAsync(AsanaWorkspace asanaWorkspace, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetTagsInWorkspace), args, asanaWorkspace);
            return request.GoCollectionAsync<AsanaTag>(GoCollectionMaxRecursiveCount);
        }

        public Task GetTaskById(long int64, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTaskById), int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaTask>(o, callback), ErrorCallback);
        }

        public Task<AsanaTask> GetTaskByIdAsync(long int64, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetTaskById), args, int64);
            return request.GoAsync<AsanaTask>();
        }



        #region Section

        public Task<AsanaSection> CreateSectionInAProjectAsync(long projectId, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.CreateSectionInAProject), args, projectId);
            return request.GoAsync<AsanaSection>();
        }

        public Task<AsanaSection> GetSectionByIdAsync(long id, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetSection), args, id);
            return request.GoAsync<AsanaSection>();
        }


        public Task<IAsanaObjectCollection<AsanaSection>> GetSectionsInAProjectAsync(long sectionId, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetSectionsInAProject), args, sectionId);
            return request.GoCollectionAsync<AsanaSection>(GoCollectionMaxRecursiveCount);
        }

        public Task<AsanaSection> AddTaskToSectionAsync(long sectionId, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.AddTaskToSection), args, sectionId);
            return request.GoAsync<AsanaSection>();
        }

        #endregion



        public Task GetStoriesInTask(AsanaTask asanaTask, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetStoriesInTask), asanaTask);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaStory>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaStory>> GetStoriesInTaskAsync(AsanaTask asanaTask)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetStoriesInTask), asanaTask);
            return request.GoCollectionAsync<AsanaStory>(GoCollectionMaxRecursiveCount);
        }

        public Task GetProjectsOnATask(AsanaTask asanaTask, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetProjectsOnATask), asanaTask);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaProject>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaProject>> GetProjectsOnATaskAsync(AsanaTask asanaTask)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetProjectsOnATask), asanaTask);
            return request.GoCollectionAsync<AsanaProject>(GoCollectionMaxRecursiveCount);
        }

        public Task GetStoryById(long int64, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetStoryById), int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaStory>(o, callback), ErrorCallback);
        }

        public Task<AsanaStory> GetStoryByIdAsync(long int64)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetStoryById), int64);
            return request.GoAsync<AsanaStory>();
        }

        public Task GetProjectById(long int64, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetProjectById), int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaProject>(o, callback), ErrorCallback);
        }

        public Task GetJobById(long int64, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetJobById), int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaJob>(o, callback), ErrorCallback);
        }

        public Task GetTeamById(long int64, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTeamById), int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaTeam>(o, callback), ErrorCallback);
        }

        public Task DuplicateProjectById(long int64, AsanaDuplicateProjectSettings settings, AsanaResponseEventHandler callback)
        {
            var data = Parsing.Serialize(settings, true, false);
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.DuplicateProject), data, int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaDuplicateProjectJob>(o, callback), ErrorCallback);
        }

        public Task<AsanaProject> GetProjectByIdAsync(long int64, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetProjectById), args, int64);
            return request.GoAsync<AsanaProject>();
        }


        #region TASK

        public Task GetTasksInAProject(AsanaProject asanaProject, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTasksInAProject), asanaProject);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaTask>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaTask>> GetTasksInAProjectAsync(AsanaProject asanaProject, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetTasksInAProject), args, asanaProject);
            return request.GoCollectionAsync<AsanaTask>(GoCollectionMaxRecursiveCount);
        }
        public Task<IAsanaObjectCollection<AsanaTask>> GetTasksInASectionAsync(AsanaSection section, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetTasksInASection), args, section);
            return request.GoCollectionAsync<AsanaTask>(GoCollectionMaxRecursiveCount);
        }

        public Task<IAsanaObjectCollection<AsanaTask>> GetSubTasksInATaskAsync(AsanaTask asanaTask, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetSubTasksInATask), args, asanaTask);
            return request.GoCollectionAsync<AsanaTask>(GoCollectionMaxRecursiveCount);
        }

        public Task<IAsanaObjectCollection<AsanaTask>> GetDependenciesTasksAsync(AsanaTask asanaTask, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetDependenciesTasks), args, asanaTask);
            return request.GoCollectionAsync<AsanaTask>(GoCollectionMaxRecursiveCount);
        }

        public Task GetTasksByTag(AsanaTag asanaTag, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTasksByTag), asanaTag);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaTask>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaTask>> GetTasksByTagAsync(AsanaTag asanaTag)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTasksByTag), asanaTag);
            return request.GoCollectionAsync<AsanaTask>(GoCollectionMaxRecursiveCount);
        }

        public Task DuplicateTaskById(long int64, AsanaDuplicateTaskSettings settings, AsanaResponseEventHandler callback)
        {
            var data = Parsing.Serialize(settings, true, false);
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.DuplicateTask), data, int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaDuplicateTaskJob>(o, callback), ErrorCallback);
        }
        public Task<AsanaDuplicateTaskJob> DuplicateTaskByIdAsync(long int64, AsanaDuplicateTaskSettings settings)
        {
            var data = Parsing.Serialize(settings, true, false);
            var request = GetBaseRequestWithParamsJson(AsanaFunction.GetFunction(Function.DuplicateTask), data, int64);
            var task = request.GoAsync<AsanaDuplicateTaskJob>();
            return task;
        }


        public Task<IAsanaObjectCollection<AsanaDependent>> GetTaskDependentsAsync(AsanaTask task)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTaskDependents), task);
            return request.GoCollectionAsync<AsanaDependent>(GoCollectionMaxRecursiveCount);
        }
        public Task<IAsanaObjectCollection<AsanaDependent>> GetTaskDependenciesAsync(AsanaTask task)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTaskDependencies), task);
            return request.GoCollectionAsync<AsanaDependent>(GoCollectionMaxRecursiveCount);
        }

        #endregion



        public Task GetTagById(long int64, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTagById), int64);
            return request.Go((o, h) => PackAndSendResponse<AsanaTag>(o, callback), ErrorCallback);
        }

        public Task<AsanaTag> GetTagByIdAsync(long int64, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetTagById), args, int64);
            return request.GoAsync<AsanaTag>();
        }

        public Task GetTeamsInWorkspace(AsanaWorkspace asanaWorkspace, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTeamsInWorkspace), asanaWorkspace);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaTeam>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaTeam>> GetTeamsInWorkspaceAsync(AsanaWorkspace asanaWorkspace, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetTeamsInWorkspace), args, asanaWorkspace);
            return request.GoCollectionAsync<AsanaTeam>(GoCollectionMaxRecursiveCount);
        }

        public Task GetProjectsInTeam(AsanaTeam asanaTeam, AsanaCollectionResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetProjectsInTeam), asanaTeam);
            return request.Go((o, h) => PackAndSendResponseCollection<AsanaProject>(o, callback), ErrorCallback);
        }

        public Task<IAsanaObjectCollection<AsanaProject>> GetProjectsInTeamAsync(AsanaTeam asanaTeam, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetProjectsInTeam), args, asanaTeam);
            return request.GoCollectionAsync<AsanaProject>(GoCollectionMaxRecursiveCount);
        }

        public Task GetEventsInAProject(AsanaProject asanaProject, string string1, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetEventsInAProject), asanaProject, string1);
            return request.Go((o, h) => PackAndSendResponse<AsanaEvent>(o, callback), ErrorCallback);
        }

        public Task<AsanaEvent> GetEventsInAProjectAsync(AsanaProject asanaProject, string string1, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetEventsInAProject), args, asanaProject, string1);
            return request.GoAsync<AsanaEvent>(true);
        }

        public Task GetEventsInTask(AsanaTask asanaTask, string string1, AsanaResponseEventHandler callback)
        {
            var request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetEventsInTask), asanaTask, string1);
            return request.Go((o, h) => PackAndSendResponse<AsanaEvent>(o, callback), ErrorCallback);
        }

        public Task<AsanaEvent> GetEventsInTaskAsync(AsanaTask asanaTask, string string1, Dictionary<string, object> args = null)
        {
            var request = GetBaseRequestWithParams(AsanaFunction.GetFunction(Function.GetEventsInTask), args, asanaTask, string1);
            return request.GoAsync<AsanaEvent>(true);
        }




        public Task Get<AsanaT>(AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaUser))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetUsers));
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaUser>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaWorkspace))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetWorkspaces));
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaWorkspace>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(AsanaResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaUser))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetMe));
                return request.Go((o, h) => PackAndSendResponse<AsanaUser>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(long arg1, AsanaResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaUser))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetUserById), arg1);
                return request.Go((o, h) => PackAndSendResponse<AsanaUser>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaWorkspace))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetWorkspaceById), arg1);
                return request.Go((o, h) => PackAndSendResponse<AsanaWorkspace>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaTask))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTaskById), arg1);
                return request.Go((o, h) => PackAndSendResponse<AsanaTask>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaStory))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetStoryById), arg1);
                return request.Go((o, h) => PackAndSendResponse<AsanaStory>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaProject))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetProjectById), arg1);
                return request.Go((o, h) => PackAndSendResponse<AsanaProject>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaTag))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTagById), arg1);
                return request.Go((o, h) => PackAndSendResponse<AsanaTag>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(AsanaWorkspace arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaUser))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetUsersInWorkspace), arg1);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaUser>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaProject))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetProjectsInWorkspace), arg1);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaProject>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaTag))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTagsInWorkspace), arg1);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaTag>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaTeam))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTeamsInWorkspace), arg1);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaTeam>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(AsanaWorkspace arg1, AsanaUser arg2, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaTask))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTasksInWorkspace), arg1, arg2);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaTask>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(AsanaTask arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaStory))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetStoriesInTask), arg1);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaStory>(o, callback), ErrorCallback);
            }


            if (typeof(AsanaT) == typeof(AsanaProject))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetProjectsOnATask), arg1);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaProject>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(AsanaTag arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaTask))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTasksByTag), arg1);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaTask>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(AsanaProject arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaTask))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetTasksInAProject), arg1);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaTask>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(AsanaTeam arg1, AsanaCollectionResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaProject))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetProjectsInTeam), arg1);
                return request.Go((o, h) => PackAndSendResponseCollection<AsanaProject>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(AsanaProject arg1, string arg2, AsanaResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaEvent))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetEventsInAProject), arg1, arg2);
                return request.Go((o, h) => PackAndSendResponse<AsanaEvent>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }


        public Task Get<AsanaT>(AsanaTask arg1, string arg2, AsanaResponseEventHandler callback) where AsanaT : AsanaObject
        {
            AsanaRequest request = default(AsanaRequest);

            if (typeof(AsanaT) == typeof(AsanaEvent))
            {
                request = GetBaseRequest(AsanaFunction.GetFunction(Function.GetEventsInTask), arg1, arg2);
                return request.Go((o, h) => PackAndSendResponse<AsanaEvent>(o, callback), ErrorCallback);
            }


            throw new TypeAccessException("Unknown type for this request: " + typeof(AsanaT).Name);
        }

    }

    // Binds the enums, formations and methods
    public partial class AsanaFunction
    {
        internal void InitFunctions()
        {
            if (_functions.Any() || _associations.Any())
                return;

            _functions.Add(Function.GetUsers, new AsanaFunction("/users", "GET"));
            _functions.Add(Function.GetMe, new AsanaFunction("/users/me", "GET"));
            _functions.Add(Function.GetUserById, new AsanaFunction("/users/{0}", "GET"));
            _functions.Add(Function.GetWorkspaces, new AsanaFunction("/workspaces", "GET"));
            _functions.Add(Function.GetWorkspaceById, new AsanaFunction("/workspaces/{0}", "GET"));
            _functions.Add(Function.GetUsersInWorkspace, new AsanaFunction("/workspaces/{0:ID}/users", "GET"));
            _functions.Add(Function.GetTasksInWorkspace, new AsanaFunction("/workspaces/{0:ID}/tasks?assignee={1:ID}", "GET"));
            _functions.Add(Function.GetTasksBySearch, new AsanaFunction("/workspaces/{0:ID}/tasks/search", "GET"));
            _functions.Add(Function.GetProjectsInWorkspace, new AsanaFunction("/workspaces/{0:ID}/projects", "GET"));
            _functions.Add(Function.GetTagsInWorkspace, new AsanaFunction("/workspaces/{0:ID}/tags", "GET"));

            _functions.Add(Function.GetSection, new AsanaFunction("/sections/{0}", "GET"));
            _functions.Add(Function.GetSectionsInAProject, new AsanaFunction("/projects/{0}/sections", "GET"));
            _functions.Add(Function.CreateSectionInAProject, new AsanaFunction("/projects/{0:Target}/sections", "POST"));
            _functions.Add(Function.UpdateSection, new AsanaFunction("/sections/{0}", "PUT"));
            _functions.Add(Function.DeleteSection, new AsanaFunction("/sections/{0}", "DELETE"));

            _functions.Add(Function.GetTaskById, new AsanaFunction("/tasks/{0}", "GET"));
            _functions.Add(Function.GetStoriesInTask, new AsanaFunction("/tasks/{0:ID}/stories", "GET"));
            _functions.Add(Function.GetProjectsOnATask, new AsanaFunction("/tasks/{0:ID}/projects", "GET"));
            _functions.Add(Function.GetTasksInAProject, new AsanaFunction("/projects/{0:ID}/tasks", "GET"));
            _functions.Add(Function.GetTasksInASection, new AsanaFunction("/sections/{0:ID}/tasks", "GET"));
            _functions.Add(Function.GetSubTasksInATask, new AsanaFunction("/tasks/{0:ID}/subtasks", "GET"));
            _functions.Add(Function.GetDependenciesTasks, new AsanaFunction("/tasks/{0:ID}/dependencies", "GET"));
            _functions.Add(Function.GetEventsInTask, new AsanaFunction("/tasks/{0:ID}/events?sync={1}", "GET"));

            _functions.Add(Function.TaskSetParent, new AsanaFunction("/tasks/{0}/setParent", "POST"));
            _functions.Add(Function.AddSubTaskToTask, new AsanaFunction("/tasks/{0:ID}/subtasks", "POST"));
            _functions.Add(Function.CreateWorkspaceTask, new AsanaFunction("/tasks", "POST"));
            _functions.Add(Function.AddTaskToSection, new AsanaFunction("/sections/{0:Target}/addTask", "POST"));
            _functions.Add(Function.AddProjectToTask, new AsanaFunction("/tasks/{0:ID}/addProject", "POST"));
            _functions.Add(Function.RemoveProjectFromTask, new AsanaFunction("/tasks/{0:ID}/removeProject", "POST"));
            _functions.Add(Function.AddStoryToTask, new AsanaFunction("/tasks/{0:Target}/stories", "POST"));
            _functions.Add(Function.AddTagToTask, new AsanaFunction("/tasks/{0:ID}/addTag", "POST"));
            _functions.Add(Function.RemoveTagFromTask, new AsanaFunction("/tasks/{0:ID}/removeTag", "POST"));
            _functions.Add(Function.AddFollowersToTask, new AsanaFunction("/tasks/{0:ID}/addFollowers", "POST"));
            _functions.Add(Function.RemoveFollowersFromTask, new AsanaFunction("/tasks/{0:ID}/removeFollowers", "POST"));
            _functions.Add(Function.GetTasksByTag, new AsanaFunction("/tags/{0:ID}/tasks", "GET"));
            _functions.Add(Function.DuplicateTask, new AsanaFunction("/tasks/{0}/duplicate", "POST"));
            _functions.Add(Function.GetTaskDependents, new AsanaFunction("/tasks/{0}/dependents", "GET"));
            _functions.Add(Function.SetTaskDependents, new AsanaFunction("/tasks/{0}/addDependents", "POST"));
            _functions.Add(Function.UnlinkTaskDependents, new AsanaFunction("/tasks/{0}/removeDependents", "POST"));
            _functions.Add(Function.GetTaskDependencies, new AsanaFunction("/tasks/{0}/dependencies", "GET"));
            _functions.Add(Function.SetTaskDependencies, new AsanaFunction("/tasks/{0}/addDependencies", "POST"));
            _functions.Add(Function.UnlinkTaskDependencies, new AsanaFunction("/tasks/{0}/removeDependencies", "POST"));


            _functions.Add(Function.GetStoryById, new AsanaFunction("/stories/{0}", "GET"));
            _functions.Add(Function.GetProjectById, new AsanaFunction("/projects/{0}", "GET"));
            _functions.Add(Function.GetJobById, new AsanaFunction("/jobs/{0}", "GET"));
            _functions.Add(Function.GetTeamById, new AsanaFunction("/teams/{0}", "GET"));


            _functions.Add(Function.GetTagById, new AsanaFunction("/tags/{0}", "GET"));
            _functions.Add(Function.GetTeamsInWorkspace, new AsanaFunction("/organizations/{0:ID}/teams", "GET"));
            _functions.Add(Function.GetProjectsInTeam, new AsanaFunction("/teams/{0:ID}/projects", "GET"));
            _functions.Add(Function.GetEventsInAProject, new AsanaFunction("/projects/{0:ID}/events?sync={1}", "GET"));

            _functions.Add(Function.DuplicateProject, new AsanaFunction("/projects/{0}/duplicate", "POST"));
            _functions.Add(Function.CreateWorkspaceProject, new AsanaFunction("/projects", "POST"));
            _functions.Add(Function.CreateWorkspaceTag, new AsanaFunction("/tags", "POST"));

            _functions.Add(Function.UpdateTask, new AsanaFunction("/tasks/{0:ID}", "PUT"));
            _functions.Add(Function.UpdateTag, new AsanaFunction("/tags/{0:ID}", "PUT"));
            _functions.Add(Function.UpdateProject, new AsanaFunction("/projects/{0:ID}", "PUT"));
            _functions.Add(Function.UpdateWorkspace, new AsanaFunction("/workspaces/{0:ID}", "PUT"));

            _functions.Add(Function.DeleteTask, new AsanaFunction("/tasks/{0:ID}", "DELETE"));
            _functions.Add(Function.DeleteProject, new AsanaFunction("/projects/{0:ID}", "DELETE"));

            _functions.Add(Function.AddCustomFieldToProject, new AsanaFunction("/projects/{0}/addCustomFieldSetting", "POST"));

            _functions.Add(Function.AddUsersToProject, new AsanaFunction("/projects/{0}/addMembers", "POST"));
            _functions.Add(Function.RemoveUsersFromProject, new AsanaFunction("/projects/{0}/removeMembers", "POST"));
            _functions.Add(Function.AddFollowersToProject, new AsanaFunction("/projects/{0}/addFollowers", "POST"));
            _functions.Add(Function.RemoveFollowerFromProject, new AsanaFunction("/projects/{0}/removeFollowers", "POST"));



            _associations.Add(typeof(AsanaWorkspace), new AsanaFunctionAssociation(null, GetFunction(Function.UpdateWorkspace), null));
            _associations.Add(typeof(AsanaSection), new AsanaFunctionAssociation(GetFunction(Function.CreateSectionInAProject), GetFunction(Function.UpdateSection), GetFunction(Function.DeleteSection)));
            _associations.Add(typeof(AsanaSectionTask), new AsanaFunctionAssociation(GetFunction(Function.AddTaskToSection), null, null));

            _associations.Add(typeof(AsanaTask), new AsanaFunctionAssociation(GetFunction(Function.CreateWorkspaceTask), GetFunction(Function.UpdateTask), GetFunction(Function.DeleteTask)));
            _associations.Add(typeof(SaveAsanaTask), new AsanaFunctionAssociation(GetFunction(Function.CreateWorkspaceTask), GetFunction(Function.UpdateTask), GetFunction(Function.DeleteTask)));
            _associations.Add(typeof(AsanaProject), new AsanaFunctionAssociation(GetFunction(Function.CreateWorkspaceProject), GetFunction(Function.UpdateProject), GetFunction(Function.DeleteProject)));
            _associations.Add(typeof(AsanaTag), new AsanaFunctionAssociation(GetFunction(Function.CreateWorkspaceTag), GetFunction(Function.UpdateTag), null));
            _associations.Add(typeof(AsanaStory), new AsanaFunctionAssociation(GetFunction(Function.AddStoryToTask), null, null));

        }
    }
}