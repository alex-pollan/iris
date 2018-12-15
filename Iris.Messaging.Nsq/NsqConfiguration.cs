using Microsoft.Extensions.Configuration;
using System;

namespace Iris.Messaging.Nsq
{    
    public interface INsqConfiguration
    {
        string[] LookupdHttpEndpoints { get; set; }
    }

    public class NsqConfiguration : INsqConfiguration
    {
        public NsqConfiguration(IConfiguration configuration)
        {
            LookupdHttpEndpoints = configuration["vcap:services:user-provided:0:credentials:lookup"]
                .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] LookupdHttpEndpoints { get; set; }
    }
}