using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public GameObject laserPrefab;
    Vector3 hanLaserSpawn = new Vector3(-2f, -1.85f, -1f);
    Vector3 greedoLaserSpawn = new Vector3(0.55f, 0.5f, -1f);

    public Button hanBtn, greedoBtn;
    public Text scoreTxt, highScoreTxt;

    bool hanShotFirst = true;
    int score = 0;
    int highScore = 0;
    string savePath;

    private void Awake()
    {
        Screen.fullScreen = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        savePath = Application.persistentDataPath + "save.hs";
        Debug.Log(savePath);

        hanBtn.interactable = false;
        greedoBtn.interactable = false;

        if (LoadGame())
        {
            highScoreTxt.text = "High Score: " + highScore;
        }
        else
        {
            highScoreTxt.text = "High Score: " + 0;
        }

        StartCoroutine(StartRound(1f));

    }

    IEnumerator StartRound(float difficulty)
    {

        hanBtn.interactable = false;
        greedoBtn.interactable = false;

        hanShotFirst = 0 == Random.Range(0, 2);

        yield return new WaitForSeconds(1f);

        if (hanShotFirst)
        {
            Debug.Log("Han shot first!");
            Instantiate(laserPrefab, hanLaserSpawn, Quaternion.Euler(0f, 0f, 270f));

            //yield return new WaitForSeconds(difficulty);
            yield return new WaitForSecondsRealtime(difficulty);

            Instantiate(laserPrefab, greedoLaserSpawn, Quaternion.Euler(0f, 0f, 90f));
        }
        else
        {

            Debug.Log("Greedo shot first!");
            Instantiate(laserPrefab, greedoLaserSpawn, Quaternion.Euler(0f, 0f, 90f));

            //yield return new WaitForSeconds(difficulty);
            yield return new WaitForSecondsRealtime(difficulty);

            Instantiate(laserPrefab, hanLaserSpawn, Quaternion.Euler(0f, 0f, 270f));

        }

        yield return new WaitForSeconds(0.75f);

        hanBtn.interactable = true;
        greedoBtn.interactable = true;

    }

    float getDifficulty(int x)
    {
        float temp = 1f / Mathf.Exp(x * 0.15f);
        Debug.Log("Difficuty: " + temp);
        return temp;
    }

    public void HanButtonPressed()
    {
        Debug.Log("Han Button Pressed");
        if (hanShotFirst)
        {

            score += 1;

            if (score > highScore)
            {
                highScore = score;
                highScoreTxt.text = "High Score: " + highScore;
                SaveGame();
            }

        }
        else
        {
            score = 0;
        }

        scoreTxt.text = "Score: " + score;

        StartCoroutine(StartRound(getDifficulty(score)));

    }

    public void GreedoButtonPressed()
    {
        Debug.Log("Greedo Button Pressed");
        if (!hanShotFirst)
        {

            score += 1;

            if (score > highScore)
            {
                highScore = score;
                highScoreTxt.text = "High Score: " + highScore;
                SaveGame();
            }

        }
        else
        {

            score = 0;

        }

        scoreTxt.text = "Score: " + score;

        StartCoroutine(StartRound(getDifficulty(score)));

    }

    public void RestartButtonPressed()
    {

        score = 0;
        scoreTxt.text = "Score: " + score;
        StartCoroutine(StartRound(1f));

    }

    void SaveGame()
    {

        Save save = new Save(highScore);

        var binaryFormatter = new BinaryFormatter();

        using (var fileStream = File.Create(savePath))
        {
            binaryFormatter.Serialize(fileStream, save);
        }

        Debug.Log("File Saved");
    }

    bool LoadGame()
    {

        if (File.Exists(savePath))
        {

            Save save;

            var binaryFormatter = new BinaryFormatter();

            using (var fileStream = File.Open(savePath, FileMode.Open))
            {
                save = (Save)binaryFormatter.Deserialize(fileStream);
            }

            highScore = save.getHighScore();
            
            Debug.Log("File Loaded");

            return true;
        }

        return false;

    }

}

[System.Serializable]
public class Save
{
    public Save()
    {
        highScore = 0;
    }
    public Save(int x)
    {
        highScore = x;
    }

    int highScore;

    public int getHighScore()
    {
        return highScore;
    }

    public void setHighScore(int x)
    {
        highScore = x;
    }
}
