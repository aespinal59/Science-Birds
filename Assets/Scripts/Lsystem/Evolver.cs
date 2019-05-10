using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class LSystemEvolver
{
    //  Member variables
    private int maxWidth;
    private int maxHeight;
    private int numRules;
    private double mutationRate;

    //Constructor
    public LSystemEvolver(int nrules, int width, int height, double mutRate)
    {
        //  Set hyperparameters
        numRules = nrules;
        maxWidth = width;
        maxHeight = height;
        mutationRate = mutRate;

        

    }

    public static List<LSystem> GenerateRandomPopulation(int populationSize, int numRule, int maxW) {
        //  Initialize population
        List<LSystem> population = new List<LSystem>();
        for (int i = 0; i < populationSize; i++)
        {
            //  Create random LSystem and add it to population
            population.Add(new LSystem(numRule, maxW));
        }

        return population;
    }

    //Methods
    private LSystem Crossover(LSystem lparent, LSystem rparent)
    {
        return LSystem.Crossover(lparent, rparent);
    }

    private LSystem Mutation(LSystem l)
    {
        return LSystem.Mutation(l);
    }

    //  Evolves population using fitness values and mu + lambda genetic algorithm.
    public List<LSystem> EvolvePopulation(List<LSystem> population, List<float> fitness, int mu, int lambda)
    {
        //  Check if length of fitness array matches population size
        if (fitness.Count != population.Count)
        {
            Console.WriteLine("Error: Invalid fitness size.");
            return population;
        }

        //  Check if mu + lambda equals population size.
        if (mu + lambda != population.Count)
        {
            Console.WriteLine("Error: mu + lambda does not equal populationSize.");
            return population;
        }

        //  Sett the fitness values for the population
        for (int popIndex = 0; popIndex < population.Count; popIndex++)
        {
            population[popIndex].SetFitness(fitness[popIndex]);
        }

        //  Sort the population in descending order by fitness value
        List<LSystem> orderedPopulation = population.OrderByDescending(l => l.fitness).ToList<LSystem>();

        //  Perform mu + lambda genetic algorithm. 
        //  Mu is the number of parents, lambda is the number of offspring.
        List<LSystem> newPopulation = orderedPopulation;

        //  Delete least fit lambda individuals
        //  Replace removed individuals with copies of mu best individuals
        //  Mutate the offspring
        for (int popIndex = mu; popIndex < lambda; popIndex++)
        {
            newPopulation[popIndex] = Mutation(newPopulation[popIndex % mu]);
        }

        return newPopulation;

    }




}