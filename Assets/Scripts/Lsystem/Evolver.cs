using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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





    static Random rand = new Random();
    public static Dictionary<MapKey, LSystem> solutions = new Dictionary<MapKey, LSystem>();
    public static Dictionary<MapKey, double> perfomances = new Dictionary<MapKey, double>();

    //  Evolves population using map elites
    public List<LSystem> EvolvePopulationMAPElitesEdition(List<RatingSystem.LSystemEvolution> ratedLSystems)
    {
        LSystem lSystem = null;
        double performance = 0;
        //  Sett the fitness values for the population and add to solutions and performances
        foreach (RatingSystem.LSystemEvolution ratedLSystem in ratedLSystems)
        {
            lSystem = ratedLSystem.lSystem;
            performance = ratedLSystem.fitness;
            performance = lSystem.fitness; //Get the fitness for this thing
            MapKey key = KeyExtracter(ratedLSystem);

            if (!solutions.ContainsKey(key) ||
                (solutions.ContainsKey(key) && perfomances[key] < performance))
            {
                solutions[key] = lSystem;
                perfomances[key] = performance;
            }
        }

        //Get LSystems, order by fitness desc, and only take max. There may be more.
        List<LSystem> newPopulation =
        solutions.Select(x => x.Value).OrderByDescending(y => y.fitness).Select(z => Mutation(z)).Take(RatingSystem.MAX_LSYSTEMS).ToList();

        while (newPopulation.Count < RatingSystem.MAX_LSYSTEMS)
        {
            newPopulation.Add(Mutation(solutions.Select(x => x.Value).ToArray()[rand.Next(solutions.Count)]));
        }
        return newPopulation;

    }

    private MapKey KeyExtracter(RatingSystem.LSystemEvolution ratedLSystem)
    {
        MapKey key = new MapKey();

        //Retreive average values from lsystem and insert into key
        //TODO: this is an example, figure out what features you want and how to get avg of levels generate for values
        //key.FeatureSpace["Pig"] = (int)Math.Round(ratedLSystem.xmls.Select(x => CountOccurencesOf(x, "Pig")).Average());
        //key.FeatureSpace["TNT"] = (int)Math.Round(ratedLSystem.xmls.Select(x => CountOccurencesOf(x, "TNT")).Average());
        foreach (string value in LSystem.block_names.Values.Distinct().Union(new string[]{ "wood", "ice", "stone" }))
        {
            key.FeatureSpace[value] = (int)Math.Round(ratedLSystem.xmls.Select(x => CountOccurencesOf(x, value)).Average());
        }

        return key;
    }

    private int CountOccurencesOf(string text, string pattern)
    {
        int count = 0;
        foreach (Match match in Regex.Matches(text, pattern))
        {
            count++;
        }
        return count;
    }

    public class MapKey
    {
        private Dictionary<string, int> _FeatureSpace;
        public Dictionary<string, int> FeatureSpace
        {
            get
            {
                if (_FeatureSpace == null)
                    _FeatureSpace = new Dictionary<string, int>();
                return _FeatureSpace;
            }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            MapKey other = obj as MapKey;
            if (other == null)
                return false;

            var keys = FeatureSpace.Keys.Union(other.FeatureSpace.Keys);
            foreach (string key in keys)
            {
                if (!FeatureSpace.ContainsKey(key) || !other.FeatureSpace.ContainsKey(key))
                    return false;

                if (FeatureSpace[key] != other.FeatureSpace[key])
                    return false;
            }
            return true;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int hashCode = 0;

            foreach (var pair in FeatureSpace)
            {
                hashCode = pair.Key.GetHashCode() + pair.Value.GetHashCode();
            }
            return hashCode;
        }
    }
}