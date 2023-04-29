using Microsoft.AspNetCore.Http;

namespace SimpleChat.Core.Model
{
    public class ImageUploadModel
    {
        public string ChatID { set; get; }
        public IFormFile? ImgFile { set; get; }
    }
}
