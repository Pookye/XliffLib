﻿using NUnit.Framework;
using System;
using Localization.Xliff.OM.Serialization;
using System.IO;
using Localization.Xliff.OM.Core;

namespace XliffLib.Test
{
    [TestFixture()]
    public class ParagraphSplitterExtractorTests
    {
        [Test()]
        public void SingleParagraphPlainTextUnitIsNotSplit()
        {
            var xliff = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en-GB"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
    <file id=""f1"">
        <unit id=""u1"">
            <segment>
                <source>Hello Word!</source>
            </segment>
        </unit>
    </file>
</xliff>";

            XliffDocument document = LoadXliff(xliff);
            var splitter = new ParagraphSplitter();

            var newDocument = splitter.ExecuteExtraction(document);

            Assert.AreEqual(1, newDocument.Files[0].Containers.Count);
            var unit = newDocument.Files[0].Containers[0] as Unit;
            Assert.IsNotNull(unit);
        }

        [Test()]
        public void MulipleParagraphPlainTextUnitIsSplit()
        {
            var xliff = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en-GB"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
    <file id=""f1"">
        <unit id=""u1"" name=""originaParagraph"">
            <segment>
                <source>Hello Word1!
Hello Word2!
Hello Word3!</source>
            </segment>
        </unit>
    </file>
</xliff>";
            XliffDocument document = LoadXliff(xliff);
            var splitter = new ParagraphSplitter();

            var newDocument = splitter.ExecuteExtraction(document);

            Assert.AreEqual(1, newDocument.Files[0].Containers.Count);
            var group = newDocument.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group);
            Assert.AreEqual("u1-g", group.Id);
            Assert.AreEqual("originaParagraph", group.Name);

            Assert.AreEqual(3, group.Containers.Count);

            var unit1 = group.Containers[0] as Unit;
            var textUnit1 = unit1.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("Hello Word1!", textUnit1);

            var unit2 = group.Containers[1] as Unit;
            var textUnit2 = unit2.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("Hello Word2!", textUnit2);

            var unit3 = group.Containers[2] as Unit;
            var textUnit3 = unit3.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("Hello Word3!", textUnit3);
        }

        [Test()]
        public void SingleHtmlParagraphUnitIsWrappedIntoGroup()
        {
            var xliff = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en-GB"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
    <file id=""f1"">
        <unit id=""u1"" name=""body"">
            <segment>
                <source><![CDATA[<p>Hello Word!</p>]]></source>
            </segment>
        </unit>
    </file>
</xliff>";

            XliffDocument document = LoadXliff(xliff);
            var splitter = new ParagraphSplitter();

            var newDocument = splitter.ExecuteExtraction(document);

            Assert.AreEqual(1, newDocument.Files[0].Containers.Count);
            var group = newDocument.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group);
            Assert.AreEqual("body", group.Name);

            Assert.AreEqual(1, group.Containers.Count);
            var unit = group.Containers[0] as Unit;

