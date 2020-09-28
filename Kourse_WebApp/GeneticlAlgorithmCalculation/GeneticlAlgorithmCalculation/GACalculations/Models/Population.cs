using System;
using System.Linq;

namespace GeneticlAlgorithmCalculation.GACalculations.Models
{
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
