namespace RequestHandler.BusinessLayer
{
    public interface IBL_Calculations
    {
        Task<decimal?> Get24hAvgPriceAsync(string symbol);
        Task<decimal?> GetSimpleMovingAverageAsync(string symbol, int numberOfDataPoints, string timePeriod, DateTime? startDateTime = null);
    }
}
