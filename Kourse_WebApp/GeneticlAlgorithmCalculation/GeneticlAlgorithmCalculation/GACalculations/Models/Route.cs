namespace GeneticlAlgorithmCalculation.GACalculations.Models
{
    public class Route
    {
        public Place[] route { get; private set; }
        public double distance { get; private set; }
        public double DurationInMilliseconds { get; set; }
        public int GenerationsCount { get; set; }

        public Route(Individual individual, Place[] places)
        {
            // get individual's chromosome
            var chromosome = individual.Chromosome;

            // create route
            route = new Place[places.Length];
            for (int genIndex = 0; genIndex < chromosome.Length; genIndex++)
            {
                route[genIndex] = places[chromosome[genIndex]];
            }
        }

        public double GetDistance()
        {
            if (distance > 0)
            {
                return distance;
            }

            var totalDistance = 0.0;
            for (int cityIndex = 0; cityIndex < route.Length - 1; cityIndex++)
            {
                totalDistance += route[cityIndex].GetDistance(route[cityIndex + 1]);
            }

            totalDistance += route[route.Length - 1].GetDistance(route[0]);

            distance = totalDistance;

            return distance;
        }
    }
}
