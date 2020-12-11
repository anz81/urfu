using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using Urfu.Its.Common;

namespace Urfu.Its.Web.DataContext
{
    /// <summary>
    /// Defines a simple interface for scheduling background tasks. Useful for UnitTesting ASP.net code
    /// </summary>
    public interface ITaskScheduler
    {
        /// <summary>
        /// Schedules a task which can run in the background, independent of any request.
        /// </summary>
        /// <param name="workItem">A unit of execution.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        void QueueBackgroundWorkItem(Action<CancellationToken> workItem);

        /// <summary>
        /// Schedules a task which can run in the background, independent of any request.
        /// </summary>
        /// <param name="workItem">A unit of execution.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
    }


    public class BackgroundTaskScheduler : BackgroundService, ITaskScheduler
    {
        public BackgroundTaskScheduler()
        {
            var l = new Logger();
            _logger = (ILogger)l.GetLogger();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogTrace("BackgroundTaskScheduler Service started.");

            _stoppingToken = stoppingToken;

            _isRunning = true;
            try
            {
                await Task.Delay(-1, stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                _isRunning = false;
                _logger.LogTrace("BackgroundTaskScheduler Service stopped.");
            }
        }

        public void QueueBackgroundWorkItem(Action<CancellationToken> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            if (!_isRunning)
                throw new Exception("BackgroundTaskScheduler is not running.");

            _ = Task.Run(() => workItem(_stoppingToken), _stoppingToken);
        }

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            if (!_isRunning)
                throw new Exception("BackgroundTaskScheduler is not running.");

            _ = Task.Run(async () =>
            {
                try
                {
                    await workItem(_stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "When executing background task.");
                    throw;
                }
            }, _stoppingToken);
        }

        private readonly ILogger _logger;
        private volatile bool _isRunning;
        private CancellationToken _stoppingToken;
    }
}
