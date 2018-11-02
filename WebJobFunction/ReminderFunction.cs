using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace WebJobFunction
{
    public static class ReminderFunction
    {
        private static string _url = "https://volly.azurewebsites.net/api/WebjobApi";
        private static int _retry = 3;

        [FunctionName("ReminderFunction")]
        public static void Run([TimerTrigger("0 30 12 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var client = new HttpClient();

            HttpResponseMessage message;
            int count = 0;
            do
            {
                message = client
                    .PostAsync(_url, null)
                    .Result;
                count++;
            } while (!message.IsSuccessStatusCode && count < _retry);
        }
    }
}
