#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Kooboo.CMS.Common.Persistence.Non_Relational;
using Kooboo.CMS.Content.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Kooboo.Drawing;
using System.IO;

namespace Kooboo.CMS.Content.Persistence.Default
{
    public static class TextContentFileHelper
    {
        public static void StoreFiles(this TextContent content)
        {
            var schema = content.GetSchema();
            if (content.ContentFiles != null)
            {
                var textContentFileProvider = Providers.DefaultProviderFactory.GetProvider<ITextContentFileProvider>();
                schema = schema.AsActual();
                Dictionary<string, string> fileFields = new Dictionary<string, string>();
                foreach (var file in content.ContentFiles)
                {
                    Column column = schema[file.Name];

                    // BATCH UPLOAD napravi title ako nije zadan
                    if ( string.IsNullOrEmpty((string)content["Title"]) )
                    {
                        // napravi title od filenama
                        string title = file.FileName.Replace("-", " ").Replace("_", " ");
                        title = Regex.Replace(title,@"\.[^\.]+$","");
                        content["Title"] = title;
                    }

                    // ako nije pronađena columna probaj ju naći
                    if(column==null)
                    {
                        
                        // ako je slika probaj naći image crop 
                        bool isImage = ImageTools.IsImageExtension(Path.GetExtension(file.FileName));
                        if(isImage)
                        {
                            column = schema.Columns.Find(i => i.ControlType == "ImageCrop");
                        }

                        // ako je još uvjek null probaj naći file controlu
                        if (column == null)
                        {
                            column = schema.Columns.Find(i => i.ControlType == "File");
                        }
                    }

                    if (column != null)
                    {
                        if (file.Stream.Length > 0 && !string.IsNullOrEmpty(file.FileName))
                        {
                            var fileVirtualPath = textContentFileProvider.Save(content, file);//hrvoje remove Url.ResolveUrl()
                            var value = content[file.Name] == null ? "" : content[file.Name].ToString();
                            if (fileFields.ContainsKey(file.Name))
                            {
                                value = fileFields[file.Name];
                            }

                            if (value == null || string.IsNullOrEmpty(value.ToString()))
                            {
                                value = fileVirtualPath;
                            }
                            else
                            {
                                value = value.ToString().Trim('|') + "|" + fileVirtualPath;
                            }
                            fileFields[column.Name] = value;
                        };

                    }

                }
                foreach (var item in fileFields)
                {
                    content[item.Key] = item.Value;
                }
            }
        }

        public static void DeleteFiles(this TextContent content)
        {
            Providers.DefaultProviderFactory.GetProvider<ITextContentFileProvider>().DeleteFiles(content);
        }

        public static void MoveFiles(this TextContent content)
        {
            Providers.DefaultProviderFactory.GetProvider<ITextContentFileProvider>().MoveFiles(content);
        }
    }
}
