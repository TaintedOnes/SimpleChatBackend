using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SimpleChat.API.Hubs;
using SimpleChat.Core.Business_Interface;
using SimpleChat.Core.Business_Interface.ServiceQuery;
using SimpleChat.Core.Model;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace SimpleChat.API.Controllers
{

    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageServiceQuery messageServiceQuery;
        private readonly IMessageService messageService;
        private readonly string botToken;
        public MessageController(IMessageServiceQuery messageServiceQuery,IMessageService messageService, IConfiguration config)
        {
            this.messageServiceQuery = messageServiceQuery;
            this.messageService = messageService;
            this.botToken = config.GetValue<string>("BotConfiguration:BotToken");
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var messages = this.messageServiceQuery.GetAll();
            return Ok(messages);
        }
        
        
        [HttpGet("received-messages/{userId}")]
        public IActionResult GetUserReceivedMessages(string userId)
        {
            var messages = this.messageServiceQuery.GetReceivedMessages(userId);
            return Ok(messages);
        }
        [HttpPost()]
        public async Task<IActionResult> DeleteMessage([FromBody]MessageDeleteModel messageDeleteModel)
        {
            var message=await this.messageService.DeleteMessage(messageDeleteModel);
            return Ok(message);
        }

        [HttpPost("uploadImg")]
        public IActionResult UploadImage([FromForm]ImageUploadModel imgUploadModel)
        {
            if (imgUploadModel.ImgFile != null && imgUploadModel.ImgFile.Length > 0)
            {
                try
                {
                    string fileName = imgUploadModel.ImgFile.FileName;
                    BotClient bot = new BotClient(botToken);
                    Message message = bot.SendPhoto(
                        chatId: imgUploadModel.ChatID,
                        photo: new InputFile(new StreamContent(imgUploadModel.ImgFile.OpenReadStream()), fileName)
                    );

                    var result = new
                    {
                        result = GetPhoto(message.Photo[^1].FileId)
                    };
                    return Ok(JsonConvert.SerializeObject(result));
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            return BadRequest();
        }

        private string GetPhoto(string photoID)
        {
            HttpClient client = new HttpClient();
            string photoApiUrl = string.Format("https://api.telegram.org/bot{0}/getFile?file_id={1}", botToken, photoID);
            HttpResponseMessage response = Task.Run(async () => await client.GetAsync(photoApiUrl)).Result;
            string photoPath = "";
            if (response.IsSuccessStatusCode)
            {
                string resContent = Task.Run(async () => await response.Content.ReadAsStringAsync()).Result;
                TelegramPhotoResponse test = JsonConvert.DeserializeObject<TelegramPhotoResponse>(resContent);
                if (test.Ok)
                {
                    photoPath = test.Result.FilePath;
                }
            }

            if (!string.IsNullOrEmpty(photoPath))
            {
                return string.Format("https://api.telegram.org/file/bot{0}/{1}", botToken, photoPath);
            }
            return "";
        }
    }
}