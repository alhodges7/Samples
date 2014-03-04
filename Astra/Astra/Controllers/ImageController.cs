using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Astra.CompositeRepository;
using Astra.Controllers.Shared;
using Astra.Models;
using Astra.Helper;

namespace Astra.Controllers
{
    public class ImageController : BaseController
    {
      private const int TARGET_THUMBNAIL_SIZE = 150;

    [HttpGet]
    public ActionResult UploadImageDialog(int resourceID = 0)
    {
      return PartialView("../Admin/_UploadImageDialog", resourceID as object);
    }

    [HttpGet]
    public ActionResult UploadCoverImageDialog(int resourceID = 0)
    {
      return PartialView("../Admin/_UploadCoverImageDialog", resourceID as object);
    }

    [HttpPost]
    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UploadImageOperation(int resourceId, string caption, bool isCoverImage, IEnumerable<HttpPostedFileBase> files)
    {
      if (Request.Files.Count > 0)
      {
        var testFile = Request.Files[0];

        byte[] byteData = new byte[testFile.ContentLength];
        testFile.InputStream.Read(byteData, 0, byteData.Length);

        byte[] thumbnailByteData = null;
        ResizeImageFromByteData(byteData, ref thumbnailByteData, TARGET_THUMBNAIL_SIZE);

        ResourceImage newImage = new ResourceImage
        {
          Caption = caption,
          ContentType = testFile.ContentType,
          ImageData = byteData,
          ImageThumbnail = thumbnailByteData
        };

        using (var repository = new ScopedCompositeRepository().Repositories.MiscRepository)
        {
          repository.AddImage(resourceId, newImage, isCoverImage);
        }
      }

      return PartialView("../Admin/_ImageUploadTriggerPage", isCoverImage);
    }

    public ActionResult DeleteImageOperation(int imageId)
    {
      using (var repository = new ScopedCompositeRepository().Repositories.MiscRepository)
      {
        repository.DeleteImage(imageId);
      }

      return null;
    }

    public ActionResult DeleteCoverImageOperation(int resourceId)
    {
      using (var repository = new ScopedCompositeRepository().Repositories.MiscRepository)
      {
        repository.DeleteCoverImage(resourceId);
      }

      return null;
    }

    private bool ResizeImageFromByteData(byte[] byteData, ref byte[] returnData,
        int targetDimension)
    {
      try
      {
        using (Image img = Image.FromStream(new MemoryStream(byteData)))
        {
          int width = 0, height = 0;
          GetResizedDimensions(img.Width, img.Height, ref width, ref height, targetDimension);
          Image thumbNail = new Bitmap(width, height, img.PixelFormat);
          Graphics g = Graphics.FromImage(thumbNail);
          g.CompositingQuality = CompositingQuality.HighQuality;
          g.SmoothingMode = SmoothingMode.HighQuality;
          g.InterpolationMode = InterpolationMode.HighQualityBicubic;
          Rectangle rect = new Rectangle(0, 0, width, height);
          g.DrawImage(img, rect);

          MemoryStream outputMemoryStream = new MemoryStream();
          thumbNail.Save(outputMemoryStream, System.Drawing.Imaging.ImageFormat.Gif);
          returnData = outputMemoryStream.ToArray();
        }
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private void GetResizedDimensions(
        int initialWidth, int initialHeight,
        ref int newWidth, ref int newHeight,
        int targetDimension)
    {
      if (initialWidth > initialHeight)
      {
        newWidth = targetDimension;
        newHeight = (int)(((double)initialHeight / (double)initialWidth) * targetDimension);
      }
      else
      {
        newHeight = targetDimension;
        newWidth = (int)(((double)initialWidth / (double)initialHeight) * targetDimension);
      }
    }
  }
}
