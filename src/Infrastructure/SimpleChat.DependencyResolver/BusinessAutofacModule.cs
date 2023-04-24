using Autofac;
using SimpleChat.Business;
using SimpleChat.Business.ServiceQuery;
using SimpleChat.Core.Business_Interface;
using SimpleChat.Core.Business_Interface.ServiceQuery;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.DependencyResolver
{
    public static class BusinessAutofacModule
    {
        public static ContainerBuilder CreateAutofacBusinessContainer(this IServiceCollection services, ContainerBuilder builder)
        {
            builder.RegisterType<IMessageService>().As<MessageService>();
            builder.RegisterType<IMessageServiceQuery>().As<MessageServiceQuery>();
            return builder;
        }
    }

    public class BusinessAutofacModule1 : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageService>().As<IMessageService>();
            builder.RegisterType<MessageServiceQuery>().As<IMessageServiceQuery>();
        }
    }
}
