using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public int currentLevel = 1;

    public bool IsPaused { get; private set; }

    private string savePath => Application.persistentDataPath + "/save.txt";

    private void Awake()    
    {   
        Debug.Log(Application.persistentDataPath);

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region PAUSE / RESUME

    public void TogglePause()
    {
        if (IsPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        IsPaused = true;

        PauseMenuUI.instance.ShowPauseMenu();
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        IsPaused = false;

        PauseMenuUI.instance.HidePauseMenu();
    }

    #endregion

    public void SaveGame()
    {
        System.IO.File.WriteAllText(savePath, currentLevel.ToString());
        Debug.Log("Game saved: level " + currentLevel);
    }

    public void ResetLevel()
    {
        SaveGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        ResumeGame();
    }

    public void LoadGame()
    {
        if (System.IO.File.Exists(savePath))
        {
            string data = System.IO.File.ReadAllText(savePath);
            if (int.TryParse(data, out int level))
            {
                Debug.Log("Loaded saved level: " + level);
                currentLevel = level;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Duel" + level);
            }
        }
    }

    public void LoadNextLevel()
    {
        if (currentLevel == 3)
        {
            Debug.Log("All NPC levels defeated!");
            //konec hry
            return;
        }
        currentLevel++;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Duel" + currentLevel);
        
    }

    public void StartNewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Duel" + currentLevel);
    }
}