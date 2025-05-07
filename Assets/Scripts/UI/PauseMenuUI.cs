using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PauseMenuUI : MonoBehaviour
{
    private VisualElement pauseRoot;
    private VisualElement bossRoot;

    public static PauseMenuUI instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        var doc = GetComponent<UIDocument>();
        var root = doc.rootVisualElement;

        pauseRoot = root.Q<VisualElement>("Container");
        pauseRoot.style.display = DisplayStyle.None;

        var saveButton = root.Q<Button>("SaveGameButton");
        var restartButton = root.Q<Button>("RestartLevelButton");

        saveButton.clicked += () => GameManager.instance.SaveGame();
        restartButton.clicked += () => GameManager.instance.ResetLevel();

        var bossUI = GameObject.Find("BossUI")?.GetComponent<UIDocument>();
        bossRoot = bossUI?.rootVisualElement.Q<VisualElement>("Container");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowPauseMenu()
    {
        pauseRoot.style.display = DisplayStyle.Flex;
        if (bossRoot != null) bossRoot.style.display = DisplayStyle.None;
    }

    public void HidePauseMenu()
    {
        pauseRoot.style.display = DisplayStyle.None;
        if (bossRoot != null) bossRoot.style.display = DisplayStyle.Flex;
    }
}
