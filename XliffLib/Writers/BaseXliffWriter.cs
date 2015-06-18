﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XliffLib.Writers
{
    public abstract class BaseXliffWriter
    {
        /// <summary>
        /// Underlying XML document
        /// </summary>
        public XDocument XliffDoc { get; protected set; }

        /// <summary>
        /// Returns the XML string of the XLIFF file
        /// </summary>
        /// <returns>string with XML</returns>
        public string ToXmlString()
        {
            var wr = new StringWriter();
            XliffDoc.Save(wr);
            return wr.GetStringBuilder().ToString();
        }
    }
}
