using GeneticlAlgorithmCalculation.GACalculations;
using GeneticlAlgorithmCalculation.GACalculations.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GeneticlAlgorithmCalculation.Controllers
{
    [ApiController]
    public class GeneticAlgorithmCalculationController : ControllerBase
    {
        private const int MaxNotChangedGenerations = 700;
        private const int PopulationSize = 100;
        private const double MutationRate = 0.001;
        private const double CrossoverRate = 0.9;
        private const int ElitismCount = 2;

        private readonly ILogger<GeneticAlgorithmCalculationController> _logger;

        public GeneticAlgorithmCalculationController(
            ILogger<GeneticAlgorithmCalculationController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpPost("calculations/bestroute")]
        public ActionResult<Route> Calculate(TravellingSalesmanProblemRequest request)
        {
            var gaCalculator =
                new TravelingSalesmanProblemCalculation(
                    MaxNotChangedGenerations,
                    PopulationSize,
                    MutationRate,
                    CrossoverRate,
                    ElitismCount);

            return gaCalculator.CalculateBestRoute(request.Places);
        }

        public class TravellingSalesmanProblemRequest
        {
            public Place[] Places { get; set; }
        }
    }
}
