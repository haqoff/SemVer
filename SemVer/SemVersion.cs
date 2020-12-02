﻿using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SemVer
{
    /// <summary>
    /// Представляет собой модель семантической версии.
    /// </summary>
    public class SemVersion
    {
        /// <summary>
        /// Получает МАЖОРНУЮ версию.
        /// </summary>
        public int Major { get; }

        /// <summary>
        /// Получает МИНОРНУЮ версию.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        /// Получает ПАТЧ-версию.
        /// </summary>
        /// <value>
        /// The patch version.
        /// </value>
        public int Patch { get; }

        /// <summary>
        /// Получает пререлизную версию.
        /// </summary>
        public string PreRelease { get; }

        /// <summary>
        /// Получает билд версию.
        /// </summary>
        public string Build { get; }


        private static readonly Regex ParseEx =
            new Regex(@"^(?<major>\d+)" +
                      @"(?>\.(?<minor>\d+))?" +
                      @"(?>\.(?<patch>\d+))?" +
                      @"(?>\-(?<pre>[0-9A-Za-z\-\.]+))?" +
                      @"(?>\+(?<build>[0-9A-Za-z\-\.]+))?$",
                RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture,
                TimeSpan.FromSeconds(0.5));

        public SemVersion(int major, int minor = 0, int patch = 0, string preRelease = "", string build = "")
        {
            Major = major;
            Minor = minor;
            Patch = patch;

            PreRelease = preRelease ?? "";
            Build = build ?? "";
        }

        /// <summary>
        /// Разбирает указанную строку в модель семантического версионирования.
        /// </summary>
        /// <param name="version">Строка версии.</param>
        /// <param name="strict">Признак того, что выполняется валидация указанного строки версии.</param>
        /// <exception cref="InvalidOperationException">Ошибка, возникающая в случае невалидной строки при выполнении валидации.</exception>
        public static SemVersion Parse(string version, bool strict = false)
        {
            var match = ParseEx.Match(version);
            if (!match.Success)
                throw new ArgumentException($"Invalid version '{version}'.", nameof(version));

            var major = int.Parse(match.Groups["major"].Value, CultureInfo.InvariantCulture);

            var minorMatch = match.Groups["minor"];
            int minor = 0;
            if (minorMatch.Success)
                minor = int.Parse(minorMatch.Value, CultureInfo.InvariantCulture);
            else if (strict)
                throw new InvalidOperationException("Invalid version (no minor version given in strict mode)");

            var patchMatch = match.Groups["patch"];
            int patch = 0;
            if (patchMatch.Success)
                patch = int.Parse(patchMatch.Value, CultureInfo.InvariantCulture);
            else if (strict)
                throw new InvalidOperationException("Invalid version (no patch version given in strict mode)");

            var preRelease = match.Groups["pre"].Value;
            var build = match.Groups["build"].Value;

            return new SemVersion(major, minor, patch, preRelease, build);
        }

        /// <summary>
        /// Получает текущую версию в строковом представлении.
        /// </summary>
        public override string ToString()
        {
            var estimatedLength = 4 + Major.Digits() + Minor.Digits() + Patch.Digits()
                                  + PreRelease.Length + Build.Length;
            var version = new StringBuilder(estimatedLength);
            version.Append(Major);
            version.Append('.');
            version.Append(Minor);
            version.Append('.');
            version.Append(Patch);
            if (PreRelease.Length > 0)
            {
                version.Append('-');
                version.Append(PreRelease);
            }

            if (Build.Length > 0)
            {
                version.Append('+');
                version.Append(Build);
            }

            return version.ToString();
        }
    }
}