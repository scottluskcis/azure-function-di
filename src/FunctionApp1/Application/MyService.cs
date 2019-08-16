using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FunctionApp1.Application
{
    public class MyService : IMyService
    {
        private readonly CustomConfig _config;
        private readonly ILogger _logger;

        public MyService(IOptions<CustomConfig> options, ILogger<MyService> logger)
        {
            _config = options?.Value ?? new CustomConfig();
            _logger = logger;
        }

        #region Implementation of IMyService

        public string FormatMessage(string message)
        {
            _logger.LogInformation("format message called");

            if(string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            return $"{_config.MessagePrefix} - from {nameof(MyService)} - message: {message}";
        }

        #endregion
    }
}
