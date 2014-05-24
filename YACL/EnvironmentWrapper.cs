using System;

namespace YACL
{
    /// <summary>
    ///     The different times of exit codes supported by the YACL runtime.
    /// </summary>
    public enum ExitCode
    {
        Success,
        NoCommand,
        InvalidCommand,
        NoCommandArguments,
        InvalidCommandArguments,
        MissingCommandArguments,
        InternalNoCommandMetadata,
        InternalNoRequiredArgumentsProvided,
    }

    /// <summary>
    ///     Helper class to manage exiting the application gracefully, with a propietary exit code.
    /// </summary>
    public static class EnvironmentWrapper
    {
        public static void Exit(ExitCode exitCode)
        {
            Environment.Exit((int) exitCode);
        }
    }
}