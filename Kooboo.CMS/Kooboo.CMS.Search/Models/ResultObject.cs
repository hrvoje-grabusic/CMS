#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion

using Kooboo.CMS.Content.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Highlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Kooboo.CMS.Search.Models
{
    public class ResultObject
    {
        public Highlighter Highlighter {get;set;}
        public Analyzer analyzer { get; set; }

        public string Title { get; set; }
        public string HighlightedTitle { get; set; }
        public string Body { get; set; }
        public string HighlightedBody { get; set; }

        public string Url { get; set; }

        public object NativeObject { get; set; }

        /// <summary>
        /// Create custom HighlightedBody
        /// </summary>
        /// <param name="fields">Comma seperated fieldanmes "title,description,keywords"</param>
        public IHtmlString HighlightFields(string fields)
        {
            string[] field_names = fields.Split(',');

            TextContent item = ToTextContent();

            // add desired columns
            StringBuilder sb = new StringBuilder();
            foreach(string field in field_names)
            {
                if(item.ContainsKey(field))
                {
                    sb.AppendFormat(" {0} ", Kooboo.StringExtensions.StripAllTags(item[field].ToString()));
                }
            }

            string new_body = string.Join("...", Highlighter.GetBestFragments(analyzer, "_BodyIndex_", sb.ToString(), 5));
            return new HtmlString(new_body);
        }

        public TextContent ToTextContent()
        {
            return (TextContent)NativeObject;
        }
    }
}
