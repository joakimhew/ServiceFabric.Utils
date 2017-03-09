using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class HttpCommunicationClientFactory : CommunicationClientFactoryBase<HttpCommunicationClient>
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public HttpCommunicationClientFactory(IServicePartitionResolver resolver = null, IEnumerable<IExceptionHandler> exceptionHandlers = null)
            : base(resolver, CreateExceptionHandlers(exceptionHandlers))
        {
        }

        protected override bool ValidateClient(HttpCommunicationClient client)
        {
            return true;
        }

        protected override bool ValidateClient(string endpoint, HttpCommunicationClient client)
        {
            return true;
        }

        protected override Task<HttpCommunicationClient> CreateClientAsync(string endpoint, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpCommunicationClient(_httpClient, endpoint));
        }

        protected override void AbortClient(HttpCommunicationClient client)
        {
        }

        private static IEnumerable<IExceptionHandler> CreateExceptionHandlers(
            IEnumerable<IExceptionHandler> additionalExceptionHandlers)
        {
            return
                new[] { new HttpExceptionHandler() }.Union(additionalExceptionHandlers ??
                                                           Enumerable.Empty<IExceptionHandler>());
        }
    }
}