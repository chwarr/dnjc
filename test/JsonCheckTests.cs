// Copyright 2020, G.Christopher Warrington <c45207@mygcw.net>
//
// This software is released under the GNU AFFERO GENERAL PUBLIC LICENSE
// Version 3. A copy of this license is included in the file LICENSE.
//
// SPDX-License-Identifier: AGPL-3.0-only

namespace DotNetJsonCheck.Tests
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    public class JsonCheckTests
    {
        private readonly ITestOutputHelper _logger;

        public JsonCheckTests(ITestOutputHelper logger) => _logger = logger;

        [Fact]
        public async Task EmptyInput_Errors()
        {
            using var ms = new MemoryStream();

            JsonCheckResult? result =
                await JsonCheck.Check(ms).SingleOrDefaultAsync().ConfigureAwait(false);

            Assert.NotNull(result);

            Assert.Equal(JsonCheckLevel.Error, result.Level);
            Assert.Equal(1, result.LineNumber);
        }

        [Theory]
        [CombinatorialData]
        public async Task ValidJson_BomAgnostic_NoErrors(bool includeBom)
        {
            const string ValidJson = @"""I'm valid JSON!""";

            using MemoryStream ms = EncodeUtf8(ValidJson, includeBom: includeBom);
            bool hasError = await HasError(ms).ConfigureAwait(false);

            Assert.False(hasError);
        }

        [Fact]
        public async Task InvalidJson_ErrorReportedOnExpectedLineAndByteOffset()
        {
            const string InvalidJson = @"{
""line-2-ok"": true,
""line-3-not-ok"": not-a-token,
}";

            using MemoryStream ms = EncodeUtf8(InvalidJson);

            JsonCheckResult r =
                await JsonCheck.Check(ms).SingleAsync().ConfigureAwait(false);

            Assert.Equal(JsonCheckLevel.Error, r.Level);
            Assert.Equal(3, r.LineNumber);
            Assert.Equal(18, r.BytePositionInLine);
        }

        [Theory]
        [CombinatorialData]
        public async Task OptionsPermutations_ErrorsWhenExpected(
            bool allowComment,
            bool allowTrailingComma)
        {
            var o = new JsonCheckOptions
            {
                AllowComments = allowComment,
                AllowTrailingCommas = allowTrailingComma,
            };

            foreach (JsonTestCase jtc in JsonTestCase.Cases)
            {
                _logger.WriteLine("Testing <{0}>", jtc.Json);

                bool commentErrorExpected = jtc.HasComment && !allowComment;
                bool trailingCommaErrorExpected = jtc.HasTrailingComma && !allowTrailingComma;
                bool errorExpected = commentErrorExpected || trailingCommaErrorExpected;

                bool hasError = await HasError(jtc.Json, o).ConfigureAwait(false);

                Assert.Equal(errorExpected, hasError);
            }
        }

        private static MemoryStream EncodeUtf8(string str, bool includeBom = false)
        {
            var e = new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: includeBom,
                throwOnInvalidBytes: true);

            return new MemoryStream(e.GetBytes(str));
        }

        private static ValueTask<bool> HasError(Stream s, JsonCheckOptions? options = null)
        {
            return JsonCheck.Check(s, options).AnyAsync();
        }

        private static ValueTask<bool> HasError(string json, JsonCheckOptions? options = null)
        {
            using MemoryStream ms = EncodeUtf8(json);
            return HasError(ms, options);
        }
    }
}
