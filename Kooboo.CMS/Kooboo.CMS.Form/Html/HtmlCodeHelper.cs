#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Kooboo.Web.Url;
using Kooboo.Globalization;
using Kooboo.CMS.Common;
using System.Security.Policy;
using System.Web.Mvc;
using System.Web.Routing;
namespace Kooboo.CMS.Form.Html
{
    public static class HtmlCodeHelper
    {
        /// <summary>
        /// abc"cde => abc""cde, 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EscapeQuote(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return "";
            }
            return s.Replace("\"", "\"\"");
        }
        public static string RazorHtmlEncode(this string s)
        {
            return System.Web.HttpUtility.HtmlEncode(s).Replace("@", "@@");
        }
        public static string RazorHtmlAttributeEncode(this string s)
        {
            return System.Web.HttpUtility.HtmlAttributeEncode(s).Replace("@", "@@");
        }

        public static IHtmlString RenderColumnValue(this object v)
        {
            if (v == null)
            {
                return new HtmlString("-");
            }
            if (v is bool)
            {
                if (((bool)v) == true)
                {
                    return new HtmlString("YES".Localize());
                }
                else
                {
                    return new HtmlString("-");
                }
            }
            if (v is string)
            {
                var s = v.ToString();

                if (s.StartsWith("~/") || s.StartsWith("/") || s.StartsWith("http://"))
                {
                    List<IHtmlString> htmlStrings = new List<IHtmlString>();
                    foreach (var item in s.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        htmlStrings.Add(RenderFile(item));
                    }

                    return new AggregateHtmlString(htmlStrings);
                }
                return new HtmlString(Kooboo.StringExtensions.StripAllTags(s.Trim()));

            }
            else
            {
                return new HtmlString(v.ToString().Trim());
            }
        }

        private static IHtmlString RenderFile(string item)
        {
            var url = UrlUtility.ResolveUrl(item);

            // get request context needed to build action urls
            RequestContext rc = ((MvcHandler)HttpContext.Current.Handler).RequestContext;
            
            // istance UrlHelper so we can build mvc urls
            UrlHelper uh = new UrlHelper(rc);

            // we need the sitename to resize images
            String sitename = (string)HttpContext.Current.Request["siteName"];

            var extension = Path.GetExtension(item).ToLower();

            if (!String.IsNullOrEmpty(sitename) && (extension==".jpg" || extension==".jpeg"))
            {
                url = uh.Action("ResizeImage", "Resource", new { siteName = sitename, url = item, area = "", width = 0, height = 60, preserverAspectRatio = true, quality = 80, t = DateTime.Now.Ticks});
            }

            try
            {

                if (extension == ".gif" || extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".ico")
                {
                    return new HtmlString(string.Format("<img src='{0}' height='60'/>", url));
                }
                else
                {
                    return new HtmlString(string.Format("<a href='{0}'>{0}</a>", url));
                }
            }
            catch
            {
                return new HtmlString(item.Trim());
            }
        }
    }
}
