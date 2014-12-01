using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Net;
using System.Web.Security;
using System.Security.Policy;

namespace Kooboo.Drawing
{
    public static class MetroImage
    {
		/// <summary>
		/// Read image either from local file or from remote website
		/// </summary>
		/// <param name="image_path"></param>
		/// <param name="target_path"></param>
		/// <returns></returns>
		public static System.Drawing.Image getImage(string image_path,string target_path="")
		{
			Image img = null;
			if (Regex.IsMatch(image_path, @"^https?"))
			{
				img = getRemoteImage(image_path, target_path);
			}
			else
			{
				img = Image.FromFile(image_path);
			}
			return img;
		}

		/// <summary>
		/// Get remote image and cache locally
		/// </summary>
		/// <param name="image_path">Appsolute path to remote image</param>
		/// <param name="targetFilePath">targetFilePath where the image should be cached</param>
		/// <returns>Image object</returns>
		public static Image getRemoteImage(string remote_path, string targetFilePath)
		{
			remote_path = HttpUtility.UrlDecode(remote_path);

			// filename
			string filename = FormsAuthentication.HashPasswordForStoringInConfigFile(remote_path, "MD5") + ".jpg";

			// cache path
			string dir = Path.GetDirectoryName(targetFilePath);
			string save_path = Path.Combine(dir,filename);

			// debug
			//HttpContext.Current.Response.Write(save_path + "- -save_path : dir -" + dir + " targat file path: " + targetFilePath);
			//HttpContext.Current.Response.End();

			// napravi folder ako ne postoji
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			// ako nismo skinuli tu sliku skini je opet
			if (!System.IO.File.Exists(save_path))
			{
				WebClient WebClient = new WebClient();
				byte[] imageBytes;
				try
				{
					imageBytes = WebClient.DownloadData(remote_path);

					// Open file for reading
					System.IO.FileStream _FileStream = new System.IO.FileStream(save_path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
					_FileStream.Write(imageBytes, 0, imageBytes.Length);
					_FileStream.Close();

				}
				catch (WebException ex)
				{
					//HttpContext.Current.Response.Write("remote-path returned 404:" + remote_path);
					//HttpContext.Current.Response.End();

					System.IO.FileStream FileStream = new System.IO.FileStream(save_path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
					//FileStream.Write(imageBytes, 0, imageBytes.Length);
					Stream r_stream = ex.Response.GetResponseStream();
					r_stream.CopyTo(FileStream);
					r_stream.Close();
					FileStream.Close();
				}
				WebClient.Dispose();
			}

			// vrati putanju do lokalne slike
			return Image.FromFile(save_path);
		}

        public static bool SmartSize(string sourceFilePath, string targetFilePath, int width, int height, string vAlign, string hAlign = "center")
        {
            Image img = getImage(sourceFilePath,targetFilePath) ;
            Image target = resize(img, width, height,vAlign,hAlign);
            img.Dispose();

            String extension = System.IO.Path.GetExtension(sourceFilePath).ToLower();

            if (extension !=".png")
            {

                ImageCodecInfo jgpEncoder = ImageTools.GetJpgCodec();
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 98L);
                myEncoderParameters.Param[0] = myEncoderParameter;

                target.Save(targetFilePath, jgpEncoder, myEncoderParameters);

            }
            else
            {
                target.Save(targetFilePath, System.Drawing.Imaging.ImageFormat.Png);
            }
            return true;
        }

		public static bool CropAndResize(string sourceFilePath, string targetFilePath, int x, int y, int width, int height, int destWidth=0, int destHeight=0 )
		{
			Image img = getImage(sourceFilePath, targetFilePath);
            
			Image target = ImageCropAndResize(img, x, y, width, height, destWidth, destHeight);
			img.Dispose();

            String extension = System.IO.Path.GetExtension(sourceFilePath).ToLower();

            if (extension != ".png")
            {

                ImageCodecInfo jgpEncoder = ImageTools.GetJpgCodec();
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 98L);
                myEncoderParameters.Param[0] = myEncoderParameter;

                target.Save(targetFilePath, jgpEncoder, myEncoderParameters);

            }
            else
            {
                target.Save(targetFilePath, System.Drawing.Imaging.ImageFormat.Png);
            }

			return true;
		}

