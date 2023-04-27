using Microsoft.AspNetCore.Mvc;
using SimpleChat.Core.Business_Interface.ServiceQuery;

namespace SimpleChat.API.Controllers
{

    [Route("api/conversation")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationServiceQuery conversationServiceQuery;
        public ConversationController(IConversationServiceQuery conversationServiceQuery)
        {
            this.conversationServiceQuery = conversationServiceQuery;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var conversations = this.conversationServiceQuery.GetAllConversation();
            return Ok(conversations);
        }
    }
}