using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Reflection;
using Task.Validators;

namespace Task_Tracker
{
    class Task
    {
        public int TaskId { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsComplete { get; set; }

        public override string ToString()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            return $"\nTask ID: {TaskId},\n" +
                   $"Description: {Description},\n" +
                   $"Due Date: {DueDate.ToShortDateString()}, \n" +
                   $"Complete: {IsComplete}\n\n";
        }
    }

    class TaskOperations
    {
        public static void AddTask(ref Task[] tasks, ref int taskCount)
        {
            string description = UserInputValidator.GetValidDescription();
            DateTime? dueDate = UserInputValidator.GetValidDueDate();

            if (description != null && dueDate != null)
            {
                if (taskCount == tasks.Length)
                {
                    Array.Resize(ref tasks, tasks.Length + 1);
                }

                Task newTask = new Task
                {
                    TaskId = taskCount + 1,
                    Description = description,
                    DueDate = dueDate.Value,
                    IsComplete = false
                };

                tasks[taskCount] = newTask;
                taskCount++;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n**************//Task added successfully//*************\n");
                Console.ResetColor();
            }
        }

        public static void MarkTaskComplete(Task[] tasks, int taskCount)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Enter the Task ID to mark as complete: ");
                Console.ResetColor();
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\nInvalid input. Please enter a valid positive integer.\n");
                    Console.ResetColor();
                    continue;
                }

                if (!int.TryParse(input, out int taskId))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\nInvalid input. Alphabets are not accepted. Please enter a valid positive integer.\n");
                    Console.ResetColor();
                    continue;
                }

                if (taskId > 0 && taskId <= taskCount)
                {
                    Task task = Array.Find(tasks, t => t != null && t.TaskId == taskId);
                    if (task != null)
                    {
                        task.IsComplete = true;
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("\n*************//Task marked as complete//***************\n");
                        Console.ResetColor();
                        return;
                    }
                    else
                    {
                        string error = "\nTask not found.\n";
                        ErrorLogger.LogError(error);
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(error);
                        Console.ResetColor();
                    }
                }
                else
                {
                    string error = "\nInvalid Task ID. Please enter a valid positive integer.\n";
                    ErrorLogger.LogError(error);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(error);
                    Console.ResetColor();
                }
            }
        }

        public static void ViewAllTasks(Task[] tasks, int taskCount)
        {
            if (taskCount == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nNo tasks available.\n");
                Console.ResetColor();
                return;
            }

            for (int i = 0; i < taskCount; i++)
            {
                Console.WriteLine(tasks[i]);
            }
        }

        public static void ViewIncompleteTasks(Task[] tasks, int taskCount)
        {
            bool hasIncompleteTasks = false;

            for (int i = 0; i < taskCount; i++)
            {
                if (!tasks[i].IsComplete)
                {
                    hasIncompleteTasks = true;
                    Console.WriteLine(tasks[i]);
                }
            }

            if (!hasIncompleteTasks)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nNo incomplete tasks available.\n");
                Console.ResetColor();
            }
        }
    }

    class UserInputValidator
    {
        public static string GetValidDescription()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Enter task description: ");
                Console.ResetColor();
                string description = Console.ReadLine();
                var validationResult = TaskValidator.ValidateDescription(description);
                if (validationResult.isValid)
                {
                    return description;
                }
                else
                {
                    ErrorLogger.LogError(validationResult.message);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(validationResult.message);
                    Console.ResetColor();
                }
            }
        }

        public static DateTime? GetValidDueDate()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Enter due date (yyyy-mm-dd): ");
                Console.ResetColor();
                string dueDateStr = Console.ReadLine();
                var validationResult = TaskValidator.ValidateDueDate(dueDateStr);
                if (validationResult.isValid && DateTime.TryParse(dueDateStr, out DateTime dueDate))
                {
                    return dueDate;
                }
                else
                {
                    ErrorLogger.LogError(validationResult.message);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(validationResult.message);
                    Console.ResetColor();
                }
            }
        }
    }

    class ErrorLogger
    {
        private static readonly string _directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\", "Task_Tracker_Payal");
        private static readonly string _errorPath = Path.Combine(_directoryPath, "Error.txt");

        public static void LogError(string errorMessage)
        {
            try
            {
                if (!Directory.Exists(_directoryPath))
                {
                    Directory.CreateDirectory(_directoryPath);
                }
                using (StreamWriter sw = File.AppendText(_errorPath))
                {
                    sw.WriteLine($"{DateTime.Now}: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Error logging message: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    class ProgramTaskTracker
    {
        static void Main(string[] args)
        {
            Task[] taskTracker = new Task[1];
            int taskCount = 0;
            bool start = true;

            while (start)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\n--------------------------Welcome to Task Tracker Application----------------------\n");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("1. Add a new task");
                Console.WriteLine("2. Mark a task as complete");
                Console.WriteLine("3. View all tasks");
                Console.WriteLine("4. View incomplete tasks");
                Console.WriteLine("5. Exit");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Select an option: ");
                Console.ResetColor();

                string option = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(option))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\nError: Option cannot be empty. Please enter a valid option.\n");
                    Console.ResetColor();
                    continue;
                }
                switch (option)
                {
                    case "1":
                        TaskOperations.AddTask(ref taskTracker, ref taskCount);
                        break;
                    case "2":
                        TaskOperations.MarkTaskComplete(taskTracker, taskCount);
                        break;
                    case "3":
                        TaskOperations.ViewAllTasks(taskTracker, taskCount);
                        break;
                    case "4":
                        TaskOperations.ViewIncompleteTasks(taskTracker, taskCount);
                        break;
                    case "5":
                        start = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("\nError: Invalid option. Please try again.\n");
                        Console.ResetColor();
                        break;
                }
            }
        }
    }
}
