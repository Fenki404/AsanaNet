using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsanaNet.Objects;
using Microsoft.Extensions.Configuration;

namespace AsanaNet.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Third example of how to perform

            //ExecuteParallelAsync().Wait();
            //ExecuteAsync().Wait();
            //ExecuteWithTasks();
            //MonitorProjectChanges(733775454290030, TimeSpan.FromSeconds(5)).Wait();

            //GetProjectAsync(1200791760303935).Wait();
            //UpdateTaskAsync(1200844375334941).Wait();
            UpdateTaskAsync(1201353834582988).Wait();
            //https://app.asana.com/0/1200791760303935/1200844375334941/f
            //AddSection().Wait();

            //Console.ReadLine();
        }

        private static async Task MonitorProjectChanges(long projectId, TimeSpan interval)
        {
            var apiToken = GetApiToken();
            var asana = new Asana(apiToken, AuthenticationType.Basic, ErrorCallback);

            //var project = await asana.GetProjectByIdAsync(projectId);
            //var events = await asana.GetEventsInAProjectAsync(project, "8233e364b4a1a439d0ace299e825a47b:2");

            var lastToken = string.Empty;
            while (true)
            {
                var events = await asana.GetEventsInAProjectAsync(projectId, lastToken);
                lastToken = events.Sync;
                if (events.Data != null)
                    foreach (var item in events.Data)
                    {
                        Console.WriteLine($"{item.CreatedAt} - {item.Type}: {item.Action}");
                        if (item.Resource != null)
                        {
                            var line1 =
                                $"    {item.Resource.CreatedAt} - {item.Resource.Name} - {item.Resource.CreatedBy?.Name}";
                            var line2 = $"    {item.Resource.Type} - {item.Resource.Text}";
                            if (line1.Trim().Length > 2)
                                Console.WriteLine(line1);
                            if (line2.Trim().Length > 1)
                                Console.WriteLine(line2);
                        }
                    }
                Thread.Sleep(interval);
            }
        }



        private static async Task GetProjectAsync(long id)
        {
            // CONFIGURE YOUR ASANA API TOKEN IN APPSETTINGS.CONFIG FILE
            Console.WriteLine("# Asana - Async Method #");
            var apiToken = GetApiToken();
            var asana = new Asana(apiToken, AuthenticationType.Basic, ErrorCallback);

            var me = await asana.GetMeAsync();
            Console.WriteLine("Hello, " + me.Name);

            var startTime = DateTime.Now;

            var project = await asana.GetProjectByIdAsync(id, AsanaProject.SerializePropertiesToArgs());
            var tasks = await asana.GetTasksInAProjectAsync(project, AsanaTask.SerializePropertiesToArgs());
            Console.WriteLine($"    Project: {project.Name}  ID: {project.ID}    {tasks.Count()} Tasks");
            foreach (AsanaTask task in tasks)
                Console.WriteLine($"      Task: {task.Name}  {task.StartAt?.DateTime} -> {task.DueAt?.DateTime}");

            Console.WriteLine();
            Console.WriteLine("Execution time " + (DateTime.Now - startTime));
            Console.ReadLine();
        }

        private static async Task CreateTask()
        {
            // CONFIGURE YOUR ASANA API TOKEN IN APPSETTINGS.CONFIG FILE
            Console.WriteLine("# Asana - Async Method #");
            var apiToken = GetApiToken();
            var asana = new Asana(apiToken, AuthenticationType.Basic, ErrorCallback);

            var me = await asana.GetMeAsync();
            Console.WriteLine("Hello, " + me.Name);

            var startTime = DateTime.Now;
            var workSpaces = await asana.GetWorkspacesAsync();
            var first = workSpaces.First();

            var task = new AsanaTask(first)
            {
                Assignee = me,
                Name = "Test Task Christian"
            };
            var result = await task.SaveAsync<AsanaTask>(asana);

            Debug.WriteLine(result);
        }

        private static async Task AddSection()
        {
            // CONFIGURE YOUR ASANA API TOKEN IN APPSETTINGS.CONFIG FILE
            Console.WriteLine("# Asana - Async Method #");
            var apiToken = GetApiToken();
            var asana = new Asana(apiToken, AuthenticationType.Basic, ErrorCallback);

            var me = await asana.GetMeAsync();
            var ws = me.Workspaces.First();
            Console.WriteLine("Hello, " + me.Name);

            var query = new Dictionary<string, object>();
            query.Add("limit", 100);



            var sec = await asana.GetSectionByIdAsync(1200791760303948);

            //var projects = await asana.GetProjectsInWorkspaceAsync(ws, query);

            return;

            var properties = AsanaTask.SerializePropertiesToArgs();

            var projectId = 1200791760303935;
            var project = await asana.GetProjectByIdAsync(projectId, AsanaProject.SerializePropertiesToArgs());

            var proj = new AsanaProject(new AsanaWorkspace(), 123);

            var sections = await asana.GetSectionsInAProjectAsync(1200791760303935, AsanaSection.SerializePropertiesToArgs());


            var target = sections.First();

            var sectionTask = new AsanaSectionTask(target, "1200844375334941");
            var res = await sectionTask.SaveAsync<AsanaSectionTask>(asana);
            //await asana.AddTaskToSectionAsync(sectionId, adder);

            var tasksInSection = await asana.GetTasksInASectionAsync(target, AsanaTask.SerializePropertiesToArgs());

            Debug.WriteLine("Done");
            //var newSection = new AsanaSection("insert Section 1 2 3", project);
            //var result = await newSection.SaveAsync<AsanaSection>(asana);

            ////var task = await asana.GetTaskByIdAsync(1200844373558126, properties);

            //// await task.DeleteAsync();

            //Debug.WriteLine(result);

            //result.Name = "update Section";
            //result = await result.SaveAsync<AsanaSection>();

            //Debug.WriteLine(result);

            //await result.DeleteAsync();
            //Debug.WriteLine(result);

        }

        private static async Task RemoveTask()
        {
            // CONFIGURE YOUR ASANA API TOKEN IN APPSETTINGS.CONFIG FILE
            Console.WriteLine("# Asana - Async Method #");
            var apiToken = GetApiToken();
            var asana = new Asana(apiToken, AuthenticationType.Basic, ErrorCallback);

            var me = await asana.GetMeAsync();
            Console.WriteLine("Hello, " + me.Name);

            var properties = AsanaTask.SerializePropertiesToArgs();
            var task = await asana.GetTaskByIdAsync(1200844373558126, properties);

            // await task.DeleteAsync();

            Debug.WriteLine(task);
        }

        private static async Task UpdateTaskAsync(long id)
        {
            // CONFIGURE YOUR ASANA API TOKEN IN APPSETTINGS.CONFIG FILE
            Console.WriteLine("# Asana - Async Method #");
            var apiToken = GetApiToken();
            var asana = new Asana(apiToken, AuthenticationType.Basic, ErrorCallback);

            var me = await asana.GetMeAsync();
            Console.WriteLine("Hello, " + me.Name);

            var startTime = DateTime.Now;


            var properties = AsanaTask.SerializePropertiesToArgs();
            var task = await asana.GetTaskByIdAsync(id, properties);
            Console.WriteLine($"      Task: {task.Name}  {task.StartAt?.DateTime} -> {task.DueAt?.DateTime}");
            task.Notes = "updated Note";

            var newCF = new AsanaCustomField()
            {
                ResourceSubtype = "enum",
                EnumValue = task.CustomFields[2].EnumOptions[1]
            };
            task.CustomFields[2].SetValue(newCF);
            //task.CustomFields[2].SetEnumValue(null);

            //if (task.DueAt != null)
            //    task.DueAt.DateTime += TimeSpan.FromMinutes(10);
            //else
            //{
            //    task.DueAt = new AsanaDateTime(DateTime.Now);
            //}


            Console.WriteLine(task.CustomFields);

            //task.Assignee = null;

            var result = await task.SaveAsync<AsanaTask>();

            task = await asana.GetTaskByIdAsync(id, properties);
            Console.WriteLine($"      Task: {task.Name}  {task.StartAt?.DateTime} -> {task.DueAt?.DateTime}");


            Console.WriteLine();
            Console.WriteLine("Execution time " + (DateTime.Now - startTime));
            Console.ReadLine();
        }


        /// <summary>
        /// New API format
        /// </summary>
        /// <returns></returns>
        private static async Task ExecuteAsync()
        {
            // CONFIGURE YOUR ASANA API TOKEN IN APPSETTINGS.CONFIG FILE
            var startTime = DateTime.Now;
            Console.WriteLine("# Asana - Async Method #");
            var apiToken = GetApiToken();
            var asana = new Asana(apiToken, AuthenticationType.Basic, ErrorCallback);

            var me = await asana.GetMeAsync();
            Console.WriteLine("Hello, " + me.Name);

            var workspaces = await asana.GetWorkspacesAsync();
            foreach (var workspace in workspaces)
            {
                Console.WriteLine("Workspace: " + workspace.Name);

                var teams = await asana.GetTeamsInWorkspaceAsync(workspace);
                foreach (var team in teams)
                {
                    //                    if (team.Name != "Projetos Especiais")
                    //                        continue;

                    Console.WriteLine("  Team: " + team.Name);

                    // Projects
                    var projects = await asana.GetProjectsInTeamAsync(team);
                    foreach (AsanaProject project in projects)
                    {
                        Console.WriteLine("    Project: " + project.Name);

                        var tasks = await asana.GetTasksInAProjectAsync(project);
                        foreach (AsanaTask task in tasks)
                            Console.WriteLine("      Task: " + task.Name);
                    }
                }
            }


            Console.WriteLine();
            Console.WriteLine("Execution time " + (DateTime.Now - startTime));
            Console.ReadLine();
        }

        /// <summary>
        /// New API format - Parallel execution
        /// </summary>
        /// <returns></returns>
        private static async Task ExecuteParallelAsync()
        {
            // CONFIGURE YOUR ASANA API TOKEN IN APPSETTINGS.CONFIG FILE
            var startTime = DateTime.Now;
            Console.WriteLine("# Asana - Async Method #");
            var apiToken = GetApiToken();
            var asana = new Asana(apiToken, AuthenticationType.Basic, ErrorCallback);

            // Parallel tasks
            var meTask = asana.GetMeAsync();

            var workspaces = await asana.GetWorkspacesAsync();
            //            var workspacesConcurrentList = new ConcurrentQueue<AsanaWorkspace>(workspaces);
            var workspaceTasks = workspaces.Select(async workspace =>
            {
                var workSpaceInfo = new HierarchicalParallelExecutionData
                {
                    Info = "Workspace: " + workspace.Name,
                    Object = workspace
                };

                // Teams
                var teams = await asana.GetTeamsInWorkspaceAsync(workspace);
                //                var teamsConcurrentList = new ConcurrentQueue<AsanaTeam>(teams);
                var teamTasks = teams.Select(async team =>
                {
                    //                    if (team.Name != "Projetos Especiais")
                    //                        return;

                    var teamInfo = new HierarchicalParallelExecutionData
                    {
                        Info = "  Team: " + team.Name,
                        Object = team
                    };
                    workSpaceInfo.Items.Add(teamInfo);

                    // Projects
                    var projects = await asana.GetProjectsInTeamAsync(team);
                    //                    var projectsConcurrentList = new ConcurrentQueue<AsanaProject>(projects);
                    var projectTasks = projects.Select(async project =>
                    {
                        var projectInfo = new HierarchicalParallelExecutionData
                        {
                            Info = "    Project: " + project.Name,
                            Object = team
                        };
                        teamInfo.Items.Add(projectInfo);

                        // Taks
                        var tasks = await asana.GetTasksInAProjectAsync(project);
                        foreach (var task in tasks)
                        {
                            var taskInfo = new HierarchicalParallelExecutionData
                            {
                                Info = "      Task: " + task.Name,
                                Object = team
                            };
                            projectInfo.Items.Add(taskInfo);
                        }

                    });
                    await Task.WhenAll(projectTasks);
                });
                await Task.WhenAll(teamTasks);

                return workSpaceInfo;
            });
            var hierarchicalCall = await Task.WhenAll(workspaceTasks);

            var me = meTask.Result;
            Console.WriteLine("Hello, " + me.Name);

            foreach (var item in hierarchicalCall)
                item.WriteToConsole();

            Console.WriteLine();
            Console.WriteLine("Execution time " + (DateTime.Now - startTime));
            Console.ReadLine();
        }


        /// <summary>
        /// Old API format
        /// </summary>
        /// <returns></returns>
        private static void ExecuteWithTasks()
        {
            // CONFIGURE YOUR ASANA API TOKEN IN APPSETTINGS.CONFIG FILE
            var startTime = DateTime.Now;
            Console.WriteLine("# Asana - Task Method #");
            var apiToken = GetApiToken();
            var asana = new Asana(apiToken, AuthenticationType.Basic, ErrorCallback);

            asana.GetMe(response =>
            {
                var me = (AsanaUser)response;
                Console.WriteLine("Hello, " + me.Name);
            }).Wait();

            asana.GetWorkspaces(o =>
            {
                foreach (AsanaWorkspace workspace in o)
                {
                    Console.WriteLine("Workspace: " + workspace.Name);

                    // Times
                    asana.GetTeamsInWorkspace(workspace, teams =>
                    {
                        foreach (AsanaTeam team in teams)
                        {
                            //                            if (team.Name != "Projetos Especiais")
                            //                                continue;

                            Console.WriteLine("  Team: " + team.Name);

                            // Projetos
                            asana.GetProjectsInTeam(team, projects =>
                            {
                                foreach (AsanaProject project in projects)
                                {
                                    Console.WriteLine("    Project: " + project.Name);

                                    asana.GetTasksInAProject(project, tasks =>
                                    {
                                        foreach (AsanaTask task in tasks)
                                        {
                                            Console.WriteLine("      Task: " + task.Name);
                                        }
                                    }).Wait();
                                }
                            }).Wait();
                        }
                    }).Wait();
                }
            }).Wait();


            Console.WriteLine();
            Console.WriteLine("Execution time " + (DateTime.Now - startTime));
            Console.ReadLine();
        }

        private static void ErrorCallback(string arg1, string arg2, string arg3, object response)
        {
            Debug.WriteLine($"{arg1} {arg2} {arg3}");
        }

        private static string GetApiToken()
        {
            var configs = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            return configs.GetSection("ApiToken").Value;
        }
    }
}