using UnityEngine;
using UnityEngine.SceneManagement;
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

        SceneManager.sceneLoaded += OnSceneLoaded;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                SceneManager.LoadScene("Duel" + level);
            }
        }
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        SceneManager.LoadScene("Duel" + currentLevel);
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Duel" + currentLevel);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterFighterDeathEvents();
    }

    //nenačte npc, protože se spawnují později
    private void RegisterFighterDeathEvents()
    {
        Fighter[] fighters = FindObjectsByType<Fighter>(FindObjectsSortMode.None);
        foreach (var fighter in fighters)
        {
            fighter.OnDeath += () => HandleFighterDeath(fighter);
        }
    }

    // přidá event na smrt NPC
    public void RegisterFighter(Fighter fighter)
    {
        fighter.OnDeath += () => HandleFighterDeath(fighter);
    }

    public void HandleFighterDeath(Fighter fighter)
    {
        if (fighter is Player)
        {
            Debug.Log("Player died!");
            EndGame();
        }else if(currentLevel == 3)
        {
            Debug.Log("Player defeated all NPCs!");
            EndGame();
        }
        else if (fighter is Knight)
        {
            Debug.Log("NPC died!");
            LoadNextLevel();
        }
    }

    public void EndGame()
    {
        PauseGame();
        Debug.Log("GAME OVER");
    }

}