            Assert.AreEqual("p", unit.Name);
            Assert.AreEqual("Hello Word!", unit.Resources[0].Source.Text[0].ToString());
        }


        [Test()]
        public void SingleHtmlParagraphUnitWithFormattingIsWrappedIntoGroup()
        {
            var xliff = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en-GB"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
    <file id=""f1"">
        <unit id=""u1"" name=""body"">
            <segment>
                <source><![CDATA[<p>Hello <b>Word</b>!</p>]]></source>
            </segment>
        </unit>
    </file>
</xliff>";

            XliffDocument document = LoadXliff(xliff);
            var splitter = new ParagraphSplitter();

            var newDocument = splitter.ExecuteExtraction(document);

            Assert.AreEqual(1, newDocument.Files[0].Containers.Count);
            var group = newDocument.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group);
            Assert.AreEqual("body", group.Name);

            Assert.AreEqual(1, group.Containers.Count);
            var unit = group.Containers[0] as Unit;

            Assert.AreEqual("p", unit.Name);
            Assert.AreEqual("<![CDATA[Hello <b>Word</b>!]]>", unit.Resources[0].Source.Text[0].ToString());
        }

        [Test()]
        public void MultipleParagraphsUnitIsTransformedIntoGroupWithManyUnits()
        {
            var xliff = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en-GB"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
    <file id=""f1"">
        <unit id=""u1"">
            <segment>
                <source><![CDATA[<p>Hello Word1!</p><p>Hello Word2!</p><p>Hello <b>Word3</b>!</p>]]></source>
            </segment>
        </unit>
    </file>
</xliff>";

            XliffDocument document = LoadXliff(xliff);


            var splitter = new ParagraphSplitter();

            var newDocument = splitter.ExecuteExtraction(document);

            Assert.AreEqual(1, newDocument.Files[0].Containers.Count);
            var group = newDocument.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group);
            Assert.AreEqual("u1-g", group.Id);

            Assert.AreEqual(3, group.Containers.Count);

            var unit1 = group.Containers[0] as Unit;
            var textUnit1 = unit1.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("p", unit1.Name);
            Assert.AreEqual("Hello Word1!", textUnit1);

            var unit2 = group.Containers[1] as Unit;
            var textUnit2 = unit2.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("p", unit2.Name);
            Assert.AreEqual("Hello Word2!", textUnit2);

            var unit3 = group.Containers[2] as Unit;
            var textUnit3 = unit3.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("p", unit3.Name);
            Assert.AreEqual("<![CDATA[Hello <b>Word3</b>!]]>", textUnit3);
        }

        [Test()]
        public void UnorderedListWithoutFormattingPlusParagraphIsTransformedIntoGroupWithManyUnits()
        {
            var xliff = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en-GB"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
    <file id=""f1"">
        <unit id=""u1"">
            <segment>
                <source><![CDATA[<p>Hello World!</p><ul><li>Hello Word1!</li><li>Hello Word2!</li><li>Hello Word3!</li></ul>]]></source>
            </segment>
        </unit>
    </file>
</xliff>";

            XliffDocument document = LoadXliff(xliff);

            var splitter = new ParagraphSplitter();

            var newDocument = splitter.ExecuteExtraction(document);

            Assert.AreEqual(1, newDocument.Files[0].Containers.Count);
            var group = newDocument.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group);
            Assert.AreEqual("u1-g", group.Id);

            Assert.AreEqual(2, group.Containers.Count);

            var pUnit = group.Containers[0] as Unit;
            var pUnitText = pUnit.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("p", pUnit.Name);
            Assert.AreEqual("Hello World!", pUnitText);

            var ulGroup = group.Containers[1] as Group;
            Assert.AreEqual("ul", ulGroup.Name);

            var unit1 = ulGroup.Containers[0] as Unit;
            var textUnit1 = unit1.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("li", unit1.Name);
            Assert.AreEqual("Hello Word1!", textUnit1);

            var unit2 = ulGroup.Containers[1] as Unit;
            var textUnit2 = unit2.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("li", unit2.Name);
            Assert.AreEqual("Hello Word2!", textUnit2);

            var unit3 = ulGroup.Containers[2] as Unit;
            var textUnit3 = unit3.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("li", unit3.Name);
            Assert.AreEqual("Hello Word3!", textUnit3);
        }

        [Test()]
        public void UnorderedListWithItemWithoutFormattingIsTransformedIntoGroupWithManyUnits()
        {
            var xliff = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en-GB"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
    <file id=""f1"">
        <unit id=""u1"" name=""original"">
            <segment>
                <source><![CDATA[<ul><li>Hello Word1!</li></ul>]]></source>
            </segment>
        </unit>
    </file>
</xliff>";

            XliffDocument document = LoadXliff(xliff);

            var splitter = new ParagraphSplitter();

            var newDocument = splitter.ExecuteExtraction(document);

            Assert.AreEqual(1, newDocument.Files[0].Containers.Count);
            var group = newDocument.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group,"Didn't get a Group, but a {0}", newDocument.Files[0].Containers[0].GetType().Name);
            Assert.AreEqual("u1-g", group.Id);

            Assert.AreEqual("original", group.Name);
            Assert.AreEqual(1, group.Containers.Count);

            var ulGroup = group.Containers[0] as Group;
            Assert.AreEqual("ul", ulGroup.Name);
            Assert.AreEqual(1, ulGroup.Containers.Count);

            var unit1 = ulGroup.Containers[0] as Unit;
            var textUnit1 = unit1.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("li", unit1.Name);
            Assert.AreEqual("Hello Word1!", textUnit1);
        }

        [Test()]
        public void UnorderedListWithMultipleItemsWithoutFormattingIsTransformedIntoGroupWithManyUnits()
        {
            var xliff = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en-GB"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
    <file id=""f1"">
        <unit id=""u1"" name=""original"">
            <segment>
                <source><![CDATA[<ul><li>Hello Word1!</li><li>Hello Word2!</li><li>Hello Word3!</li></ul>]]></source>
            </segment>
        </unit>
    </file>
</xliff>";

            XliffDocument document = LoadXliff(xliff);

            var splitter = new ParagraphSplitter();

            var newDocument = splitter.ExecuteExtraction(document);

            Assert.AreEqual(1, newDocument.Files[0].Containers.Count);
            var group = newDocument.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group);
            Assert.AreEqual("u1-g", group.Id);

            Assert.AreEqual("original", group.Name);
            Assert.AreEqual(1, group.Containers.Count);

            var ulGroup = group.Containers[0] as Group;
            Assert.AreEqual("ul", ulGroup.Name);
            Assert.AreEqual(3, ulGroup.Containers.Count);

            var unit1 = ulGroup.Containers[0] as Unit;
            var textUnit1 = unit1.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("li", unit1.Name);
            Assert.AreEqual("Hello Word1!", textUnit1);

            var unit2 = ulGroup.Containers[1] as Unit;
            var textUnit2 = unit2.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("li", unit2.Name);
            Assert.AreEqual("Hello Word2!", textUnit2);

            var unit3 = ulGroup.Containers[2] as Unit;
            var textUnit3 = unit3.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("li", unit3.Name);
            Assert.AreEqual("Hello Word3!", textUnit3);
        }

        [Test()]
        public void NestedUnitIsTransformedIntoGroupWithManyUnits()
        {
            var xliff = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff srcLang=""en-GB"" version=""2.0"" xmlns=""urn:oasis:names:tc:xliff:document:2.0"">
    <file id=""f1"">
        <group id=""g1"">
            <unit id=""u1"">
                <segment>
                    <source><![CDATA[<p>Hello Word nested!</p><p>Hello Word2!</p>]]></source>
                </segment>
            </unit>
        </group>
        <unit id=""u2"">
            <segment>
                <source><![CDATA[<p>Hello Word!</p>]]></source>
            </segment>
        </unit>
    </file>
</xliff>";

            XliffDocument document = LoadXliff(xliff);

            var splitter = new ParagraphSplitter();

            var newDocument = splitter.ExecuteExtraction(document);

            Assert.AreEqual(2, newDocument.Files[0].Containers.Count);
            var group = newDocument.Files[0].Containers[0] as Group;
            Assert.IsNotNull(group);
            Assert.AreEqual("g1", group.Id);

            Assert.AreEqual(1, group.Containers.Count);

            var nestedGroup = group.Containers[0] as Group;

            Assert.IsNotNull(nestedGroup);

            Assert.AreEqual(2, nestedGroup.Containers.Count);

            var unit1 = nestedGroup.Containers[0] as Unit;
            var textUnit1 = unit1.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("p", unit1.Name);
            Assert.AreEqual("Hello Word nested!", textUnit1);

            var unit2 = nestedGroup.Containers[1] as Unit;
            var textUnit2 = unit2.Resources[0].Source.Text[0].ToString();

            Assert.AreEqual("p", unit2.Name);
            Assert.AreEqual("Hello Word2!", textUnit2);
        }


        //Move to actual XliffReader
        private static XliffDocument LoadXliff(string xliff)
        {
            XliffDocument document = null;
            using (Stream stream = GenerateStreamFromString(xliff))
            {
                var reader = new XliffReader();
                document = reader.Deserialize(stream);
            }

            return document;
        }


        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
