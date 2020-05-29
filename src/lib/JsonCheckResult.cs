// Copyright 2020, G. Christopher Warrington <code@cw.codes>
//
// dnjc is free software: you can redistribute it and/or modify it under the
// terms of the GNU Affero General Public License Version 3 as published by the
// Free Software Foundation.
//
// dnjc is distributed in the hope that it will be useful, but WITHOUT ANY
// WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
// details.
//
// A copy of the GNU Affero General Public License Version 3 is included in the
// file LICENSE in the root of the repository.
//
// SPDX-License-Identifier: AGPL-3.0-only

namespace DotNetJsonCheck
{
    public class JsonCheckResult
    {
        public JsonCheckResult(
            JsonCheckLevel level,
            string message,
            long? lineNumber,
            long? bytePositionInLine)
        {
            Level = level;
            Message = message;
            LineNumber = lineNumber;
            BytePositionInLine = bytePositionInLine;
        }

        public JsonCheckLevel Level { get; }

        public string Message { get; }

        public long? LineNumber { get; }

        public long? BytePositionInLine { get; }
    }
}
