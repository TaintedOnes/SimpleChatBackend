using SimpleChat.API.Controllers;
using SimpleChat.Core.Business_Interface;
using SimpleChat.Core.Business_Interface.ServiceQuery;
using SimpleChat.Core.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SimpleChat.API.Tests
{
    public class MessageControllerTests
    {
        public Mock<IMessageServiceQuery> messageServiceQuery;
        public Mock<IMessageService> messageService;

        private MessageController sut;
        public MessageControllerTests()
        {
            this.messageServiceQuery = new Mock<IMessageServiceQuery>();
            this.messageService = new Mock<IMessageService>();
            IConfiguration config = null;
            sut = new MessageController(this.messageServiceQuery.Object,this.messageService.Object, config);

        }
        [Fact]
        public void GetAll_should_return_data_saved_in_database()
        {
            var messages = new List<Message>{
                new Message{Id=Convert.ToInt64(Guid.NewGuid()),Content="Hi Bulbul", Sender="Ehasanul", Receiver="Bulbul"},
                new Message{Id=Convert.ToInt64(Guid.NewGuid()),Content="Hi Ehasanul", Sender="Bulbul", Receiver="Ehasanul"},
            };
            this.messageServiceQuery.Setup(x => x.GetAll()).Returns(messages);
            var result = sut.GetAll() as OkObjectResult;
            result.StatusCode.Should().Be(200);
        }
    }
}
