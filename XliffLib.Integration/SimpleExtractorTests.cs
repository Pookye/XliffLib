﻿using NUnit.Framework;
using System;
using XliffLib.Integration.Utils;
using Localization.Xliff.OM.Core;
using XliffLib.Utils;

namespace XliffLib.Integration
{
    [TestFixture]
    public class SimpleExtractorTests
    {
        [Test, TestCaseSource(typeof(DataSamples), "FileNames")]
        public void CanExtractSimpleFile(string filename)
        {
            var bundle = EmbeddedFilesReader.ReadString("XliffLib.Integration.TestFiles." + filename + ".json").ToBundle();
            var xliff = EmbeddedFilesReader.ReadString("XliffLib.Integration.TestFiles." + filename + ".xlf");

            var extractor = new SimpleExtractor();
            var xliffModel = extractor.Extract(bundle, "en-US", "it-IT");

            var xliffString = extractor.Write(xliffModel, true);

            var cleanedExpected = System.Text.RegularExpressions.Regex.Replace(xliff, @"\s+", " ");
            var cleanedResult = System.Text.RegularExpressions.Regex.Replace(xliffString, @"\s+", " ");

            Assert.AreEqual(cleanedExpected, cleanedResult);
        }
    }
}
