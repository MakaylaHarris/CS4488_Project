using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.lib
{
    public delegate void InitTaskCallback(Task task, int taskNumber);
    /// <summary>
    /// class to help initialize test project
    /// Created 4/7/2021 by Robert Nelson
    /// </summary>
    public class InitModel
    {
        private Project project;

        public Project Project { get => project; }

        /// <summary>
        /// Initializes a new project
        /// </summary>
        /// <param name="numTasks">Number of tasks to add</param>
        /// <param name="subtasksPerTask">Number of subtasks per task</param>
        /// <param name="maxSubTaskLevel">The nested level of subtasks</param>
        public InitModel(int numTasks = 1, int subtasksPerTask = 1, int maxSubTaskLevel = 0)
        {
            if (maxSubTaskLevel > 0 && subtasksPerTask > 0)
                project = Init_Project(numTasks, initTaskCallback: new UniformSubtaskCallback(subtasksPerTask, maxSubTaskLevel).Callback);
            else
                project = Init_Project(numTasks);
        }

        /// <summary>
        /// Callback for creating uniform amount of subtasks
        /// </summary>
        class UniformSubtaskCallback
        {
            private InitTaskCallback init;
            public UniformSubtaskCallback(int subtasksPerTask, int maxSubTaskLevel)
            {
                SubtasksPerTask = subtasksPerTask;
                MaxSubTaskLevel = maxSubTaskLevel - 1;
                if(MaxSubTaskLevel > 0)
                    init = (new UniformSubtaskCallback(subtasksPerTask, MaxSubTaskLevel).Callback);
            }

            public void Callback(Task task, int taskNumber)
            {
                InitModel.Init_SubTasks(task, SubtasksPerTask, baseName: task.Name + "-", init: init);
            }

            public int SubtasksPerTask { get; }
            public int MaxSubTaskLevel { get; }
        }

        public static Project Init_Project(int numTasks, string projectName = "Foo", string taskBaseName = "Task ", InitTaskCallback initTaskCallback=null)
        {
            Project project = new Project(projectName, DateTime.Now, null, insert: false, track: false, likelyDuration: 10);
            Init_Tasks(project, numTasks, taskBaseName, initTaskCallback);
            return project;
        }

        public static List<Project> Init_Projects(int numProjects=1, int numTasksPerProject = 1, string projectBaseName="Project ", string taskBaseName="Task ", InitTaskCallback initTaskCallback = null)
        {
            List<Project> projects = new List<Project>();
            for (int i = 0; i < numProjects; i++) {
                Project project = new Project(projectBaseName + i.ToString(), DateTime.Now, null, insert: false, track: false, likelyDuration: 10);
                Init_Tasks(project, numTasksPerProject, taskBaseName, initTaskCallback);
            }
            return projects;
        }

        public static void Link_Dependencies(List<Task> toLink)
        {
            for (int i = 0; i < toLink.Count - 1; i++)
                toLink[i].AddDependency(toLink[i + 1]);
        }

        public static List<Task> Init_Tasks(Project project, int numTasks = 1, string baseName="Task ", InitTaskCallback initTaskCallback = null)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < numTasks; i++)
            {
                Task t = new Task(baseName + i.ToString(), DateTime.Now.AddDays(i * 3), null, 3, project: project, track: false, insert: false);
                tasks.Add(t);
                if (initTaskCallback != null)
                    initTaskCallback(t, i);
            }
            return tasks;
        }

        public static List<Task> Init_SubTasks(Task parent, int numSubTasks = 1, string baseName="SubTask ", InitTaskCallback init = null)
        {
            Project project = parent.Project;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < numSubTasks; i++)
            {
                Task t = new Task(baseName + i.ToString(), DateTime.Now.AddDays(i * 3), null, 3, project: project, track: false, insert: false);
                parent.AddSubTask(t);
                tasks.Add(t);
                if (init != null)
                    init(t, i);
            }
            return tasks;
        }
    }
}
