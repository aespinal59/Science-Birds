using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatingSystem : MonoBehaviour
{
    public class LevelData
    {
        public Sprite levelSprite;
        public string levelXML;
        public bool pressedButton;
        public uint preScore;
        public float timeToStable;

        public LevelData(string xml)
        {
            levelXML = xml;
            pressedButton = false;
            levelSprite = null;
            preScore = 0;
            timeToStable = 0f;
        }

    }

    public class LSystemRating
    {
        public LSystem lSystem;
        public bool isSelected;

        public LSystemRating(LSystem lSystem)
        {
            this.lSystem = lSystem;
            this.isSelected = false;
        }
    }

    // determines if levels and screenshots are currently being generated (meaning time should be sped up)
    private static bool isGenerating = false;
    public static bool IsGenerating => isGenerating;

    public static int CurrentLSystemIndex = 0; // -1 means you are on the main Level Select Menu, 0-5 means displaying that corresponding LSystem's levels

    public static List<LSystem> lSystems;
    public static List<bool> isStarred;
    public static List<List<LevelData>> levelData;

    private static readonly float TIME_SCALE_FOR_GENERATION = 10f;
    public static readonly int MAX_LSYSTEMS = 12;
    public static readonly int MAX_LEVELS = 6;


    void Awake()
    {
        lSystems = new List<LSystem>();
        levelData = new List<List<LevelData>>();
        isStarred = new List<bool>();

        for(int i = 0; i < MAX_LSYSTEMS; ++i)
        {
            levelData.Add(new List<LevelData>());
            isStarred.Add(false);
        }
        //Debug.Log(System.DateTime.Now.ToString() + "\tResetting Level Count from Init: " + RatingSystem.levelSprites.Count);
    }

    public static void ClearAll()
    {
        lSystems = new List<LSystem>();
        levelData = new List<List<LevelData>>();
        isStarred = new List<bool>();

        for (int i = 0; i < MAX_LSYSTEMS; ++i)
        {
            levelData.Add(new List<LevelData>());
            isStarred.Add(false);
        }
    }

    public static void GenerateXMLs(int lSystemIndex, int height)
    {
        levelData[lSystemIndex].Clear();
        for (int i = 0; i < MAX_LEVELS; ++i)
        {
            //levelData[lSystemIndex].Add(new LevelData(XML));
            levelData[lSystemIndex].Add(new LevelData(lSystems[lSystemIndex].GenerateXML(height)));
        }
    }

    public static void StartGeneratingScreenshots(int lSystemIndex)
    {
        Debug.Log("Starting generation for " + lSystemIndex);
        AudioListener.volume = 0f;
        //Debug.Log(System.DateTime.Now.ToString() + "\tResetting Level Count from Start: " + RatingSystem.levelSprites.Count);
        isGenerating = true;
        CurrentLSystemIndex = lSystemIndex;
        Time.timeScale = TIME_SCALE_FOR_GENERATION;
    }

    public static void EndGeneratingScreenshots()
    {
        Debug.Log("Ending generation");
        AudioListener.volume = 0.1f;
        isGenerating = false;
        Time.timeScale = 1f;
    }

    public static void AddLevel(int i, Sprite level, uint preScore, float timeToStable)
    {
        levelData[CurrentLSystemIndex][i % MAX_LEVELS].levelSprite = level;
        levelData[CurrentLSystemIndex][i % MAX_LEVELS].preScore = preScore;
        levelData[CurrentLSystemIndex][i % MAX_LEVELS].timeToStable = timeToStable;
    }

    public static void RateLevel(int i, GameObject star)
    {
        Debug.Log("pressing " + (i % MAX_LEVELS));
        levelData[CurrentLSystemIndex][i % MAX_LEVELS].pressedButton = !levelData[CurrentLSystemIndex][i % MAX_LEVELS].pressedButton;
        if (levelData[CurrentLSystemIndex][i % MAX_LEVELS].pressedButton)
        {
            star.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            star.GetComponent<Image>().color = Color.black;
        }
    }

    public static void RateLSystem(int lSystemIndex, GameObject star)
    {
        Debug.Log("pressing " + lSystemIndex);
        isStarred[lSystemIndex] = !isStarred[lSystemIndex];
        if (isStarred[lSystemIndex])
        {
            star.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            star.GetComponent<Image>().color = Color.black;
        }
    }

    public static void SubmitRatings(MonoBehaviour script)
    {
        GetFitnesses();
        Dictionary<int, int> ratings = new Dictionary<int, int>();

        for (int i = 0; i < levelData[CurrentLSystemIndex].Count; ++i)
        {
            if (levelData[CurrentLSystemIndex][i].pressedButton)
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

        // IMPORTANT: Use the new List<bool> isStarred to determine which LSystem's have been selected by the player
        // the index in isStarred corresponds to the index in lSystems

        script.StartCoroutine(SqlConnection.PostRating(null));
    }

    public static List<float> GetFitnesses()
    {
        Debug.Log("------FIT-------------");
        List<float> fitnesses = new List<float>();

        for (int i = 0; i < levelData.Count; ++i)
        {
            float averageVal = 0;
            foreach (LevelData data in levelData[i])
            {
                averageVal += 1f / (data.preScore + 10f * data.timeToStable + 1f);
                Debug.Log("preScore: " + data.preScore + "\ttimeToStable: " + data.timeToStable);
            }
            averageVal /= MAX_LEVELS;
            Debug.Log("fitness: " + averageVal);
            fitnesses.Add(averageVal);
        }
        return fitnesses;
    }

    private static string XML = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<Level width =\"2\">\r\n<Camera x=\"0\" y=\"2\" minWidth=\"20\" maxWidth=\"30\">\r\n<Birds>\r\n<Bird type=\"BirdRed\"/>\r\n<Bird type=\"BirdRed\"/>\r\n<Bird type=\"BirdRed\"/>\r\n<Bird type=\"BirdBlack\"/>\r\n<Bird type=\"BirdBlack\"/>\r\n<Bird type=\"BirdRed\"/>\r\n<Bird type=\"BirdBlack\"/>\r\n</Birds>\r\n<Slingshot x=\"-8\" y=\"-2.5\">\r\n<GameObjects>\r\n<Block type=\"SquareHole\" material=\"wood\" x=\"-2.2456510103\" y=\"-3.08\" rotation=\"0\" />\r\n<Block type=\"SquareHole\" material=\"wood\" x=\"-1.2456510103\" y=\"-3.08\" rotation=\"0\" />\r\n<Block type=\"RectTiny\" material=\"ice\" x=\"-2.2456510103\" y=\"-2.445\" rotation=\"90\" />\r\n<Block type=\"RectTiny\" material=\"stone\" x=\"-1.7456510103\" y=\"-2.445\" rotation=\"90\" />\r\n<Block type=\"RectTiny\" material=\"wood\" x=\"-1.2456510103\" y=\"-2.445\" rotation=\"90\" />\r\n<Block type=\"RectMedium\" material=\"ice\" x=\"-2.2456510103\" y=\"-1.39\" rotation=\"90\" />\r\n<Block type=\"RectMedium\" material=\"stone\" x=\"-1.7456510103\" y=\"-1.39\" rotation=\"90\" />\r\n<Block type=\"RectMedium\" material=\"ice\" x=\"-1.2456510103\" y=\"-1.39\" rotation=\"90\" />\r\n<Block type=\"RectSmall\" material=\"ice\" x=\"-2.2456510103\" y=\"-0.125\" rotation=\"90\" />\r\n<Block type=\"RectSmall\" material=\"ice\" x=\"-1.2456510103\" y=\"-0.125\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"-0.0649028163\" y=\"-3.075\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"0.5550971837\" y=\"-3.075\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"0.9950971837\" y=\"-3.075\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"1.6150971837\" y=\"-3.075\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"ice\" x=\"-0.0649028163\" y=\"-2.225\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"wood\" x=\"0.5550971837\" y=\"-2.225\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"0.9950971837\" y=\"-2.225\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"1.6150971837\" y=\"-2.225\" rotation=\"90\" />\r\n<Block type=\"RectTiny\" material=\"ice\" x=\"-0.0649028163\" y=\"-1.69\" rotation=\"0\" />\r\n<Block type=\"RectTiny\" material=\"ice\" x=\"0.5550971837\" y=\"-1.69\" rotation=\"0\" />\r\n<Block type=\"RectTiny\" material=\"stone\" x=\"0.9950971837\" y=\"-1.69\" rotation=\"0\" />\r\n<Block type=\"RectTiny\" material=\"wood\" x=\"1.6150971837\" y=\"-1.69\" rotation=\"0\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"-0.0649028163\" y=\"-1.155\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"wood\" x=\"0.5550971837\" y=\"-1.155\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"0.9950971837\" y=\"-1.155\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"1.6150971837\" y=\"-1.155\" rotation=\"90\" />\r\n<Block type=\"SquareHole\" material=\"stone\" x=\"0.2450971837\" y=\"-0.31\" rotation=\"0\" />\r\n<Block type=\"SquareHole\" material=\"ice\" x=\"1.3050971837\" y=\"-0.31\" rotation=\"0\" />\r\n<Block type=\"RectTiny\" material=\"stone\" x=\"3.1428886339\" y=\"-3.39\" rotation=\"0\" />\r\n<Block type=\"RectTiny\" material=\"ice\" x=\"3.8728886339\" y=\"-3.39\" rotation=\"0\" />\r\n<Block type=\"RectTiny\" material=\"ice\" x=\"4.6028886339\" y=\"-3.39\" rotation=\"0\" />\r\n<Block type=\"RectFat\" material=\"ice\" x=\"3.1428886339\" y=\"-2.855\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"wood\" x=\"3.8728886339\" y=\"-2.855\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"wood\" x=\"4.6028886339\" y=\"-2.855\" rotation=\"90\" />\r\n<Block type=\"SquareHole\" material=\"wood\" x=\"3.4528886339\" y=\"-2.01\" rotation=\"0\" />\r\n<Block type=\"SquareHole\" material=\"ice\" x=\"4.2928886339\" y=\"-2.01\" rotation=\"0\" />\r\n<Block type=\"RectTiny\" material=\"stone\" x=\"3.4528886339\" y=\"-1.375\" rotation=\"90\" />\r\n<Block type=\"RectTiny\" material=\"stone\" x=\"3.8728886339\" y=\"-1.375\" rotation=\"90\" />\r\n<Block type=\"RectTiny\" material=\"stone\" x=\"4.2928886339\" y=\"-1.375\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"ice\" x=\"3.5578886339\" y=\"-0.735\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"wood\" x=\"4.1878886339\" y=\"-0.735\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"ice\" x=\"3.872888633875582\" y=\"-0.095\" rotation=\"0\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"6.6221404398\" y=\"-3.285\" rotation=\"0\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"8.0821404398\" y=\"-3.285\" rotation=\"0\" />\r\n<Block type=\"SquareSmall\" material=\"ice\" x=\"6.6221404398\" y=\"-2.855\" rotation=\"0\" />\r\n<Block type=\"SquareSmall\" material=\"ice\" x=\"8.0821404398\" y=\"-2.855\" rotation=\"0\" />\r\n<Block type=\"RectMedium\" material=\"stone\" x=\"7.3521404398\" y=\"-2.53\" rotation=\"0\" />\r\n<Block type=\"RectMedium\" material=\"stone\" x=\"7.3521404398\" y=\"-2.31\" rotation=\"0\" />\r\n<Block type=\"SquareTiny\" material=\"ice\" x=\"7.352140439815956\" y=\"-2.09\" rotation=\"0\" />\r\n<Block type=\"RectBig\" material=\"wood\" x=\"7.5036218332\" y=\"1.5567356041\" rotation=\"0\" />\r\n<Block type=\"RectFat\" material=\"ice\" x=\"7.1886218332\" y=\"2.0917356041\" rotation=\"90\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"7.8186218332\" y=\"2.0917356041\" rotation=\"90\" />\r\n<Block type=\"RectSmall\" material=\"stone\" x=\"7.5036218332\" y=\"2.6267356041\" rotation=\"0\" />\r\n<Block type=\"SquareHole\" material=\"ice\" x=\"7.503621833191875\" y=\"3.1567356041\" rotation=\"0\" />\r\n<Block type=\"RectFat\" material=\"stone\" x=\"-0.3364931188\" y=\"2.5654473817\" rotation=\"0\" />\r\n<Block type=\"RectFat\" material=\"wood\" x=\"0.5835068812\" y=\"2.5654473817\" rotation=\"0\" />\r\n<Block type=\"RectFat\" material=\"wood\" x=\"1.5035068812\" y=\"2.5654473817\" rotation=\"0\" />\r\n<Block type=\"RectBig\" material=\"ice\" x=\"0.5835068812\" y=\"2.8904473817\" rotation=\"0\" />\r\n<Block type=\"SquareSmall\" material=\"ice\" x=\"0.5835068812019446\" y=\"3.2154473817\" rotation=\"0\" />\r\n<Block type=\"CircleSmall\" material=\"stone\" x=\"6.7921404398\" y=\"-1.975\" rotation=\"0\" />\r\n<Block type=\"CircleSmall\" material=\"wood\" x=\"1.0250971837\" y=\"0.335\" rotation=\"0\" />\r\n<Block type=\"CircleSmall\" material=\"wood\" x=\"1.5850971837\" y=\"0.335\" rotation=\"0\" />\r\n<Block type=\"Triangle\" material=\"wood\" x=\"-0.1031597855\" y=\"3.4104473817\" rotation=\"90.0\" />\r\n<Block type=\"Circle\" material=\"stone\" x=\"-1.2456510103\" y=\"0.7\" rotation=\"0\" />\r\n<Block type=\"CircleSmall\" material=\"stone\" x=\"4.1562219672\" y=\"0.345\" rotation=\"0\" />\r\n<Pig type=\"BasicSmall\" material=\"\" x=\"-2.2456510103\" y=\"0.55\" rotation=\"0\" />\r\n<Pig type=\"BasicSmall\" material=\"\" x=\"0.5250971837\" y=\"0.36\" rotation=\"0\" />\r\n<Pig type=\"BasicSmall\" material=\"\" x=\"3.5895553005\" y=\"0.37\" rotation=\"0\" />\r\n<Pig type=\"BasicSmall\" material=\"\" x=\"7.3121404398\" y=\"-3.25\" rotation=\"0\" />\r\n<Pig type=\"BasicSmall\" material=\"\" x=\"7.3521404398\" y=\"-1.73\" rotation=\"0\" />\r\n<Pig type=\"BasicSmall\" material=\"\" x=\"7.2236218332\" y=\"3.8267356041\" rotation=\"0\" />\r\n<Pig type=\"BasicSmall\" material=\"\" x=\"7.7836218332\" y=\"3.8267356041\" rotation=\"0\" />\r\n<Pig type=\"BasicSmall\" material=\"\" x=\"0.5835068812\" y=\"3.6804473817\" rotation=\"0\" />\r\n<Pig type=\"BasicSmall\" material=\"\" x=\"1.2701735479\" y=\"3.2504473817\" rotation=\"0\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"6.573621833191876\" y=\"1.1367356040556196\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"7.193621833191876\" y=\"1.1367356040556196\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"7.813621833191875\" y=\"1.1367356040556196\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"8.433621833191875\" y=\"1.1367356040556196\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"-1.2764931187980553\" y=\"2.040447381746927\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"-0.6564931187980554\" y=\"2.040447381746927\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"-0.03649311879805539\" y=\"2.040447381746927\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"0.5835068812019446\" y=\"2.040447381746927\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"1.2035068812019447\" y=\"2.040447381746927\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"1.8235068812019446\" y=\"2.040447381746927\" />\r\n<Platform type=\"Platform\" material=\"\" x=\"2.4435068812019445\" y=\"2.040447381746927\" />\r\n<TNT type=\"\" material=\"\" x=\"-1.7456510103\" y=\"-0.3\" rotation=\"0\" />\r\n<TNT type=\"\" material=\"\" x=\"-0.0349028163\" y=\"0.36\" rotation=\"0\" />\r\n<TNT type=\"\" material=\"\" x=\"7.9121404398\" y=\"-1.95\" rotation=\"0\" />\r\n</GameObjects>\r\n</Level>\r\n";
}
