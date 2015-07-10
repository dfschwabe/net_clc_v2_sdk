using System;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CenturyLinkCloudSdk.Tests
{
    public static class TaskExtensions
    {
        public static T Await<T>(this Task<T> task)
        {
            try
            {
                return task.Result;
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            return default(T);
        }
    }
}