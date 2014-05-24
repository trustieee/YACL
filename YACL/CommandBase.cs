using System;
using System.Collections.Generic;
using System.Text;

namespace YACL
{
    /// <summary>
    ///     Base class for a YACL command.
    /// </summary>
    /// <remarks>
    ///     Derive this class to create your own YACL command.
    /// </remarks>
    public abstract class CommandBase
    {
        private readonly Dictionary<string, string> mParsedArguments = new Dictionary<string, string>();

        /// <summary>
        ///     Gets the description of this command.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        ///     Gets the name of this command.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        ///     Gets the usage for this command.
        /// </summary>
        /// <remarks>
        ///     Commands without any arguments cannot show their usage.
        ///     It's recommended to create a HelpCommand in your application
        ///     and provide the simple usage for every command in your application through that command's Execute() override.
        /// </remarks>
        public virtual string Usage
        {
            get
            {
                var usage = new StringBuilder();
                usage.AppendFormat("{0}.exe help\n", ApplicationBase.ApplicationName);
                if (RequiredArguments == null)
                {
                    Console.WriteLine("there was an error gathering the required arguments for command ({0})", Name);
                    EnvironmentWrapper.Exit(ExitCode.InternalNoRequiredArgumentsProvided);
                }
                else
                {
                    usage.AppendLine(string.Format("command arguments for ({0}) are:", Name));
                    foreach (string requiredArgument in RequiredArguments)
                    {
                        usage.AppendLine(string.Format("- {0}", requiredArgument));
                    }
                }

                return usage.ToString();
            }
        }

        /// <summary>
        ///     <b>True</b> if this command requires arguments, otherwise <b>False</b>.
        /// </summary>
        /// <remarks>Optional</remarks>
        public virtual bool RequiresArguments
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets the required arguments for this command.
        /// </summary>
        /// <remarks>Optional</remarks>
        public virtual string[] RequiredArguments
        {
            get { return null; }
        }

        /// <summary>
        ///     Gets the parsed command arguments as a list of key value pairs.
        /// </summary>
        public Dictionary<string, string> ParsedArguments
        {
            get { return mParsedArguments; }
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     The actions that take place when this command is executed.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        ///     Adds a parsed command argument to this command.
        /// </summary>
        /// <param name="parsedArgument"></param>
        public void AddArgument(KeyValuePair<string, string> parsedArgument)
        {
            mParsedArguments.Add(parsedArgument.Key, parsedArgument.Value);
        }
    }
}