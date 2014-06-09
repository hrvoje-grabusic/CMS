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


namespace Kooboo.CMS.Form.Html.Controls
{
    public class TextContentPicker : Input
    {
        public override string Name
        {
            get { return "TextContentPicker"; }
        }
        public override string Type
        {
            get { return "hidden"; }
        }


        /// <summary>
        /// false
        /// </summary>
        protected virtual bool AllowMultipleFiles
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// /^.+\..+$/
        /// </summary>
        protected virtual string Validation
        {
            get
            {
                return @"/^.+\..+$/";
            }
        }

        /// <summary>
        /// Please select a valid file
        /// </summary>
        protected virtual string ValidationErrorMessage
        {
            get
            {
                return "Please select a valid file";
            }
        }

        protected override string RenderInput(Kooboo.CMS.Form.IColumn column)
        {
            string input = base.RenderInput(column);

            string foldername = "slike";
            column.CustomSettings.TryGetValue("folderName", out foldername);
            string SingleChoice =  "false";
            column.CustomSettings.TryGetValue("SingleChoice", out SingleChoice);

            // <%:Url.Action("Selection", "MediaContent", ViewContext.RequestContext.AllRouteValues())%>
            /*string mediaLibraryUrl = @"<%:Url.Action(""Selection"", ""MediaContent"", new {
                        repositoryName = Kooboo.CMS.Sites.Models.Site.Current.Repository,
                        siteName = Kooboo.CMS.Sites.Models.Site.Current.Name
                    })%>";*/
            string mediaLibraryUrl = @"@Url.Action(""SelectCategories"", ""TextContent"", new {
                        repositoryName = Kooboo.CMS.Sites.Models.Site.Current.Repository,
                        siteName = Kooboo.CMS.Sites.Models.Site.Current.FullName,
                        folderName = ""#folderName#"",
                        SingleChoice = ""#singlechoice#"",
                        TextContentMode = ""true""
                    })";
            mediaLibraryUrl = mediaLibraryUrl.Replace("#folderName#", foldername);
            mediaLibraryUrl = mediaLibraryUrl.Replace("#singlechoice#", SingleChoice);
            
            input = String.Format(@"
                <div id='textcontentpicker_{0}' class='textcontentpicker category-list clearfix' data-folderName='{2}'>
                    <ul data-bind='sortable: data'>
                        <li class='category-item-data'>
                            <!-- ko if: thumb -->
                            <img data-bind='attr:{{src: thumb}}' /> 
                            <!-- /ko -->
                            <span class='text left' data-bind='{{text:Text}}'></span>
                            <a class='remove right' data-bind='{{click: $parent.removeItem}}'>@Html.IconImage(""minus small"")</a>
                        </li>
                    </ul>
                    <input type='hidden' id='{0}' name='{0}' data-bind='{{value:postValue}}' value='@(Model.{0} ?? """")'/>
                    <a columnName='{0}' id=""textcontentpicker_btn_{0}""
                        href='{1}' class='action textcontentpickerButton'>@Html.IconImage(""plus small"")</a>
                </div>
                <script>
                $(function(){{
                    textcontentpicker('#textcontentpicker_{0}')
                }})
                </script>
            ", column.Name, mediaLibraryUrl, foldername, column.DefaultValue);
            return input;
        }
    }
}
