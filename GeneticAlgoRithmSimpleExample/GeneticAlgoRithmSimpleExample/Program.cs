using System;

namespace GeneticAlgoRithmSimpleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var rnd = new Random();
            rnd.Next();
            rnd.Next();

            // Create GA object
            var ga = new GeneticAlgorithm(100, 0.01, 0.95, 2);

            // Initialize population
            var population = ga.InitPopulation(50);
            var generation = 1;

            while (!ga.IsTerminationConditionMet(population))
            {
                Console.WriteLine($"Best solution {population.GetFittest(0)}");

                // Crossover
                population = ga.CrossoverPopulation(population, rnd);

                // Mutation
                population = ga.MutatePopulation(population, rnd);

                // Evaluate population
                ga.EvaluatePopulation(population);

                // Increment generation
                generation++;
            }

            Console.WriteLine($"The solution was founded in {generation} generation");
            Console.WriteLine($"Best solution is {population.GetFittest(0)}");
        }
    }

    public class GeneticAlgorithm
    {
        private readonly int _populationSize;
        private readonly double _mutationRate;
        private readonly double _crossoverRate;
        private readonly int _elitismCount;

        public GeneticAlgorithm(
            int populationSize,
            double mutationRate,
            double crossoverRate,
            int elitismCount)
        {
            _populationSize = populationSize;
            _mutationRate = mutationRate;
            _crossoverRate = crossoverRate;
            _elitismCount = elitismCount;
        }

        public Population InitPopulation(int chromosomeLength)
        {
            return new Population(_populationSize, chromosomeLength);
        }

        public double CalculateFitness(Individual individual)
        {
            int correctGenes = 0;

            for (int i = 0; i < individual.ChromosomeLength; i++)
            {
                if (individual.GetGene(i) == 1)
                    correctGenes++;
            }

            var fitness = (double)correctGenes / individual.ChromosomeLength;

            individual.Fitness = fitness;

            return fitness;
        }

        public void EvaluatePopulation(Population population)
        {
            var populationFitness = 0.0;

            foreach (var individual in population.Individuals)
            {
                populationFitness += CalculateFitness(individual);
            }

            population.Fitness = populationFitness;
        }

        public bool IsTerminationConditionMet(Population population)
        {
            foreach (var individual in population.Individuals)
            {
                if (individual.Fitness == 1) return true;
            }

            return false;
        }

        public Population CrossoverPopulation(Population population, Random rnd)
        {
            var newPopulation = new Population(population.Size());

            for (int i = 0; i < population.Size(); i++)
            {
                var firstParent = population.GetFittest(i);

                if (_crossoverRate > rnd.NextDouble() && i > _elitismCount)
                {
                    // initialize offspring
                    var offspring = new Individual(firstParent.ChromosomeLength);

                    // select second parent
                    var secondParent = SelectParent(population, rnd);

                    // Loop over genome
                    for (int genIndex = 0; genIndex < firstParent.ChromosomeLength; genIndex++)
                    {
                        if (0.5 > rnd.NextDouble())
                        {
                            offspring.SetGene(genIndex, firstParent.GetGene(genIndex));
                        }
                        else
                        {
                            offspring.SetGene(genIndex, secondParent.GetGene(genIndex));
                        }
                    }

                    // Add offspring to new population
                    newPopulation.SetIndividual(i, offspring);
                }
                else
                {
                    // Add individual to new population without applying crossover
                    newPopulation.SetIndividual(i, firstParent);
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
            var newPopulation = new Population(_populationSize);

            for (int populationIndex = 0; populationIndex < population.Size(); populationIndex++)
            {
                var individual = population.GetFittest(populationIndex);

                for (int genIndex = 0; genIndex < individual.ChromosomeLength; genIndex++)
                {
                    if (populationIndex >= _elitismCount)
                    {
                        if (_mutationRate > rnd.NextDouble())
                        {
                            var newGene = individual.GetGene(genIndex) == 1 ? 0 : 1;

                            individual.SetGene(genIndex, newGene);
                        }
                    }
                }

                newPopulation.SetIndividual(populationIndex, individual);
            }

            return newPopulation;
        }
    }

    public class Individual
    {
        public double Fitness { set; get; } = -1;
        public int[] Chromosome { private set; get; }
        public int ChromosomeLength { get { return Chromosome.Length; } }

        public Individual(int[] chromosome)
        {
            Chromosome = chromosome;
        }

        public Individual(int chromosomeLength)
        {
            Chromosome = new int[chromosomeLength];
            var rnd = new Random();
            for (int i = 0; i < chromosomeLength; i++)
            {
                Chromosome[i] = rnd.Next(0, 2);
            }
        }

        public void SetGene(int offset, int gene)
        {
            Chromosome[offset] = gene;
        }

        public int GetGene(int offset)
        {
            return Chromosome[offset];
        }

        public override string ToString()
        {
            var output = "";
            foreach (var gene in Chromosome)
            {
                output += gene.ToString();
            }

            return output;
        }
    }

    public class Population
    {
        public Individual[] Individuals { private set; get; }
        public double Fitness { set; get; } = -1;

        public Population(int populationSize)
        {
            Individuals = new Individual[populationSize];
        }

        public Population(int populationSize, int chromosomeLength)
        {
            Individuals = new Individual[populationSize];

            for (int i = 0; i < populationSize; i++)
            {
                Individuals[i] = new Individual(chromosomeLength);
            }
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

        public void Shuffle()
        {
            Random rnd = new Random();
            for (int i = Individuals.Length - 1; i > 0; i--)
            {
                var index = rnd.Next(i + 1);
                Individual a = Individuals[index];
                Individuals[index] = Individuals[i];
                Individuals[i] = a;
            }
        }
    }
}
