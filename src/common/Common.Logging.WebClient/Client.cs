using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Common.Logging;
using System.IO;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using EndpointClients.LoggingApi;
using System.Configuration;
using Common.ApiModels;

namespace Common.Logging.WebClient
{
    public sealed class ApiLogService : ILogServiceAsync<ApiLogServiceOptions>, IDisposable
    {
        private static readonly Lazy<ApiLogService> lazy = new Lazy<ApiLogService>(() => new ApiLogService());
        private readonly ActionBlock<string> _WriterBlock;

        public static ApiLogService Instance { get { return lazy.Value; } }

        public ApiLogServiceOptions Options { get; set; }

        private ApiLogService()
        {
            _WriterBlock = new ActionBlock<string>(async s =>
            {
                await Options.LogClient.EndpointLogMessage.Create(new LogMessageModel
                    {
                        Message = s
                    });
            });
        }

        ~ApiLogService()
        {
            if (_WriterBlock != null)
            {
                _WriterBlock.Complete();
                _WriterBlock.Completion.Wait();
            }
        }

        public void LogException(Exception ex)
        {
            _WriterBlock.Post(String.Format("EXCEPTION THROWN!!!!! {0}", JsonConvert.SerializeObject(ex)));
        }


        public void LogMessage(dynamic message)
        {
            if (InDebugMode())
                _WriterBlock.Post(JsonConvert.SerializeObject(message));
        }

        private bool InDebugMode()
        {
            ConfigurationManager.RefreshSection("appSettings");

            var appSettings = ConfigurationManager.AppSettings;

            bool isInDebugMode = false;

            if (bool.TryParse(appSettings["isDebugTurnedOn"], out isInDebugMode))
                return isInDebugMode;

            return false;
        }

        public void Dispose()
        {
            if (_WriterBlock != null)
            {
                _WriterBlock.Complete();
                _WriterBlock.Completion.Wait();
            }
        }
    }
}
