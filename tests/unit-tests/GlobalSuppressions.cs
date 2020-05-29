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

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Naming",
    "CA1707:Identifiers should not contain underscores",
    Justification = "Test case names use underscore seperators.",
    Scope = "module")]
