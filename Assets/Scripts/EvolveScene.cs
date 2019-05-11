using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveScene : MonoBehaviour
{

    //  Evolution parameters - constant for now.
    public static readonly int NUM_RULES = 10;
    public static readonly int MAX_WIDTH = 3;
    public static readonly int MAX_HEIGHT = 5;
    public static readonly double MUT_RATE = 0.5;
    public static readonly int MU = 5;
    public static readonly int LAMBDA = 7;

    public void LoadXMLs(List<string> xmls)
    {
        string[] xmlArr = new string[xmls.Count];

        for (int i = 0; i < xmls.Count; ++i)
        {
            xmlArr[i] = xmls[i];
        }

        LevelList.Instance.LoadLevelsFromSource(xmlArr);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < RatingSystem.keptForEvolution.Count; ++i)
        {
            if (RatingSystem.keptForEvolution[i].xmls.Count <= 0)
            {
                RatingSystem.GenerateXMLsForEvolution(i, 5);
                LoadXMLs(RatingSystem.keptForEvolution[i].xmls);
                RatingSystem.StartGetFitnessForEvolution(i);
                ABLevelSelector sel = gameObject.AddComponent<ABLevelSelector>();
                sel.LevelIndex = 0;
                ABSceneManager.Instance.LoadScene("GameWorld", true, sel.UpdateLevelList);
                return;
            }
        }
        RatingSystem.EndGetFitnessForEvolution();

        // TODO: Right here is when you have all the fitnesses for each lsystem in the RatingSystem.keptForEvolution
        // You can do more evolution here or pick the best 12 lSystems to put into RatingSystem.lSystems (for now just pick 12 lsystems, I'll make it possible to do more iterations tomorrow or something)
        List<LSystem> pop = new List<LSystem>();
        List<float> fit = new List<float>();

        //  Getting lsystem and fitness
        foreach (RatingSystem.LSystemEvolution l in RatingSystem.keptForEvolution) {
            pop.Add(l.lSystem);
            fit.Add(l.fitness);
        }

        //  Initialize LSystem evolver.
        LSystemEvolver evolver = new LSystemEvolver(NUM_RULES, MAX_WIDTH, MAX_HEIGHT, MUT_RATE);

        //  Evolve population and store.
        RatingSystem.lSystems = evolver.EvolvePopulation(pop, fit, MU, LAMBDA);
        Debug.Log(RatingSystem.lSystems.Count);


        // if you're done, then return to levelselectmenu MAKE SURE YOU HAVE 12 LSYSTEMS IN RATINGSYSTEM.LSYSTEM
        // RatingSystem.keptForEvolution.Clear(); // maybe do this?
        ABSceneManager.Instance.LoadScene("LevelSelectMenu");
    }
}
