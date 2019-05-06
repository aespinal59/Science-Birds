using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LSystemButton : MonoBehaviour
{
    public int lSystemIndex;
    public ABLevelSelect levelSelect;
    public PreviewPanel panel;
    public GameObject star;

    void Start()
    {
        if (RatingSystem.isStarred[lSystemIndex])
        {
            star.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            star.GetComponent<Image>().color = Color.black;
        }
    }

    public void DisplayPreviews()
    {
        RatingSystem.CurrentLSystemIndex = lSystemIndex;
        panel.UpdatePreviews(lSystemIndex);
        Debug.Log("Inside Button " + lSystemIndex);
    }

    public void GenerateNewLevels()
    {
        RatingSystem.GenerateXMLs(lSystemIndex, 5);
        levelSelect.LoadScreenshots(lSystemIndex);
    }

    public void Star()
    {
        RatingSystem.RateLSystem(lSystemIndex, star);
    }
}
