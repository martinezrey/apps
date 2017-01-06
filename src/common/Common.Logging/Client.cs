using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web;

namespace Common.Logging
{
    public sealed class LogServiceAsync<TOptions> : ILogServiceAsync<TOptions>, IDisposable
       where TOptions : ILogServiceSettings
    {
        private static readonly Lazy<LogServiceAsync<TOptions>> lazy = new Lazy<LogServiceAsync<TOptions>>(() => new LogServiceAsync<TOptions>());

        private readonly ActionBlock<string> _WriterBlock;
        private Stream _Stream;
        private TextWriter _TextWriter;

        public static LogServiceAsync<TOptions> Instance { get { return lazy.Value; } }

        private FileInfo _LogFile;
        public FileInfo LogFile
        {
            get { return _LogFile; }
        }

        public TOptions Options { get; set; }

        private LogServiceAsync()
        {
            Options = (TOptions)Activator.CreateInstance(typeof(TOptions));

            var fileName = Options.GetFileName();

            _LogFile = new FileInfo(Path.Combine(Options.LogDir.FullName, fileName));

            if (Options.LogDir != null && !Options.LogDir.Exists)
                Options.LogDir.Create();

            _WriterBlock = new ActionBlock<string>(async s =>
            {
                if (_Stream == null)
                    _Stream = File.Open(LogFile.FullName, FileMode.Append, FileAccess.Write, FileShare.Read);

                if (_TextWriter == null)
                    _TextWriter = new StreamWriter(_Stream);

                await _TextWriter.WriteLineAsync(s);
                await _TextWriter.FlushAsync();
                await _Stream.FlushAsync();
            });

            _WriterBlock.Completion.ContinueWith(action =>
            {
                if (_TextWriter != null)
                    _TextWriter.Close();

                if (_Stream != null)
                    _Stream.Close();
            });
        }

        ~LogServiceAsync()
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
            if (typeof(String) == message.GetType() || typeof(string) == message.GetType())
                _WriterBlock.Post((String)message);
            else
                _WriterBlock.Post(JsonConvert.SerializeObject(message, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None
                }));

        }

        public void Dispose()
        {

        }
    }
}