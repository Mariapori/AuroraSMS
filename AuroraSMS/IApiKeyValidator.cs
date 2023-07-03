using Microsoft.Extensions.Options;

namespace AuroraSMS
{
    public interface IApiKeyValidator
    {
        bool IsValid(string? apiKey);
    }

    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly IOptions<AuroraSMSConfig> _config;
        public ApiKeyValidator(IOptions<AuroraSMSConfig> config)
        {
            _config = config;
        }

        public bool IsValid(string? apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            var apiKeyFromEnv = System.Environment.GetEnvironmentVariable("apikey");

            if (!string.IsNullOrEmpty(apiKeyFromEnv))
            {
                if(apiKey == apiKeyFromEnv)
                {
                    return true;
                }
            }

            var apiKeyFromConfig = _config.Value.ApiKey;

            if (string.IsNullOrEmpty(apiKeyFromConfig))
            {
                return false;
            }

            if(apiKeyFromConfig != apiKey)
            {
                return false;
            }
            
            return true;
        }
    }
}