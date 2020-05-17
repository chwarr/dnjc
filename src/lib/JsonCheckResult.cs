// Copyright 2020, G. Christopher Warrington <code@cw.codes>
//
// This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
// Version 3. A copy of this license is included in the file LICENSE.
//
// SPDX-License-Identifier: AGPL-3.0-only

namespace DotNetJsonCheck
{
    public class JsonCheckResult
    {
        public JsonCheckResult(
            JsonCheckLevel level,
            string message,
            string? path,
            long? lineNumber,
            long? bytePositionInLine)
        {
            Level = level;
            Message = message;
            Path = path;
            LineNumber = lineNumber;
            BytePositionInLine = bytePositionInLine;
        }

        public JsonCheckLevel Level { get; }

        public string Message { get; }

        public string? Path { get; }

        public long? LineNumber { get; }

        public long? BytePositionInLine { get; }
    }
}
