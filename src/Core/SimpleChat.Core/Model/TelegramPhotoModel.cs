using Newtonsoft.Json;

namespace SimpleChat.Core.Model
{
    public class TelegramPhotoResponse
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }
        [JsonProperty("result")]
        public TelegramPhotoResult Result { get; set; }
    }

    public class TelegramPhotoResult
    {
        [JsonProperty("file_id")]
        public string FileID { get; set; }
        [JsonProperty("file_unique_id")]
        public string FileUniqueID { get; set; }
        [JsonProperty("file_size")]
        public int FileSize { get; set; }
        [JsonProperty("file_path")]
        public string FilePath { get; set; }
    }
}
