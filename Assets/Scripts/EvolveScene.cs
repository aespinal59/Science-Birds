using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveScene : MonoBehaviour
{

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

        // if you're done, then return to levelselectmenu MAKE SURE YOU HAVE 12 LSYSTEMS IN RATINGSYSTEM.LSYSTEM
        // RatingSystem.keptForEvolution.Clear(); // maybe do this?
        ABSceneManager.Instance.LoadScene("LevelSelectMenu");
    }
}
