using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Extensions.WebApiDependencyResolverExtension;

namespace Common.ApiFilters
{
    public class DistributedTransactionFilter : ActionFilterAttribute
    {
        private const string TransactionId = "TransactionToken";

        private ILogServiceAsync<ILogServiceSettings> _logService;
        public ILogServiceAsync<ILogServiceSettings> LogService
        {
            get
            {
                if (_logService == null)
                    _logService = GlobalConfiguration.Configuration.DependencyResolver.GetService<ILogServiceAsync<ILogServiceSettings>>();

                return _logService;
            }
            set
            {
                _logService = value;
            }
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (actionContext.Request.Headers.Contains(TransactionId))
                    {
                        var values = actionContext.Request.Headers.GetValues(TransactionId);
                        if (values != null && values.Any())
                        {
                            byte[] transactionToken = Convert.FromBase64String(values.FirstOrDefault());
                            var transaction = TransactionInterop.GetTransactionFromTransmitterPropagationToken(transactionToken);
                            var transactionScope = new TransactionScope(transaction);

                            actionContext.Request.Properties.Add(TransactionId, transactionScope);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logService.LogMessage(ex);
                }
            });
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (actionExecutedContext.Request.Properties.Keys.Contains(TransactionId))
                    {
                        var transactionScope = actionExecutedContext.Request.Properties[TransactionId] as TransactionScope;
                        if (transactionScope != null)
                        {
                            if (actionExecutedContext.Exception != null)
                            {
                                Transaction.Current.Rollback();
                            }
                            else
                            {
                                transactionScope.Complete();
                            }

                            transactionScope.Dispose();
                            actionExecutedContext.Request.Properties[TransactionId] = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logService.LogMessage(ex);
                }
            });
        }
    }
}
