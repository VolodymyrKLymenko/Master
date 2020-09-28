using GeneticlAlgorithmCalculation.GACalculations.Models;
using System;

namespace GeneticlAlgorithmCalculation.GACalculations
{
    public abstract class GeneticAlgorithmSolverBase
    {
        private readonly int _populationSize;
        private readonly double _mutationRate;
        private readonly double _crossoverRate;
        private readonly int _elitismCount;
        private readonly int _maxCountOfUnchangedGenerations;

        public GeneticAlgorithmSolverBase(
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

        protected void EvaluatePopulation(Population population, Func<Individual, double> calculateFitnes)
        {
            var populationFittest = 0.0;

            foreach (var individual in population.Individuals)
            {
                var fitness = calculateFitnes(individual);
                if (populationFittest < fitness)
                {
                    populationFittest = fitness;
                }
            }

            var avgFitness = populationFittest / population.Size();
            population.UpdateFitness(avgFitness);
        }

        protected bool IsTerminationConditionMet(Population population)
        {
            return population.NotChangedPreviousGenerations > _maxCountOfUnchangedGenerations;
        }

        protected Population CrossoverPopulation(Population population, Random rnd)
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

        protected Population MutatePopulation(Population population, Random rnd)
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
}
