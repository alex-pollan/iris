using System;
using System.Collections.Generic;

namespace Iris.Messaging.Nsq
{
    public class NsqConfiguration
    {
        public NsqConfiguration(string endpoints, Dictionary<Type, string> messageTypeTopics,
            Dictionary<Type, string> messageHandlerTypeChannels)
        {
            LookupdHttpEndpoints = endpoints.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            MessageTypeTopics = messageTypeTopics;
            MessageHandlerTypeChannels = messageHandlerTypeChannels;
        }

        public string[] LookupdHttpEndpoints { get; set; }
        public Dictionary<Type, string> MessageTypeTopics { get; }
        public Dictionary<Type, string> MessageHandlerTypeChannels { get; internal set; }
    }
}