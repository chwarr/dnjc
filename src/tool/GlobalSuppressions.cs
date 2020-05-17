// Copyright 2020, G.Christopher Warrington <c45207@mygcw.net>
//
// This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
// Version 3. A copy of this license is included in the file LICENSE.
//
// SPDX-License-Identifier: AGPL-3.0-only

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1031:Do not catch general exception types",
    Justification = "The is the program's top-level exception handler.",
    Scope = "member",
    Target = "~M:DotNetJsonCheck.Program.Main(System.String[])~System.Threading.Tasks.Task{System.Int32}")]
