using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatingSystem : MonoBehaviour
{
    // determines if levels and screenshots are currently being generated (meaning time should be sped up)
    private static bool isGenerating = false;
    public static bool IsGenerating => isGenerating;

    public static List<Sprite> levelSprites;
    public static List<bool> pressedButtons;

    private static readonly float TIME_SCALE_FOR_GENERATION = 10f;

    void Awake()
    {
        levelSprites = new List<Sprite>();
        pressedButtons = new List<bool>();
        //Debug.Log(System.DateTime.Now.ToString() + "\tResetting Level Count from Init: " + RatingSystem.levelSprites.Count);
    }

    public static void StartGenerating()
    {
        AudioListener.volume = 0f;
        //Debug.Log(System.DateTime.Now.ToString() + "\tResetting Level Count from Start: " + RatingSystem.levelSprites.Count);
        isGenerating = true;
        levelSprites = new List<Sprite>();
        pressedButtons = new List<bool>();
        Time.timeScale = TIME_SCALE_FOR_GENERATION;
    }

    public static void EndGenerating()
    {
        AudioListener.volume = 0.1f;
        isGenerating = false;
        Time.timeScale = 1f;
    }

    public static void AddLevel(Sprite level)
    {
        levelSprites.Add(level);
        pressedButtons.Add(false);
    }

    public static void RateLevel(int i, GameObject star)
    {
        Debug.Log("pressing " + i);
        pressedButtons[i] = !pressedButtons[i];
        if (pressedButtons[i])
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

        for (int i = 0; i < pressedButtons.Count; ++i)
        {
            if (pressedButtons[i])
            {
                int lSystem = LevelList.Instance.GetLevel(i).lSystem;
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
