using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
   // private List<HighscoreEntry> highscoreEntryList;
    private List<Transform> highscoreEntryTransformList;

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        // AddHighscoreEntry(4000, "Mot");

        /*highscoreEntryList = new List<HighscoreEntry>()
        {
            new HighscoreEntry { score = 9990, name = "Aba"},
            new HighscoreEntry { score = 181, name = "Tap"},
            new HighscoreEntry { score = 72, name = "Derf"},
            new HighscoreEntry { score = 4000, name = "Mot"},
            new HighscoreEntry { score = 3000, name = "Tab"},
            new HighscoreEntry { score = 5000, name = "Sap"},
            new HighscoreEntry { score = 9000, name = "Nap"},
            new HighscoreEntry { score = 100, name = "Capt"},
        };*/

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Caps the number of Highscores captures at 10
        if (highscores.highscoreEntryList.Count > 10)
        {
            for (int h = highscores.highscoreEntryList.Count; h > 10; h--)
            {
                highscores.highscoreEntryList.RemoveAt(10);
            }
        }

        // Sort entry list by Score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    // swap scores around
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 15f;

        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "th"; break;
            case 1: rankString = "1st"; break;
            case 2: rankString = "2nd"; break;
            case 3: rankString = "3rd"; break;
        }

        // Assign Position Number
        entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        // Get the Players Score
        int score = highscoreEntry.score;                                                  // TO-DO:    to connect to the Players Score
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        // Get the Player Name
        string name = highscoreEntry.name;                                                                // TO-DO:    to connect to the Players Name
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        // Set baclground visible odds and evens, easier to read
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        // Highlights First
        if (rank == 1)
        {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.red;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.red;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.red;
        }
        
        // Set Trophy
        switch (rank)
        {
            default:
                entryTransform.Find("trophy").gameObject.SetActive(false);
                break;
            case 1:
                entryTransform.Find("trophy").GetComponent<Image>().color = Color.yellow;
                break;
            case 2:
                entryTransform.Find("trophy").GetComponent<Image>().color = Color.grey;
                break;
            case 3:
                entryTransform.Find("trophy").GetComponent<Image>().color = Color.white;
                break;
        }

        transformList.Add(entryTransform);
    }

    private void AddHighscoreEntry(int score, string name)
    {
        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };

        // Load saved highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Caps the number of Highscores captures at 10
        if (highscores.highscoreEntryList.Count > 10)
        {
            for (int h = highscores.highscoreEntryList.Count; h > 10; h--)
            {
                highscores.highscoreEntryList.RemoveAt(10);
            }
        }

        // Save updated Highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }


    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    // Represents a single High score entry
    [Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;

    }
}
