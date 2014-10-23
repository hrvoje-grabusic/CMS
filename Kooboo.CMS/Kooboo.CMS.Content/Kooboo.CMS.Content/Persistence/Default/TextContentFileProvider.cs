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
using Kooboo.CMS.Content.Models;
using System.IO;
using Kooboo.CMS.Content.Models.Paths;
using Kooboo.Web.Url;
using Kooboo.IO;
using System.Web;

namespace Kooboo.CMS.Content.Persistence.Default
{
    [Kooboo.CMS.Common.Runtime.Dependency.Dependency(typeof(ITextContentFileProvider))]
    public class TextContentFileProvider : ITextContentFileProvider
    {
        #region Save
        public string Save(TextContent content, ContentFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = (Path.GetFileNameWithoutExtension(file.FileName)).ToUrlString() + extension;
            TextContentPath contentPath = new TextContentPath(content);
            string filePath = Path.Combine(contentPath.PhysicalPath, fileName);
            file.Stream.SaveAs(filePath, true);

            return UrlUtility.Combine(contentPath.VirtualPath, fileName);
        } 
        #endregion

        #region DeleteFiles
        public void DeleteFiles(TextContent content)
        {
            var contentPath = new TextContentPath(content);
            try
            {
                if (Directory.Exists(contentPath.PhysicalPath))
                {
                    IOUtility.DeleteDirectory(contentPath.PhysicalPath, true);
                }
            }
            catch (Exception e)
            {
                Kooboo.HealthMonitoring.Log.LogException(e);
            }
        } 
        #endregion

        /// <summary>
        /// After content item is moved to a new folder move the files it references to the new folder. And change the field values.
        /// Still needs
        /// </summary>
        /// <param name="textFolder"></param>
        /// <param name="uuid"></param>
        public virtual TextContent MoveFiles(TextContent content)
        {
            TextContent new_content = new TextContent(content);

            if (content != null)
            {
                //Schema schema = content.GetSchema();
                // check all fields for file references
                foreach (string key in content.Keys)
                {
                    string value = content[key] !=null ? content[key].ToString() : "";
                    if( !String.IsNullOrEmpty(value) && value.StartsWith("~/") )
                    {
                        string filename = Path.GetFileName(value);

                        if (!String.IsNullOrEmpty(filename))
                        {
                            TextContentPath contentPath = new TextContentPath(content);
                            if (contentPath != null)
                            {
                                string old_path = HttpContext.Current.Server.MapPath(value);
                                string new_path = Path.Combine(contentPath.PhysicalPath, filename);

                                if(!Directory.Exists(contentPath.PhysicalPath))
                                {
                                    Directory.CreateDirectory(contentPath.PhysicalPath);
                                }

                                //throw new Exception("old:" + old_path + " new:" + new_path);

                                File.Move(old_path, new_path);

                                new_content[key] = UrlUtility.Combine(contentPath.VirtualPath, filename);
                            }
                            else
                            {
                                //throw new Exception("content path is null");
                            }
                        }
                    }
                }
            }
            return new_content;
        }
    }
}
