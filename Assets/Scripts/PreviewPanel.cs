using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewPanel : MonoBehaviour
{
    public List<Image> previews;
    public List<Image> lSystemButtons;
    public Image marker;

    void Start()
    {
        if (RatingSystem.CurrentLSystemIndex < RatingSystem.levelData.Count)
            UpdatePreviews(RatingSystem.CurrentLSystemIndex);
    }

    public void UpdatePreviews(int lSystemIndex)
    {
        for (int i = 0; i < RatingSystem.levelData[lSystemIndex].Count; ++i)
        {
            previews[i].sprite = 
                RatingSystem.levelData[lSystemIndex][i].levelSprite;

            ABLevelSelector sel = previews[i].gameObject.GetComponent<ABLevelSelector>();
            sel.LevelIndex = lSystemIndex * RatingSystem.MAX_LEVELS + i;
        }
        marker.transform.position = new Vector2(lSystemButtons[lSystemIndex].transform.position.x + 40, lSystemButtons[lSystemIndex].transform.position.y);
    }
}
