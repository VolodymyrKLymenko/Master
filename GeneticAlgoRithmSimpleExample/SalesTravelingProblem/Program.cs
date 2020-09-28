using System;
using System.Linq;

namespace SalesTravelingProblem
{
    class Program
    {
        private static int maxNotChangedGenerations = 700;

        static void Main(string[] args)
        {
            var numCities = 100;
            var cities = new City[numCities];
            var rnd = new Random();

            for (int i = 0; i < numCities; i++)
            {
                var x = (int)(100 * rnd.NextDouble());
                var y = (int)(100 * rnd.NextDouble());

                cities[i] = new City(x, y);
            }

            // Initializa GA
            var ga = new GeneticAlgorithm(100, 0.001, 0.9, 2, maxNotChangedGenerations/*, 5*/);

            var population = ga.InitPopulation(cities.Length);

            // Evaluate population
            ga.EvaluatePopulation(population, cities);

            // Keep track of current population
            var generation = 1;

            // Start evaluation loop
            while (!ga.IsTerminationConditionMet(population))
            {
                // Print fittest individual
                var route = new Route(population.GetFittest(0), cities);

                Console.WriteLine($"-> G{generation} Best distance: {route.GetDistance()}");

                // Apply crossover
                population = ga.CrossoverPopulation(population, rnd);

                // TODO: Apply mutation
                population = ga.MutatePopulation(population, rnd);

                // Evaluate population
                ga.EvaluatePopulation(population, cities);

                // Increment current generation
                generation++;
            }

            // Display results
            Console.WriteLine($"Stopped after {generation} generations.");

            var finalRoute = new Route(population.GetFittest(0), cities);
            Console.WriteLine($"Best distance is {finalRoute.GetDistance()}");
        }
    }

    public class City
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public City(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double GetDistance(City toCity)
        {
            var deltaX = Math.Pow(X - toCity.X, 2);
            var deltaY = Math.Pow(Y - toCity.Y, 2);

            return Math.Sqrt(Math.Abs(deltaX + deltaY));
        }
    }

    public class Route
    {
        public City[] route { get; private set; }
        public double distance { get; private set; }

