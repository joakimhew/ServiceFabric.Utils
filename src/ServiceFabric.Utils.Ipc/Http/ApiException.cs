﻿using System;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class ApiException : Exception
    {
        public ApiException(string message, string stackTrace = null)
            : base(message)
        {
            StackTrace = stackTrace;
        }

        public override string StackTrace { get; }
    }
}