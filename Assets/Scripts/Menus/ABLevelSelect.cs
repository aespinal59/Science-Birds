// SCIENCE BIRDS: A clone version of the Angry Birds game used for
// research purposes
//
// Copyright (C) 2016 - Lucas N. Ferreira - lucasnfe@gmail.com
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>
//

﻿using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ABLevelSelect : ABMenu {

	public int _lines = 5;

	public GameObject _levelSelector;
    public GameObject _ratingStar;
	public GameObject _canvas;

	public Vector2 _startPos;
	public Vector2 _buttonSize;

	private int _clickedButton;

    public Camera _camera;

    public GameObject lSystemButtons;
    public GameObject levelButtons;
    // private List<GameObject> tempLevelButtons;


    public void InitializeLSystems(LSystemWrapper[] retrievedLSystems)
    {
        for (int i = 0; i < RatingSystem.MAX_LSYSTEMS; i++)
        {
            RatingSystem.lSystems.Add(LSystem.Decode(retrievedLSystems[i].GetString()));
            RatingSystem.GenerateXMLs(i, 5);
        }
    }

    public void LoadScreenshots(int lSystemIndex)
    {
        RatingSystem.StartGeneratingScreenshots(lSystemIndex);
        ABLevelSelector sel = gameObject.AddComponent<ABLevelSelector>();
        sel.LevelIndex = lSystemIndex * RatingSystem.MAX_LEVELS;

        LoadNextScene("GameWorld", true, sel.UpdateLevelList);
    }

    public void SubmitRatings()
    {
        RatingSystem.SubmitRatings();
        // keep some levels (starred ones?)
        for (int i = 0; i < RatingSystem.MAX_LSYSTEMS; ++i)
        {
            // add starred level to kept list
            if (RatingSystem.isStarred[i])
            {
                RatingSystem.keptForEvolution.Add(new RatingSystem.LSystemEvolution(RatingSystem.lSystems[i]));
            }
        }
        // TODO: MUST HAVE A BACKUP PLAN IF PLAYER DOES NOT STAR ENOUGH LEVELS

        // keptforevolution will contain a list of levels to run evolution on
        // TODO: Run evolution here and replace RatingSystem.keptForEvolution with the list of newly created LSystems

        RatingSystem.ClearAll();
       
        //ABSceneManager.Instance.LoadScene("LevelSelectMenu");
        ABSceneManager.Instance.LoadScene("Evolution");
        //RatingSystem.GenerateXMLs(RatingSystem.CurrentLSystemIndex, 5); // hardcoded height
        //LoadScreenshots(RatingSystem.CurrentLSystemIndex);
        //GenerateNewLevels(RatingSystem.CurrentLSystemIndex);
    }

    // deprecated
    public string[] loadXMLsOld()
    {
        // Load levels in the resources folder
        TextAsset[] levelsData = Resources.LoadAll<TextAsset>(ABConstants.DEFAULT_LEVELS_FOLDER);

        string[] resourcesXml = new string[levelsData.Length];
        for (int i = 0; i < levelsData.Length; i++)
            resourcesXml[i] = levelsData[i].text;


#if UNITY_WEBGL && !UNITY_EDITOR

		// WebGL builds does not load local files
		string[] streamingXml = new string[0];

#else
        // Load levels in the streaming folder
        string levelsPath = Application.dataPath + ABConstants.CUSTOM_LEVELS_FOLDER;
        string[] levelFiles = Directory.GetFiles(levelsPath, "*.xml");

        string[] streamingXml = new string[levelFiles.Length];
        for (int i = 0; i < levelFiles.Length; i++)
            streamingXml[i] = File.ReadAllText(levelFiles[i]);

#endif

        // Combine the two sources of levels
        string[] allXmlFiles = new string[resourcesXml.Length + streamingXml.Length];
        resourcesXml.CopyTo(allXmlFiles, 0);
        streamingXml.CopyTo(allXmlFiles, resourcesXml.Length);

        _startPos.x = Mathf.Clamp(_startPos.x, 0, 1f) * Screen.width;
        _startPos.y = Mathf.Clamp(_startPos.y, 0, 1f) * Screen.height;

        LevelList.Instance.LoadLevelsFromSource(allXmlFiles);

        return allXmlFiles;
    }

    public static string[] loadXMLs()
    {
        string[] allXmls = new string[RatingSystem.MAX_LEVELS * RatingSystem.MAX_LSYSTEMS];
        //Debug.Log("TOTAL: " + (RatingSystem.MAX_LEVELS * RatingSystem.MAX_LSYSTEMS));

        for (int i = 0; i < RatingSystem.MAX_LSYSTEMS; ++i)
        {
            for (int j = 0; j < RatingSystem.MAX_LEVELS; ++j)
            {
                //Debug.Log(i * RatingSystem.MAX_LEVELS + j);
                allXmls[i * RatingSystem.MAX_LEVELS + j] = RatingSystem.levelData[i][j].levelXML;
            }
        }

        //_startPos.x = Mathf.Clamp(_startPos.x, 0, 1f) * Screen.width;
        //_startPos.y = Mathf.Clamp(_startPos.y, 0, 1f) * Screen.height;

        LevelList.Instance.LoadLevelsFromSource(allXmls);

        foreach (string xml in allXmls)
        {
            Debug.Log(xml);
        }

        return allXmls;
    }

    // Use this for initialization
    void Start () {

        if (RatingSystem.lSystems.Count <= 0)
        {
            Debug.Log("Initializing LSystems...");
            SqlManager.SqlManagerInstance.StartCoroutine(SqlConnection.GetPopulation(true, 12, retrievedLSystems =>
            {
                Debug.Log("got 'em");
                foreach (LSystemWrapper w in retrievedLSystems)
                {
                    Debug.Log(w.GetString());
                }
                InitializeLSystems(retrievedLSystems);
                loadXMLs();
                LoadScreenshots(0);
            }));
        }
        else
        {
            for (int i = 0; i < RatingSystem.levelData.Count; ++i)
            {
                if (RatingSystem.levelData[i][0].levelSprite == null)
                {
                    //Debug.Log("LSystem " + i + " does not have screenshots generated");
                    LoadScreenshots(i);
                    return;
                }
            }
            RatingSystem.EndGeneratingScreenshots();
        }
    }
}
