using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YACL
{
    /// <summary>
    ///     The base logic for YACL. Derive this class in any console application to have commands parsed.
    /// </summary>
    /// <remarks>
    ///     Call Start(...) and pass in your command line arguments.
    /// </remarks>
    public class ApplicationBase
    {
        internal static string ApplicationName = Assembly.GetEntryAssembly().GetName().Name;
        private static readonly List<CommandBase> sDiscoveredCommands;
        private readonly IEnumerable<string> mCommandNames = sDiscoveredCommands.Select(s => s.Name);

        static ApplicationBase()
        {
            sDiscoveredCommands = new List<CommandBase>();
            Assembly applicationAssembly = Assembly.GetEntryAssembly();
            IEnumerable<Type> commandImplementationTypes = applicationAssembly.GetTypes().Where(t => t.BaseType == typeof (CommandBase));
            foreach (Type commandImplementationType in commandImplementationTypes)
            {
                var commandImplementationInstance = (CommandBase) Activator.CreateInstance(commandImplementationType);
                sDiscoveredCommands.Add(commandImplementationInstance);
            }
        }

        /// <summary>
        ///     Starts YACL's command parsing and execution.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public void Start(string[] args)
        {
            Console.WriteLine();
            ValidateArgs(args);
            EnvironmentWrapper.Exit(ExitCode.Success);
        }

        private void ValidateArgs(string[] args)
        {
            if (args == null || !args.Any())
            {
                Console.WriteLine("there were no commands provided.");
                Console.WriteLine("the valid commands are:");
                PrintCommands();
                EnvironmentWrapper.Exit(ExitCode.NoCommand);
            }
            else
            {
                string command = args[0];
                string[] commandArgs = args.Skip(1).Take(args.Count() - 1).ToArray();
                if (mCommandNames.Select(n => n.ToLower()).Any(command.ToLower().Equals))
                {
                    CommandBase commandImplementationToUse = sDiscoveredCommands.FirstOrDefault(c => c.Name.Equals(command, StringComparison.OrdinalIgnoreCase));
                    if (commandImplementationToUse != null)
                    {
                        ValidateCommandArguments(commandImplementationToUse, commandArgs);
                        return;
                    }
                }

                // If we made it this far, the command wasn't valid.
                Console.WriteLine("the provided command ({0}) is not valid. possible commands are:", command);
                PrintCommands();
                EnvironmentWrapper.Exit(ExitCode.InvalidCommand);
            }
        }

        private void ValidateCommandArguments(CommandBase command, string[] commandArgs)
        {
            // Requires arguments.
            if (command.RequiresArguments)
            {
                // No arguments given.
                if (commandArgs == null || !commandArgs.Any())
                {
                    Console.WriteLine("there were no command arguments provided for the command ({0})",
                        command);
                    Console.WriteLine("usage for command ({0}):", command);
                    Console.WriteLine(command.Usage);
                    EnvironmentWrapper.Exit(ExitCode.NoCommandArguments);
                }
                else
                {
                    var invalidCommandArgs = new List<string>();

                    // Arguments given, check their validity.
                    foreach (string commandArg in commandArgs)
                    {
                        string[] argPair = commandArg.Split(new[] {'='});
                        if (argPair.Count() == 2)
                        {
                            command.AddArgument(new KeyValuePair<string, string>(argPair[0], argPair[1]));
                        }
                        else
                        {
                            Console.WriteLine("the command ({0}) was given an argument of {1}, which does not meet the required format of commandArg=value", command, commandArg);
                            invalidCommandArgs.Add(commandArg);
                        }
                    }

                    List<string> missingCommandArgs = command.RequiredArguments.Where(requiredArgument => !command.ParsedArguments.ContainsKey(requiredArgument)).ToList();
                    List<string> unknownCommandArgs = (from commandArg in command.ParsedArguments where !command.RequiredArguments.Contains(commandArg.Key) select commandArg.Key).ToList();

                    // Unknown argument check (warning).
                    if (unknownCommandArgs.Any())
                    {
                        Console.WriteLine("there were command arguments provided for the command ({0}) that were unknown... this is not a breaking action and ({0}) will continue as normal", command);
                        Console.WriteLine("the unknown command arguments were:");
                        unknownCommandArgs.ForEach(a => Console.WriteLine("- {0}", a));
                    }

                    // Invalid argument check (error).
                    if (invalidCommandArgs.Any())
                    {
                        Console.WriteLine("there were invalid command arguments provided for the command ({0})", command);
                        Console.WriteLine("the invalid command arguments were:");
                        invalidCommandArgs.ForEach(a => Console.WriteLine("- {0}", a));
                        Console.WriteLine("usage for command ({0}):", command);
                        Console.WriteLine(command.Usage);
                        EnvironmentWrapper.Exit(ExitCode.InvalidCommandArguments);
                    }

                    // Missing argument check (error).
                    if (missingCommandArgs.Any())
                    {
                        Console.WriteLine("there were missing command arguments provided for the command ({0})", command);
                        Console.WriteLine("the missing command arguments were:");
                        missingCommandArgs.ForEach(a => Console.WriteLine("- {0}", a));
                        Console.WriteLine("usage for command ({0}):", command);
                        Console.WriteLine(command.Usage);
                        EnvironmentWrapper.Exit(ExitCode.InvalidCommandArguments);
                    }
                }
            }

            // If we made it this far, the command is valid.
            command.Execute();
        }

        private void PrintCommands()
        {
            mCommandNames.OrderBy(c => c).ToList().ForEach(c => Console.WriteLine("- {0}", c));
        }
    }
}