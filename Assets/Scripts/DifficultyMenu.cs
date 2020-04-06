using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.SceneManagement;

public class DifficultyMenu : MonoBehaviour
{
    int select = 0;
    Vector3 cursorStart;

    Text[] difficultyText = new Text[3];
    Text[] deathsText = new Text[3];
    Text[] timeText = new Text[3];

    public string difficultySelect;

    private void Start()
    {
        var cursor = GameObject.Find("Cursor");
        cursorStart = cursor.transform.position;

        for (var i = 0; i < 3; i++)
        {
            difficultyText[i] = GameObject.Find($"Difficulty{i + 1}").GetComponent<Text>();
            deathsText[i] = GameObject.Find($"Deaths{i + 1}").GetComponent<Text>();
            timeText[i] = GameObject.Find($"Time{i + 1}").GetComponent<Text>();

            if (!File.Exists($"Data/save{i + 1}"))
            {
                difficultyText[i].text = "No Data";
                deathsText[i].text = $"Deaths: 0";
                timeText[i].text = $"Time: 0:00:00";
            }
            else
            {
                var text = File.ReadAllText($"Data/save{i + 1}");
                var saveFile = JsonUtility.FromJson<SaveFile>(text);

                difficultyText[i].text = saveFile.difficulty.ToString();
                deathsText[i].text = $"Deaths: {saveFile.death}";
                timeText[i].text = $"Time: {saveFile.time / 3600}:{saveFile.time / 60 % 60}:{saveFile.time % 60}";
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            select++;
            if (select == 3)
                select = 0;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            select--;
            if (select == -1)
                select = 2;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            World.savenum = select + 1;
            SceneManager.LoadScene(difficultySelect);
        }
        var cursor = GameObject.Find("Cursor");
        cursor.transform.position = cursorStart + new Vector3(240f * select, 0);
    }
}
