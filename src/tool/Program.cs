// Copyright 2020, G. Christopher Warrington <code@cw.codes>
//
// This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
// Version 3. A copy of this license is included in the file LICENSE.
//
// SPDX-License-Identifier: AGPL-3.0-only

namespace DotNetJsonCheck
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using static System.FormattableString;

    internal class Program
    {
        private const int ExitSuccess = 0;
        private const int ExitBadArgs = 1;
        private const int ExitJsonErrors = 2;
        private const int ExitCatastrophic = 126;

        private static readonly CancellationTokenSource s_cancelCts =
            new CancellationTokenSource();

        private static async Task<int> Main(string[] args)
        {
            try
            {
                Console.CancelKeyPress += CancelKeyPressEventHandler;

                var options = Options.FromArgs(args);

                return options.Mode switch
                {
                    Mode.Check => await DoCheck(options).ConfigureAwait(false),
                    Mode.Help => await DoHelp(options).ConfigureAwait(false),
                    Mode.Version => await DoVersion().ConfigureAwait(false),
                    _ => throw new InvalidEnumArgumentException(nameof(options.Mode), (int)options.Mode, typeof(Mode)),
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return ExitCatastrophic;
            }
        }

        private static void CancelKeyPressEventHandler(object sender, ConsoleCancelEventArgs e)
        {
            s_cancelCts.Cancel();

            // Trap Ctrl-C and use that to indicate a co-operative shutdown.
            // Allow Ctrl-Break to make its way back to the OS so that it can kill us.
            e.Cancel = e.SpecialKey == ConsoleSpecialKey.ControlC;
        }

        private static async Task<int> DoCheck(Options options)
        {
            int errorCount = 0;

            using Stream input = Console.OpenStandardInput();

            await foreach (JsonCheckResult result in JsonCheck.Check(
                input,
                options.JsonCheckOptions,
                s_cancelCts.Token).ConfigureAwait(false))
            {
                ++errorCount;

                string errorReport = Invariant($"{result.Level}\t{result.LineNumber}\t{result.BytePositionInLine}\t{result.Message}");
                Console.Out.WriteLine(errorReport);
            }

            return errorCount == 0 ? ExitSuccess : ExitJsonErrors;
        }

        private static Task<int> DoHelp(Options options)
        {
            int result = ExitSuccess;

            if (!string.IsNullOrEmpty(options.UnknownArgument))
            {
                // Only report successful exit of --help was explicity requrested.
                result = ExitBadArgs;
                string msg = Invariant($"Unknown option \"{options.UnknownArgument}\"");
                Console.Out.WriteLine(msg);
                Console.Out.WriteLine();
            }

            Console.Out.WriteLine($@"dnjc -- .NET JSON Check
    [--allow-comments]
    [--allow-trailing-commas]
    [--strict]
    [--version]
    [--help]

    Reads JSON from standard input and parses it using
    System.Text.Json.JsonDocument.Parse().

    --allow-comments: allows /* */ and // style comments
                      (defaults to enabled)
    --allow-trailing-commas: allows trailing commas in arrays and objects
                             (defaults to enabled)
    --strict: parses JSON strictly (no comments, no trailing commas)
    --help: prints this help and exits
    --version: prints version information and exits

    If any errors are encountered, writes them to standard out and exits
    with {ExitJsonErrors}.

    Any number of options may be passed. They are processed in order.
    For example, to allow comments but not trailing commas, pass
        --strict --allow-comments
    in that order.

    The error output format is lines of:

    LEVEL<TAB>LINE<TAB>COLUMN<TAB>MESSAGE

    If LINE or COLUMN cannot be determined, they will be empty.

    Additional columns may be added in the future at the end. Ensure your
    parsing can handle this.
");

            return Task.FromResult(result);
        }

        private static Task<int> DoVersion()
        {
            string versionString = Assembly
                .GetEntryAssembly()!
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
                .InformationalVersion
                .ToString();

            Console.Out.WriteLine($@"dnjc: {versionString}

Copyright 2020, G. Christopher Warrington

This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
Version 3.");

            return Task.FromResult(ExitSuccess);
        }
    }

    internal enum Mode
    {
        Check,
        Help,
        Version,
    }

    internal class Options
    {
        private Mode _mode = Mode.Check;

        public Mode Mode
        {
            get => _mode;
            private set
            {
                // help "saturates"
                if (_mode == Mode.Help)
                {
                    return;
                }

                _mode = value;
            }
        }

        public JsonCheckOptions JsonCheckOptions { get; private set; } = new JsonCheckOptions();

        public string? UnknownArgument { get; private set; }

        public static Options FromArgs(string[] args)
        {
            var options = new Options();

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "--allow-comments":
                        options.JsonCheckOptions.AllowComments = true;
                        break;
                    case "--allow-trailing-commas":
                        options.JsonCheckOptions.AllowTrailingCommas = true;
                        break;
                    case "--help":
                    case "-h":
                    case "-help":
                    case "help":
                    case "-?":
                    case "/?":
                    case "?":
                        options.Mode = Mode.Help;
                        break;
                    case "--strict":
                        options.JsonCheckOptions.AllowComments = false;
                        options.JsonCheckOptions.AllowTrailingCommas = false;
                        break;
                    case "--version":
                        options.Mode = Mode.Version;
                        break;
                    default:
                        options.Mode = Mode.Help;
                        options.UnknownArgument = arg;
                        break;
                };
            }

            return options;
        }
    }
}
