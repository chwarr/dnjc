﻿// Copyright 2020, G. Christopher Warrington <code@cw.codes>
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
                    ex.Path,
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

        private static string CleanMessage(string jsonExceptionMessage)
        {
            // When creating the exception message, the line number of byte position are always appended:
            // From https://github.com/dotnet/runtime/blob/81bf79fd9aa75305e55abe2f7e9ef3f60624a3a1/src/libraries/System.Text.Json/src/System/Text/Json/ThrowHelper.cs#L292
            //
            // message += $" LineNumber: {lineNumber} | BytePositionInLine: {bytePositionInLine}.";

            var r = new Regex(
                " LineNumber: [0-9]* | BytePositionInLine: [0-9]*\\.$",
                RegexOptions.CultureInvariant | RegexOptions.Singleline,
                matchTimeout: TimeSpan.FromMilliseconds(200));

            Match match = r.Match(jsonExceptionMessage);

            if (match.Success)
            {
                return jsonExceptionMessage.Substring(startIndex: 0, length: match.Index);
            }

            return jsonExceptionMessage;
        }
    }
}