        public static System.Drawing.Image resize(System.Drawing.Image imgPhoto, int Width, int Height,string vAlign="center", string hAlign="center")
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            // max size
            if (Width > 3000)
            {
                Width = 3000;
            }
            if (Height > 3000)
            {
                Height = 3000;
            }

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);

            if (nPercentH > nPercentW) // prvo povecaj manju stranu da nebude bijeloga
            {
                nPercent = nPercentH;
                int w = Width;
                if(w==0)
                {
                    w = (int)Math.Floor(sourceWidth * nPercent);
                }
                if (hAlign != "left" && hAlign != "right")
                {
                    destX = (int)Math.Floor((w - (sourceWidth * nPercent)) / 2);
                }
                else if (hAlign == "right")
                {
                    destX = (int)Math.Ceiling((w - (sourceWidth * nPercent)));
                }
                //sourceX = System.Convert.ToInt16((sourceWidth - Width) / 2);
            }
            else
            {
                nPercent = nPercentW;
                int h = Height;
                if (h == 0)
                {
                    h = (int)Math.Floor(sourceHeight * nPercent);
                }
                if (vAlign != "top" && vAlign != "bottom")
                {
                    destY = (int)Math.Floor((Height - (sourceHeight * nPercent)) / 2);
                }
                else if (vAlign == "bottom")
                {
                    destY = (int)Math.Ceiling((Height - (sourceHeight * nPercent)) );
                }
                
                //sourceY = System.Convert.ToInt16((sourceHeight - Height) / 2);
            }

            int destWidth = (int)Math.Ceiling(sourceWidth * nPercent);
			int destHeight = (int)Math.Ceiling(sourceHeight * nPercent);

			int bmpWidth = Width > 0 ? Width : destWidth;
			int bmpHeight = Height > 0 ? Height : destHeight;

            Bitmap bmPhoto = new Bitmap(bmpWidth, bmpHeight, PixelFormat.Format32bppArgb); ;
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //grPhoto.Clear(Color.Transparent);
			grPhoto.CompositingMode = CompositingMode.SourceCopy;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX-1, destY-1, destWidth+1, destHeight+1),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

		public static System.Drawing.Image ImageCropAndResize(System.Drawing.Image imgPhoto, int x, int y, int Width, int Height, int destWidth=0, int destHeight=0)
		{
			int sourceWidth = imgPhoto.Width;
			int sourceHeight = imgPhoto.Height;
			int destX = 0;
			int destY = 0;

			// zadane dimenzije nesmiju biti vece od izvornih
			if (x < 0) { x = 0; }
			if (y < 0) { y = 0; }
			if (Width > sourceWidth) { Width = sourceWidth; }
			if (Height > sourceHeight) { Height = sourceHeight; }

			float nPercentW = ((float)destWidth / (float)Width);
			float nPercentH = ((float)destHeight / (float)Height);

			if (destWidth == 0 && destHeight == 0)
			{
				destWidth = Width;
				destHeight = Height;
			}
			else
			{
				nPercentW = ((float)destWidth / (float)Width);
				nPercentH = ((float)destHeight / (float)Height);

				if (destHeight > 0 && destWidth==0)
				{
					destWidth = (int)(Width * nPercentH);
				} else if (destWidth > 0 && destHeight==0)
				{
					destHeight = (int)(Height * nPercentW);
				}
			}

            Bitmap bmPhoto = bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppArgb); ;

			bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

			Graphics grPhoto = Graphics.FromImage(bmPhoto);
			//grPhoto.Clear(Color.Transparent);
			grPhoto.CompositingMode = CompositingMode.SourceCopy;
			grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage(imgPhoto,
					new Rectangle(destX - 1, destY - 1, destWidth + 2, destHeight + 2),
					new Rectangle(x, y, Width, Height),
					GraphicsUnit.Pixel);

			grPhoto.Dispose();
			return bmPhoto;
		}
    }
}
