using NsqSharp.Bus.Configuration.BuiltIn;
using System;
using System.Collections.Generic;

namespace Iris.Messaging.Nsq
{
    public class MessageTypeToTopicProvider : MessageTypeToTopicDictionary
    {
        public MessageTypeToTopicProvider(Dictionary<Type, string> messageTopics)
            : base(messageTopics)
        {
        }
    }
}