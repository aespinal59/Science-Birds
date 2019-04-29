using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatingSystem : MonoBehaviour
{
    // determines if levels and screenshots are currently being generated (meaning time should be sped up)
    private static bool isGenerating = false;
    public static bool IsGenerating => isGenerating;

    public static int CurrentLSystemIndex = -1; // -1 means you are on the main Level Select Menu, 0-5 means displaying that corresponding LSystem's levels

    public static List<LSystem> lSystems;
    public static List<List<Sprite>> levelSprites;
    public static List<List<bool>> pressedButtons;

    private static readonly float TIME_SCALE_FOR_GENERATION = 10f;
    public static readonly int MAX_LEVELS = 6;

    void Awake()
    {
        lSystems = new List<LSystem>();
        levelSprites = new List<List<Sprite>>();
        pressedButtons = new List<List<bool>>();
        for(int i = 0; i < MAX_LEVELS; ++i)
        {
            levelSprites.Add(new List<Sprite>());
            pressedButtons.Add(new List<bool>());
        }
        //Debug.Log(System.DateTime.Now.ToString() + "\tResetting Level Count from Init: " + RatingSystem.levelSprites.Count);
    }

    public static void StartGeneratingScreenshots(int lSystemIndex)
    {
        AudioListener.volume = 0f;
        //Debug.Log(System.DateTime.Now.ToString() + "\tResetting Level Count from Start: " + RatingSystem.levelSprites.Count);
        isGenerating = true;
        CurrentLSystemIndex = lSystemIndex;
        levelSprites[lSystemIndex].Clear();
        pressedButtons[lSystemIndex].Clear();
        Time.timeScale = TIME_SCALE_FOR_GENERATION;
    }

    public static void EndGeneratingScreenshots()
    {
        AudioListener.volume = 0.1f;
        isGenerating = false;
        Time.timeScale = 1f;
    }

    public static void AddLevel(Sprite level)
    {
        levelSprites[CurrentLSystemIndex].Add(level);
        pressedButtons[CurrentLSystemIndex].Add(false);
    }

    public static void RateLevel(int i, GameObject star)
    {
        Debug.Log("pressing " + (i % MAX_LEVELS));
        pressedButtons[CurrentLSystemIndex][i % MAX_LEVELS] = !pressedButtons[CurrentLSystemIndex][i % MAX_LEVELS];
        if (pressedButtons[CurrentLSystemIndex][i % MAX_LEVELS])
        {
            star.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            star.GetComponent<Image>().color = Color.black;
        }
    }

    public static void SubmitRatings()
    {
        Dictionary<int, int> ratings = new Dictionary<int, int>();

        for (int i = 0; i < pressedButtons[CurrentLSystemIndex].Count; ++i)
        {
            if (pressedButtons[CurrentLSystemIndex][i])
            {
                int lSystem = LevelList.Instance.GetLevel(CurrentLSystemIndex * MAX_LEVELS + i).lSystem;
                if (ratings.ContainsKey(lSystem))
                    ratings[lSystem] += 1;
                else
                    ratings[lSystem] = 1;
            }
        }

        foreach (int i in ratings.Keys)
        {
            Debug.Log("LSystem: " + i + "\tRatings: " + ratings[i]);

            /* TODO:
             * Submit ratings by adding the rating value to the corresponding LSystem_id in the database
             * You will have to find a way to get the Level seed, but it might be obtainable the same way I
             * got the lSystem on line 67, as long as you add a value in the ABLevel that represents the seed
             */ 
        }
    }
}
