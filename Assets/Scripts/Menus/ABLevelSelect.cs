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
    private List<GameObject> tempLevelButtons;


    public void InitializeLSystems()
    {
        /* TODO: 
         * populate RatingSystem.lSystems with 6 LSystems
         */ 

        for (int i = 0; i < RatingSystem.lSystems.Count; ++i)
        {
            GenerateNewLevels(i);
        }
    }

    public void LoadScreenshots(int lSystemIndex)
    {
        RatingSystem.StartGeneratingScreenshots(lSystemIndex);
        ABLevelSelector sel = gameObject.AddComponent<ABLevelSelector>();
        sel.LevelIndex = lSystemIndex * RatingSystem.MAX_LEVELS;

        LoadNextScene("GameWorld", true, sel.UpdateLevelList);
    }

    public void GenerateXML(int lSystemIndex, string filename)
    {
        /* TODO: 
         * Generate Level from L System into the resources/levels directory (should be a constant here somewhere...)
         * Set filemode to overwrite the file if it already exists
         */
    }

    public void GenerateNewLevels(int lSystemIndex)
    {
        for (int i = 0; i < RatingSystem.MAX_LEVELS; ++i)
        {
            GenerateXML(lSystemIndex, String.Format("level-{0:D2}.xml", lSystemIndex * RatingSystem.MAX_LEVELS + i));
        }
    }

    public void SubmitRatings()
    {
        RatingSystem.SubmitRatings();
        GenerateNewLevels(RatingSystem.CurrentLSystemIndex);

    }

    public string[] loadXMLs()
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

    public void DisplayLSystems()
    {
        lSystemButtons.SetActive(true);
        levelButtons.SetActive(false);

        foreach (GameObject obj in tempLevelButtons)
        {
            Destroy(obj);
        }
    }

    public void DisplayLevels(int lSystemIndex)
    {
        if (RatingSystem.levelSprites[lSystemIndex].Count <= 0)
        {
            LoadScreenshots(lSystemIndex);
        }
        else
        {
            lSystemButtons.SetActive(false);
            levelButtons.SetActive(true);

            RatingSystem.CurrentLSystemIndex = lSystemIndex;
            int j = 0;
            RatingSystem.EndGeneratingScreenshots();
            for (int i = 0; i < RatingSystem.MAX_LEVELS; i++)
            {

                GameObject obj = Instantiate(_levelSelector, Vector2.zero, Quaternion.identity) as GameObject;
                obj.GetComponent<Image>().sprite = RatingSystem.levelSprites[RatingSystem.CurrentLSystemIndex][i];

                obj.transform.SetParent(_canvas.transform, false);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = new Vector3(0.16f, 0.16f, 1f);

                //Vector2 pos = _startPos + new Vector2((i % _lines) * _buttonSize.x, j * _buttonSize.y);

                Vector2 pos = _startPos + new Vector2((i % _lines) * (_camera.scaledPixelWidth / 3.1f), j * (_camera.scaledPixelHeight / 3.1f));
                obj.transform.position = pos;

                //Debug.Log(obj.transform.position);

                ABLevelSelector sel = obj.AddComponent<ABLevelSelector>();
                sel.LevelIndex = lSystemIndex * RatingSystem.MAX_LEVELS + i;

                Button selectButton = obj.GetComponent<Button>();

                selectButton.onClick.AddListener(delegate
                {
                    LoadNextScene("GameWorld", true, sel.UpdateLevelList);
                });

                Text selectText = selectButton.GetComponentInChildren<Text>();
                selectText.text = "";// + (i + 1);

                // create rating button
                GameObject star = Instantiate(_ratingStar, Vector2.zero, Quaternion.identity) as GameObject;
                star.transform.SetParent(_canvas.transform, false);
                star.transform.localPosition = Vector3.zero;
                star.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

                star.transform.position = pos + new Vector2(-_camera.scaledPixelWidth / 8f, _camera.scaledPixelHeight / 10f);

                if (RatingSystem.pressedButtons[RatingSystem.CurrentLSystemIndex][i])
                {
                    star.GetComponent<Image>().color = Color.yellow;
                }
                else
                {
                    star.GetComponent<Image>().color = Color.black;
                }
                star.GetComponent<Button>().onClick.AddListener(delegate
                {
                    RatingSystem.RateLevel(sel.LevelIndex, star);
                //LoadNextScene("GameWorld", true, sel.UpdateLevelList);
            });

                tempLevelButtons.Add(obj);
                tempLevelButtons.Add(star);

                if ((i + 1) % _lines == 0)
                    j--;
            }
        }
    }

    // Use this for initialization
    void Start () {

        tempLevelButtons = new List<GameObject>();

        if (RatingSystem.lSystems.Count <= 0)
        {
            InitializeLSystems();
        }

        loadXMLs();

        if (RatingSystem.CurrentLSystemIndex >= 0)
        {
            DisplayLevels(RatingSystem.CurrentLSystemIndex);
        }
    }
}
