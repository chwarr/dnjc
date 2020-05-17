// Copyright 2020, G. Christopher Warrington <code@cw.codes>
//
// This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
// Version 3. A copy of this license is included in the file LICENSE.
//
// SPDX-License-Identifier: AGPL-3.0-only

namespace DotNetJsonCheck
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading;

    public static class JsonCheck
    {
        public static async IAsyncEnumerable<JsonCheckResult> Check(
            Stream input,
            JsonCheckOptions? options = null,
            [EnumeratorCancellation]
            CancellationToken ct = default)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            options ??= new JsonCheckOptions();

            var jdo = new JsonDocumentOptions
            {
                AllowTrailingCommas = options.AllowTrailingCommas,
                CommentHandling = options.AllowComments ? JsonCommentHandling.Skip : JsonCommentHandling.Disallow,
            };

            JsonCheckResult? result = null;

            try
            {
                using (input)
                {
                    _ = await JsonDocument.ParseAsync(input, jdo, ct).ConfigureAwait(false);
                }
            }
            catch (JsonException ex)
            {
                // JsonException uses 0-based line indices, but almost every
                // text editor uses 1-based, so add 1.
                long? reportLineNumber = ex.LineNumber + 1;

                result = new JsonCheckResult(
                    JsonCheckLevel.Error,
                    CleanMessage(ex.Message),
                    reportLineNumber,
                    ex.BytePositionInLine);
            }
            catch (OperationCanceledException)
            {
            }

            if (result != null)
            {
                yield return result;
            }
        }

        private static string CleanMessage(string message)
        {
            // Order of processing is important: we end-of-string as an anchor
            // for some of the removals.
            message = RemoveLineNumberBytePosition(message);
            message = RemoveReaderOptions(message);
            message = EscapeCrLf(message);

            return message;
        }

        private static string RemoveLineNumberBytePosition(string message)
        {
            // When creating the exception message, the line number of byte
            // position are always appended. This tool reports those in a more
            // strucutred way, so they don't need to be repeated in the
            // message.
            //
            // From https://github.com/dotnet/runtime/blob/81bf79fd9aa75305e55abe2f7e9ef3f60624a3a1/src/libraries/System.Text.Json/src/System/Text/Json/ThrowHelper.cs#L292
            //
            // message += $" LineNumber: {lineNumber} | BytePositionInLine: {bytePositionInLine}.";

            var r = new Regex(
                " LineNumber: [0-9]* | BytePositionInLine: [0-9]*\\.$",
                RegexOptions.CultureInvariant | RegexOptions.Singleline,
                matchTimeout: TimeSpan.FromMilliseconds(200));

            Match match = r.Match(message);

            if (match.Success)
            {
                return message.Substring(startIndex: 0, length: match.Index);
            }

            return message;
        }

        private static string RemoveReaderOptions(string message)
        {
            // Sometimes the message has the instructions to "Change the reader
            // options.". This doesn't make sense as output from this tool, as
            // it's about what to do in one's own code to enable the feature.
            //
            // This only works for English.
            //
            // I looked through all the exception messages in https://github.com/dotnet/runtime/blob/8dbfb8e4dcb10e739369fba2a55bfe68da5e535c/src/libraries/System.Text.Json/src/Resources/Strings.resx#L328
            // to figure out which ones to clean up.

            const string EnglishReaderOptionsMessage = " Change the reader options.";

            int idx = message.LastIndexOf(EnglishReaderOptionsMessage, StringComparison.InvariantCulture);

            if (idx != -1)
            {
                return message.Substring(startIndex: 0, length: idx);
            }

            return message;

        }

        private static string EscapeCrLf(string message)
        {
            // Depening on the input, the message can contain CR and LF. These
            // will mess up the output to standard out, so they are replaced
            // with \r \n. This does mean that the message may no longer
            // accurately reflect the contents of the JSON, but that's better
            // than having corrupt output.

            return message
                .Replace("\r", "\\r", StringComparison.InvariantCulture)
                .Replace("\n", "\\n", StringComparison.InvariantCulture);
        }
    }
}