        public Route(Individual individual, City[] cities)
        {
            // get individual's chromosome
            var chromosome = individual.Chromosome;

            // create route
            route = new City[cities.Length];
            for (int genIndex = 0; genIndex < chromosome.Length; genIndex++)
            {
                route[genIndex] = cities[chromosome[genIndex]];
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

    public class GeneticAlgorithm
    {
        private readonly int _populationSize;
        private readonly double _mutationRate;
        private readonly double _crossoverRate;
        private readonly int _elitismCount;
        private readonly int _maxCountOfUnchangedGenerations;

        public GeneticAlgorithm(
            int populationSize,
            double mutationRate,
            double crossoverRate,
            int elitismCount,
            int maxCountOfUnchangedGenerations)
        {
            _populationSize = populationSize;
            _mutationRate = mutationRate;
            _crossoverRate = crossoverRate;
            _elitismCount = elitismCount;
            _maxCountOfUnchangedGenerations = maxCountOfUnchangedGenerations;
        }

        public Population InitPopulation(int chromosomeLength)
        {
            return new Population(_populationSize, chromosomeLength);
        }

        public double CalculateFitness(Individual individual, City[] cities)
        {
            var route = new Route(individual, cities);

            var fitness = 1.0 / route.GetDistance();

            individual.Fitness = fitness;

            return fitness;
        }

        public void EvaluatePopulation(Population population, City[] cities)
        {
            var populationFittest = 0.0;

            foreach (var individual in population.Individuals)
            {
                var fitness = CalculateFitness(individual, cities);
                if (populationFittest < fitness)
                {
                    populationFittest = fitness;
                }
            }

            var avgFitness = populationFittest / population.Size();
            population.UpdateFitness(avgFitness);
        }

        public bool IsTerminationConditionMet(Population population)
        {
            return population.NotChangedPreviousGenerations > _maxCountOfUnchangedGenerations;
        }

        public Population CrossoverPopulation(Population population, Random rnd)
        {
            var newPopulation = new Population(population);

            for (int populationIndex = 0; populationIndex < population.Size(); populationIndex++)
            {
                var firstParent = population.GetFittest(populationIndex);

                if (_crossoverRate > rnd.NextDouble() && populationIndex > _elitismCount)
                {
                    // select second parent
                    var secondParent = SelectParent(population, rnd);

                    var offspringChromosomes = new int[firstParent.ChromosomeLength];
                    Array.Fill(offspringChromosomes, -1);

                    var offspring = new Individual(offspringChromosomes);
                    var substrPos1 = (int)(rnd.NextDouble() * firstParent.ChromosomeLength);
                    var substrPos2 = (int)(rnd.NextDouble() * firstParent.ChromosomeLength);

                    var startPos = Math.Min(substrPos1, substrPos2);
                    var endPos = Math.Max(substrPos1, substrPos2);

                    for (int i = startPos; i < endPos; i++)
                    {
                        offspring.SetGene(i, firstParent.GetGene(i));
                    }

                    for (int i = 0; i < secondParent.ChromosomeLength; i++)
                    {
                        var parent2Gene = i + endPos;
                        if (parent2Gene >= secondParent.ChromosomeLength)
                        {
                            parent2Gene -= secondParent.ChromosomeLength;
                        }

                        if (!offspring.ContainsGene(secondParent.GetGene(parent2Gene)))
                        {
                            for (int ii = 0; ii < offspring.ChromosomeLength; ii++)
                            {
                                if (offspring.GetGene(ii) == -1)
                                {
                                    offspring.SetGene(ii, secondParent.GetGene(parent2Gene));
                                    break;
                                }
                            }
                        }
                    }

                    // Add offspring to new population
                    newPopulation.SetIndividual(populationIndex, offspring);
                }
                else
                {
                    // Add individual to new population without applying crossover
                    newPopulation.SetIndividual(populationIndex, firstParent);
                }
            }

            return newPopulation;
        }

        private Individual SelectParent(Population population, Random rnd)
        {
            var individuals = population.Individuals;

            var populationFitness = population.Fitness;
            var rouletteWheelPosition = rnd.NextDouble() * populationFitness;

            var spinWheel = 0.0;
            foreach (var individual in individuals)
            {
                spinWheel += individual.Fitness;
                if (spinWheel >= rouletteWheelPosition)
                {
                    return individual;
                }
            }

            return individuals[population.Size() - 1];
        }

        public Population MutatePopulation(Population population, Random rnd)
        {
            var newPopulation = new Population(population);

            for (int populationIndex = 0; populationIndex < population.Size(); populationIndex++)
            {
                var individual = population.GetFittest(populationIndex);

                for (int genIndex = 0; genIndex < individual.ChromosomeLength; genIndex++)
                {
                    if (populationIndex >= _elitismCount)
                    {
                        if (_mutationRate > rnd.NextDouble())
                        {
                            var newGenePos = (int)(rnd.NextDouble() * individual.ChromosomeLength);
                            var gen1 = individual.GetGene(newGenePos);
                            var gen2 = individual.GetGene(genIndex);

                            individual.SetGene(newGenePos, gen2);
                            individual.SetGene(genIndex, gen1);
                        }
                    }
                }

                newPopulation.SetIndividual(populationIndex, individual);
            }

            return newPopulation;
        }
    }

    public class Population
    {
        public Individual[] Individuals { get; private set; }
        public double Fitness { get; private set; } = -1;
        public int NotChangedPreviousGenerations { get; private set; } = 0;

        public Population(int populationSize)
        {
            Individuals = new Individual[populationSize];
        }

        public Population(Population population)
        {
            Individuals = new Individual[population.Size()];
            NotChangedPreviousGenerations = population.NotChangedPreviousGenerations;
            Fitness = population.Fitness;
        }

        public Population(int populationSize, int chromosomeLength)
        {
            Individuals = new Individual[populationSize];

            for (int i = 0; i < populationSize; i++)
            {
                Individuals[i] = new Individual(chromosomeLength);
            }
        }

        public void UpdateFitness(double newFitness)
        {
            if (newFitness == Fitness)
            {
                NotChangedPreviousGenerations++;
            }
            else
            {
                NotChangedPreviousGenerations = 0;
            }

            Fitness = newFitness;
        }

        public Individual GetFittest(int offset)
        {
            Array.Sort(Individuals,
                (o1, o2) =>
                {
                    if (o1.Fitness > o2.Fitness) return -1;
                    else if (o1.Fitness < o2.Fitness) return 1;

                    return 0;
                });

            return Individuals[offset];
        }

        public int Size()
        {
            return Individuals.Length;
        }

        public Individual SetIndividual(int offset, Individual individual)
        {
            return Individuals[offset] = individual;
        }

        public Individual GetIndividual(int offset)
        {
            return Individuals[offset];
        }
    }

    public class Individual
    {
        public double Fitness { set; get; } = -1;
        public int[] Chromosome { private set; get; }
        public int ChromosomeLength { get { return Chromosome.Length; } }

        public Individual(int chromosomeLength)
        {
            Chromosome = new int[chromosomeLength];

            for (int i = 0; i < chromosomeLength; i++)
            {
                Chromosome[i] = i;
            }
        }

        public Individual(int[] chromosomes)
        {
            Chromosome = chromosomes;
        }

        public void SetGene(int offset, int gene)
        {
            Chromosome[offset] = gene;
        }

        public int GetGene(int offset)
        {
            return Chromosome[offset];
        }

        public bool ContainsGene(int gene)
        {
            return Chromosome.Contains(gene);
        }
    }
}
