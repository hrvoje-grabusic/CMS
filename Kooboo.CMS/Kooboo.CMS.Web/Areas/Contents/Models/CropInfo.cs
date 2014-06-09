using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kooboo.CMS.Web.Areas.Contents.Models
{
    public class CropInfo : Dictionary<string,CropBox>
    {
        public CropInfo() { }
    }
    public class CropBox
    {
        public int x = 0;
        public int y = 0;
        public int width = 0;
        public int height = 0;
    }
}