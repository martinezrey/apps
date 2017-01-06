using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Common.Logging.SqlServerClient
{
    public class SqlServerLogServiceSettings : ILogServiceSettings
    {
        public bool IsUtcDateUsed {get; private set;}

        public DirectoryInfo LogDir
        {
            get
            {
                ConfigurationManager.RefreshSection("appSettings");

                return new DirectoryInfo(ConfigurationManager.AppSettings["logDirectory"]);
            }
        }

        public Func<string> GetFileName
        {
            get { throw new NotImplementedException(); }
        }

        public SqlServerLogServiceSettings()
        {
            IsUtcDateUsed = false;
        }

}

    public sealed class SqlServiceLoggingClient : ILogServiceAsync<SqlServerLogServiceSettings>, IDisposable
    {
        private static readonly Lazy<SqlServiceLoggingClient> lazy = new Lazy<SqlServiceLoggingClient>(() => new SqlServiceLoggingClient());
        private static readonly object _SyncLogObject = new object();
        private readonly ActionBlock<string> _WriterBlock;
        private Stream _Stream;
        private TextWriter _TextWriter;

        public static SqlServiceLoggingClient Instance { get { return lazy.Value; } }

        private FileInfo _LogFile;
        public FileInfo LogFile
        {
            get { return _LogFile; }
        }

        public SqlServerLogServiceSettings Options { get; set; }

        private SqlServiceLoggingClient()
        {
            Options = new SqlServerLogServiceSettings();

            _LogFile = new FileInfo(Path.Combine(Options.LogDir.FullName,
                                                 String.Format("{0}.txt", Options.IsUtcDateUsed ? DateTime.UtcNow.ToString("MM.dd.yyyy")
                                                                                                : DateTime.Now.ToString("MM.dd.yyyy"))));

            if (InDebugMode() && !Options.LogDir.Exists)
                Options.LogDir.Create();

            _Stream = File.Open(LogFile.FullName, FileMode.Append, FileAccess.Write, FileShare.Read);
            _TextWriter = new StreamWriter(_Stream);

            _WriterBlock = new ActionBlock<string>(async s =>
            {
                var insertSucceeded = false;

                try
                {
                    using (var context = new ApplicationSecurityEntities())
                    {
                        var t = new Log
                        {
                            appId = 1,
                            data = s,
                        };

                        context.Logs.Add(t);
                        context.SaveChanges();

                        insertSucceeded = true;
                    }
                }
                catch(Exception ex)
                {
                    insertSucceeded = false;
                }


                if (insertSucceeded)
                    return;

                await _TextWriter.WriteLineAsync(String.Format("{0}: {1}", Options.IsUtcDateUsed ? DateTime.UtcNow.ToString("MM.dd.yyy HH:mm:ss.fff")
                                                                            : DateTime.Now.ToString("MM.dd.yyy HH:mm:ss.fff"),
                                                                     s));
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

        ~SqlServiceLoggingClient()
        {
            if (_WriterBlock != null)
            {
                LogMessage(new
                {
                    msg = "destructor disposing logger",
                });

                _WriterBlock.Complete();
                _WriterBlock.Completion.Wait();
            }
        }

        public void LogException(Exception ex)
        {
            _WriterBlock.Post(String.Format("EXCEPTION THROWN!!!!! {0}", JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(ex))));
        }

        public void LogMessage(dynamic message)
        {
            if (InDebugMode())
            {
                var logMessage = new
                {
                    logMessage = message,
                };

                var json = JsonConvert.SerializeObject(logMessage);

                var xml = JsonConvert.DeserializeXmlNode(json).OuterXml;

                _WriterBlock.Post(xml);
            }
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
                LogMessage(new
                {
                    msg = "dispose disposing logger",
                });

                _WriterBlock.Complete();
                _WriterBlock.Completion.Wait();
            }
        }
    }
}
