using System;

namespace ServiceFabric.Utils.Ipc.Helpers
{
    public class ServiceFabricUriBuilder
    {
        private readonly string _applicationName;
        private readonly string _serviceName;

        public ServiceFabricUriBuilder(string applicationName, string serviceName)
        {
            _applicationName = applicationName;
            _serviceName = serviceName;
        }


        public Uri Build()
        {
            return new Uri($"fabric://{_applicationName}/{_serviceName}");
        }
    }
}