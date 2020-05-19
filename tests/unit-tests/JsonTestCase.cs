// Copyright 2020, G.Christopher Warrington <c45207@mygcw.net>
//
// This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
// Version 3. A copy of this license is included in the file LICENSE.
//
// SPDX-License-Identifier: AGPL-3.0-only

namespace DotNetJsonCheck.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    public sealed class JsonTestCase
    {
        private static readonly Lazy<IReadOnlyList<JsonTestCase>> s_dataSet = new Lazy<IReadOnlyList<JsonTestCase>>(
            InitializeDataSet,
            LazyThreadSafetyMode.ExecutionAndPublication);

        private JsonTestCase(string json, bool hasComment, bool hasTrailingComma)
        {
            Json = json;
            HasComment = hasComment;
            HasTrailingComma = hasTrailingComma;
        }

        public string Json { get; }

        public bool HasComment { get; }

        public bool HasTrailingComma { get; }

        public static IReadOnlyList<JsonTestCase> Cases => s_dataSet.Value;

        private static IReadOnlyList<JsonTestCase> InitializeDataSet()
        {
            var result = new List<JsonTestCase>();

            // We're looking for private const string fields.
            IEnumerable<FieldInfo> testDataFields = typeof(JsonTestCase)
                .GetMembers(BindingFlags.NonPublic | BindingFlags.Static)
                .OfType<FieldInfo>()
                .Where(fi =>
                    fi.IsPrivate
                    && fi.IsStatic
                    && fi.IsLiteral
                    && !fi.IsInitOnly
                    && fi.FieldType == typeof(string));

            foreach (FieldInfo fi in testDataFields)
            {
                object? value = fi.GetRawConstantValue();
                if (!(value is string json))
                {
                    throw new Exception($"{fi.Name} was expected to be a non-null string, but it was null.");
                }

                bool hasComment = fi.GetCustomAttribute<HasCommentAttribute>() != null;
                bool hasTrailingComma = fi.GetCustomAttribute<HasTrailingCommaAttribute>() != null;

                result.Add(new JsonTestCase(json, hasComment, hasTrailingComma));
            }

            return result.AsReadOnly();
        }

        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        private sealed class HasCommentAttribute : System.Attribute
        {
            public HasCommentAttribute()
            {
            }
        }

        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        private sealed class HasTrailingCommaAttribute : System.Attribute
        {
            public HasTrailingCommaAttribute()
            {
            }
        }

        #region Data Set
        // These fields are used, but they're only found via reflection.
#pragma warning disable CA1823 // Avoid unused private fields

        // The data set is represented by a collection of private const string
        // fields.
        //
        // Each field can be annotated with [HasComment] or [HasTrailingComma].
        // These attributes are used to populate the relevant fields in the
        // test case data structure.
        private const string Strict1 = @"[]";

        private const string Strict2 = @"{}";

        private const string Strict3 = @"true";

        private const string Strict4 = @"false";

        private const string Strict5 = @"null";

        private const string Strict6 = @"""string""";

        private const string Strict7 = @"123.456E19";

        private const string Strict8 = @"[{},{}]";

        private const string Strict9 = @"{""an-object"": true}";

        [HasTrailingComma]
        private const string Comma1 = @"[1,]";

        [HasTrailingComma]
        private const string Comma2 = @"[1,]";

        [HasTrailingComma]
        private const string Comma3 = @"[1,2,]";

        [HasTrailingComma]
        private const string Comma4 = @"{""an-object"": true, ""look-a-trailing-comma"": true,}";

        [HasComment]
        private const string Comment1 = @"/* C-style */ true";

        [HasComment]
        private const string Comment2 = @"/* C-style */ true";

        [HasComment]
        private const string Comment3 = @"// C++ style
true";

        [HasComment]
        private const string Comment4 = @"{
// inside an object
""some-prop"": []
}";

        [HasComment]
        [HasTrailingComma]
        private const string Relaxed1 = @"[1, /* C-style */]";

        [HasComment]
        [HasTrailingComma]
        private const string Relaxed2 = @"{
// C++ style
""object"": ""with trailing comma"",
}"
;

#pragma warning restore CA1823 // Avoid unused private fields
        #endregion
    }
}
