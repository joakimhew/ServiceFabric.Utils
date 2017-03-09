using System;
using System.Fabric;
using System.Net.Http;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ServiceFabric.Utils.Ipc.Http
{
    /// <summary>
    /// Used for IPC between services within the service fabric cluster(s)
    /// https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-connect-and-communicate-with-services
    /// </summary>
    public class HttpCommunicationClient : ICommunicationClient
    {
        public HttpCommunicationClient(HttpClient client, string address)
        {
            this.HttpClient = client;
            this.Url = new Uri(address);
        }

        public HttpClient HttpClient { get; }

        public Uri Url { get; }

        ResolvedServiceEndpoint ICommunicationClient.Endpoint { get; set; }

        string ICommunicationClient.ListenerName { get; set; }

        ResolvedServicePartition ICommunicationClient.ResolvedServicePartition { get; set; }
    }
}