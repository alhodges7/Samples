using System;
using System.Linq;
using System.Web;
using Astra.CompositeRepository;
using Astra.Models;

namespace Astra.Handlers
{
    public class ImageHttpHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string filename = System.IO.Path.GetFileNameWithoutExtension(context.Request.FilePath);

            string[] splitString = filename.Split(new char[] { '_' });

            int imageId = int.Parse(splitString[0]);

            ResourceImage targetImage = null;

            using (var repository = new ScopedCompositeRepository().Repositories.MiscRepository)
            {
              targetImage = repository.FindImage(imageId);
            }

            if (targetImage == null)
            {
              // do nothing
            }
            else if (splitString[1].ToLower() == "thumb")
            {
                context.Response.ContentType = "image/gif"; // our thumbnails are stored as gif's
                context.Response.OutputStream.Write(targetImage.ImageThumbnail, 0, targetImage.ImageThumbnail.Length);
            }
            else if (splitString[1].ToLower() == "full")
            {
                context.Response.ContentType = targetImage.ContentType;
                context.Response.OutputStream.Write(targetImage.ImageData, 0, targetImage.ImageData.Length);
            }
        }
    }
}