using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewImage : MonoBehaviour
{
    public void StartLevel()
    {
        ABSceneManager.Instance.LoadScene("GameWorld", true, gameObject.GetComponent<ABLevelSelector>().UpdateLevelList);
    }
}
