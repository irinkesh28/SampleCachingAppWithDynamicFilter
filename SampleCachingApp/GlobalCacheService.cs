namespace SampleCachingApp
{
    public class GlobalCacheService
    {
        private static readonly Lazy<GlobalCacheService> lazyGlobalCacheService = new(() => new GlobalCacheService());

        public static GlobalCacheService globalCacheService => lazyGlobalCacheService.Value;

        public List<string> Keys { get; }

        private GlobalCacheService()
        {
            Keys = [];
        }
    }
}
