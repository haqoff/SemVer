using NUnit.Framework;
using SemVer;

namespace SemVerParsingTests
{
    public class Tests
    {
        public static readonly (string version, int major, int minor, int patch, string preRelease, string build)[]
            RegexValidExamples =
            {
                ("0.0.4", 0, 0, 4, "", ""),
                ("1.2.3", 1, 2, 3, "", ""),
                ("10.20.30", 10, 20, 30, "", ""),
                ("1.1.2-prerelease+meta", 1, 1, 2, "prerelease", "meta"),
                ("1.1.2+meta", 1, 1, 2, "", "meta"),
                ("1.1.2+meta-valid", 1, 1, 2, "", "meta-valid"),
                ("1.0.0-alpha", 1, 0, 0, "alpha", ""),
                ("1.0.0-beta", 1, 0, 0, "beta", ""),
                ("1.0.0-alpha.beta", 1, 0, 0, "alpha.beta", ""),
                ("1.0.0-alpha.beta.1", 1, 0, 0, "alpha.beta.1", ""),
                ("1.0.0-alpha.1", 1, 0, 0, "alpha.1", ""),
                ("1.0.0-alpha0.valid", 1, 0, 0, "alpha0.valid", ""),
                ("1.0.0-alpha.0valid", 1, 0, 0, "alpha.0valid", ""),
                ("1.0.0-alpha-a.b-c-somethinglong+build.1-aef.1-its-okay", 1, 0, 0, "alpha-a.b-c-somethinglong",
                    "build.1-aef.1-its-okay"),
            };

        public static readonly string[] RegexInvalidExamples =
        {
            "1",
            "1.2",
            "1.2.3-0123",
            "1.2.3-0123.0123",
            "1.1.2+.123",
            "+invalid",
            "-invalid",
            "-invalid+invalid",
            "-invalid.01",
            "alpha",
            "alpha.beta",
            "alpha.beta.1",
            "alpha.1",
            "alpha+beta",
            "alpha_beta",
            "alpha.",
            "alpha..",
            "beta",
            "1.0.0-alpha_beta",
            "-alpha.",
            "1.0.0-alpha..",
            "1.0.0-alpha..1",
            "1.0.0-alpha...1"
        };


        [Test]
        public void TestValidVersions()
        {
            foreach (var example in RegexValidExamples)
            {
                var actual = SemVersion.Parse(example.version);

                Assert.AreEqual(example.major, actual.Major);
                Assert.AreEqual(example.minor, actual.Minor);
                Assert.AreEqual(example.patch, actual.Patch);
                Assert.AreEqual(example.preRelease, actual.PreRelease);
                Assert.AreEqual(example.build, actual.Build);
            }
        }

        [Test]
        public void TestInvalidVersions()
        {
            foreach (var example in RegexInvalidExamples)
            {
                try
                {
                    SemVersion.Parse(example, true);
                    Assert.Fail();
                }
                catch
                {
                    Assert.Pass();
                }
            }
        }
    }
}