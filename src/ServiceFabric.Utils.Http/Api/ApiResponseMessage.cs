﻿using System.Net;

namespace ServiceFabric.Utils.Api
{
    /// <summary>
    /// Strongly typed version of the JSON message generated by <see cref="ApiHttpResponseMessage"/>
    /// </summary>
    /// <typeparam name="TMessageType">Type of the inner message property</typeparam>
    public class ApiResponseMessage<TMessageType>
    {
        public HttpStatusCode Code { get; set; }

        public TMessageType Message { get; set; }

        public string Info { get; set; }
    }
}