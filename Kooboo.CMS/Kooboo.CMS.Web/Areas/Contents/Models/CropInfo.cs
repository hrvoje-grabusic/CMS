using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kooboo.CMS.Web.Areas.Contents.Models
{
    public class CropInfo : Dictionary<string,CropBox>
    {
        public CropInfo() { }

        public CropBox GetPosition(string key)
        {
            if(this.ContainsKey(key))
            {
                return this[key];
            } else{
                CropBox cb = new CropBox();
                this.Add(key, cb);
                return cb;
            }
        }
    }
    public class CropBox
    {
        public int x = 0;
        public int y = 0;
        public int width = 0;
        public int height = 0;

        public void BindValues(FormCollection form)
        {
            x = int.Parse(form["x"]);
            y = int.Parse(form["y"]);
            width = int.Parse(form["width"]);
            height = int.Parse(form["height"]);
        }
    }
}