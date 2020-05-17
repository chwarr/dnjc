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
                    ex.Message,
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
    }
}
