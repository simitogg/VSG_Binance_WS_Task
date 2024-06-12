using Microsoft.AspNetCore.Mvc;
using Commons;
using RequestHandler.BusinessLayer;

namespace RequestHandler.Controllers
{
    [ApiController]
    [Route("api")]
    public class CalculationsController : ControllerBase
    {
        private readonly ILogger<CalculationsController> _logger;
        private readonly IBL_Calculations _BL_Calculations;

        public CalculationsController(IBL_Calculations BL_Calculations, ILogger<CalculationsController> logger)
        {
            _logger = logger;
            _BL_Calculations = BL_Calculations;
        }

        [HttpGet]
        [Route("{symbol}/24hAvgPrice")]
        public async Task<ActionResult<IEnumerable<SymbolRecord>>> GetLatestPrices(string symbol)
        {
            var averagePrice = await _BL_Calculations.Get24hAvgPriceAsync(symbol);

            if (averagePrice == null)
            {
                return NotFound();
            }

            var response = new
            {
                symbol_name = symbol,
                averagePrice = averagePrice.Value,
                timePeriod = "24h"
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("{symbol}/SimpleMovingAverage")]
        public async Task<ActionResult<decimal>> GetAveragePrice(string symbol, int n, string p, DateTime? s = null)
        {
            var sma = await _BL_Calculations.GetSimpleMovingAverageAsync(symbol, n, p, s);

            if (sma == null)
            {
                return NotFound();
            }

            var response = new
            {
                symbol_name = symbol,
                simpleMovingAverage = sma.Value,
                numberOfDataPoints = n,
                timePeriod = p,
                startDateTime = s
            };

            return Ok(response);
        }
    }
}
