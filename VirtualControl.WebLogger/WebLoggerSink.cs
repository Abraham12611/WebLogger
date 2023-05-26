﻿using System;
using Serilog.Core;
using Serilog.Events;

namespace VirtualControl.WebLogger
{
    /// <summary>
    /// Weblogger sink that prints to the weblogger output
    /// </summary>
    /// <seealso cref="Serilog.ILogger" />
    /// <seealso cref="IFormatProvider" />
    /// <seealso cref="System.IDisposable" />
    public sealed class WebloggerSink : ILogEventSink, IDisposable
    {

        private bool _disposed;
        private readonly WebLogger _logger;
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebloggerSink"/> class.
        /// Provide an instance of the weblogger to be utilized by the sink.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="logger">The logger.</param>
        public WebloggerSink(IFormatProvider formatProvider, WebLogger logger)
        {
            _formatProvider = formatProvider;
            _logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebloggerSink"/> class.
        /// Alternatively the Serilog sink can instantiate the weblogger and manage it for the application lifetime.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="port"></param>
        /// <param name="secured"></param>
        /// <param name="applicationDirectory"></param>
        public WebloggerSink(IFormatProvider formatProvider, int port, bool secured, string applicationDirectory)
        {
            _logger = new WebLogger(port,  secured, applicationDirectory);
            _logger.Start();

            _formatProvider = formatProvider;
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {

            var data = $"{logEvent.Timestamp} [{logEvent.Level.ToString().ToUpper()}] {logEvent.RenderMessage(_formatProvider)}";

            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                    _logger.WriteLine(data);
                    break;
                case LogEventLevel.Debug:
                    _logger.WriteLine(data);
                    break;
                case LogEventLevel.Information:
                    _logger.WriteLine(data);
                    break;
                case LogEventLevel.Warning:
                    _logger.WriteLine(data);
                    break;
                case LogEventLevel.Error:
                    _logger.WriteLine(data);
                    break;
                case LogEventLevel.Fatal:
                    _logger.WriteLine(data);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _logger?.Dispose();
            }

            _disposed = true;
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~WebloggerSink()
        {

        }
    }
}
