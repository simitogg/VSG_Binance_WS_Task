using Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace RequestHandler.BusinessLayer
{
    public class BL_Calculations : IBL_Calculations
    {
        private readonly SharedContext _context;
        private readonly IMemoryCache _cache;
        private readonly int _cacheExpirationMinutes;

        public BL_Calculations(SharedContext context, IMemoryCache cache, IConfiguration configuration)
        {
            _context = context;
            _cache = cache;
            _cacheExpirationMinutes = configuration.GetValue<int>("CacheSettings:SlidingExpirationMinutes");
        }

        public async Task<decimal?> Get24hAvgPriceAsync(string symbol)
        {
            var endTime = DateTime.UtcNow;
            var startTime = endTime.AddDays(-1);

            var cacheKey = $"{symbol}-24hAvgPrice";
            if (!_cache.TryGetValue(cacheKey, out decimal? averagePrice))
            {
                // Fetch the records for the last 24 hours
                var records = await _context.SymbolRecords
                    .Where(p => p.Symbol == symbol && p.Timestamp >= startTime)
                    .ToListAsync();

                // If there are no records for the last 24 hours, fetch the oldest available records
                if (!records.Any())
                {
                    records = await _context.SymbolRecords
                        .Where(p => p.Symbol == symbol)
                        .OrderBy(p => p.Timestamp)
                        .ToListAsync();
                }

                // Calculate the average price
                if (records.Any())
                {
                    averagePrice = records.Average(p => p.Price);
                }

                // Cache the result
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheExpirationMinutes));
                _cache.Set(cacheKey, averagePrice, cacheEntryOptions);
            }

            return averagePrice;
        }

        public async Task<decimal?> GetSimpleMovingAverageAsync(string symbol, int numberOfDataPoints, string timePeriod, DateTime? startDateTime = null)
        {
            var cacheKey = $"{symbol}-SMA-{numberOfDataPoints}-{timePeriod}-{startDateTime}";
            if (!_cache.TryGetValue(cacheKey, out decimal? sma))
            {
                var query = _context.SymbolRecords.AsQueryable();

                if (startDateTime.HasValue)
                {
                    query = query.Where(p => p.Timestamp >= startDateTime.Value);
                }

                switch (timePeriod)
                {
                    case "1d":
                        query = query.Where(p => p.Timestamp >= DateTime.UtcNow.AddDays(-numberOfDataPoints));
                        break;
                    case "1w":
                        query = query.Where(p => p.Timestamp >= DateTime.UtcNow.AddDays(-numberOfDataPoints * 7));
                        break;
                    case "5m":
                        query = query.Where(p => p.Timestamp >= DateTime.UtcNow.AddMinutes(-numberOfDataPoints * 5));
                        break;
                    default:
                        throw new ArgumentException("Invalid time period specified.");
                }

                var symbolPriceData = await query
                    .OrderByDescending(p => p.Timestamp)
                    .Take(numberOfDataPoints)
                    .Select(p => p.Price)
                    .ToListAsync();

                if (symbolPriceData.Count < numberOfDataPoints)
                {
                    return null; // Not enough data points
                }

                sma = symbolPriceData.Average();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheExpirationMinutes));
                _cache.Set(cacheKey, sma, cacheEntryOptions);
            }

            return sma;
        }
    }
}
