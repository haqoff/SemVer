using System.Collections.Generic;
using NUnit.Framework;
using SemVer;

namespace SemVerParsingTests
{
    public class Tests
    {
        public static readonly IReadOnlyList<SemVersion> VersionsInOrder = new List<SemVersion>()
        {
            new SemVersion(-2),
            new SemVersion(0, 0, -1),
            new SemVersion(0),
            new SemVersion(0, 0, 1, "13"),
            new SemVersion(0, 0, 1, ".a"),
            new SemVersion(0, 0, 1, "b"),
            new SemVersion(1, 0, 10, "alpha"),
            new SemVersion(1, 2, 0, "alpha", "dev"),
            new SemVersion(1, 2, 0, "nightly2"),
            new SemVersion(1, 2),
            new SemVersion(1, 2, 0, "", "nightly"),
            new SemVersion(1, 2, 1, "99"),
            new SemVersion(1, 2, 1)
        }.AsReadOnly();

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
        public void TestValidVersionsRaw()
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
        public void TestValidVersionsEqualityOperator()
        {
            SemVersion prevVer = null;

            foreach (var example in RegexValidExamples)
            {
                var ver1 = SemVersion.Parse(example.version);
                var ver2 = new SemVersion(example.major, example.minor, example.patch, example.preRelease, example.build);

                Assert.True(ver1 == ver2);
                Assert.True(ver1.Equals(ver2));
                Assert.True(ver1 != prevVer);

                prevVer = ver1;
            }
        }

        [Test]
        public void TestOperatorsGreaterLessAndEqual()
        {
            for (var ver1Index = 0; ver1Index < VersionsInOrder.Count; ver1Index++)
            {
                for (var ver2Index = 0; ver2Index < VersionsInOrder.Count; ver2Index++)
                {
                    var first = VersionsInOrder[ver1Index];
                    var second = VersionsInOrder[ver2Index];

                    if (ver1Index < ver2Index)
                    {
                        Assert.True(first < second, "{0} < {1}", first, second);
                        Assert.True(first <= second, "{0} <= {1}", first, second);
                    }

                    if (ver1Index > ver2Index)
                    {
                        Assert.True(first > second, "{0} > {1}", first, second);
                        Assert.True(first >= second, "{0} >= {1}", first, second);
                    }

                    if (ver1Index == ver2Index)
                    {
                        Assert.True(first == second, "{0} == {1}", first, second);
                    }
                }
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