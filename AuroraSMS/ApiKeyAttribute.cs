using Microsoft.AspNetCore.Mvc;

namespace AuroraSMS
{
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute(): base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
