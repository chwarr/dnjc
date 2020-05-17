// Copyright 2020, G.Christopher Warrington <c45207@mygcw.net>
//
// This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
// Version 3. A copy of this license is included in the file LICENSE.
//
// SPDX-License-Identifier: AGPL-3.0-only

namespace DotNetJsonCheck
{
    public class JsonCheckOptions
    {
        public bool AllowComments { get; set; } = true;

        public bool AllowTrailingCommas { get; set; } = true;
    }
}
