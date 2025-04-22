using Weekly.Infrastructure;

namespace Weekly.Services
{
    public sealed partial class WeeklyService
    {
        private readonly CacheManager _cacheManager;
        private readonly ApiManager _apiManager;
        private readonly FileManager _fileManager;
        private readonly TokenManager _tokenManager;
        private readonly Configuration _configuration;

        public WeeklyService(CacheManager cacheManager, ApiManager apiManager, FileManager fileManager, TokenManager tokenManager, Configuration configuration)
        {
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
            _apiManager = apiManager ?? throw new ArgumentNullException(nameof(apiManager));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _tokenManager = tokenManager ?? throw new ArgumentNullException(nameof(tokenManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
    }
}
