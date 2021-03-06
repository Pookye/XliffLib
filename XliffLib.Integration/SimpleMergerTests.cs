﻿using NUnit.Framework;
using System;
using XliffLib.Integration.Utils;
using Localization.Xliff.OM.Core;
using XliffLib.Utils;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XliffLib.Integration
{

    [TestFixture]
    public class SimpleMergerTests
    {
        [Test, TestCaseSource(typeof(DataSamples), "FileNamesSimpleExtractor")]
        public void CanMergeSimpleFile(string filename)
        {
            var bundle = EmbeddedFilesReader.ReadString("XliffLib.Integration.TestFiles." + filename + ".json");
            var xliff = EmbeddedFilesReader.ReadString("XliffLib.Integration.TestFiles." + filename + ".target.xlf");

            var merger = new SimpleMerger();
            var xliffModel = merger.Read(xliff);

            var resultingBundle = merger.Merge(xliffModel);

            var jsonResult = resultingBundle.ToJson();

            JObject expected = JObject.Parse(bundle);
            JObject result = JObject.Parse(jsonResult);

            Assert.IsTrue(JToken.DeepEquals(expected, result),"The two bundles are different:\r\nExpected {0}\r\nResult {1}",bundle,jsonResult);
        }

    }
}