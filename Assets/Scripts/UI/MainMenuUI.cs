using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    Button newGameButton;
    Button loadGameButton;

    private void Awake()
    {
        var ui = GetComponent<UIDocument>();
        var root = ui.rootVisualElement;

        newGameButton = root.Q<Button>("StartNewGameButton");
        loadGameButton = root.Q<Button>("LoadGameButton");

        newGameButton.clicked += () =>
        {
            GameManager.instance.currentLevel = 1;
            SceneManager.LoadScene("Duel");
        };

        loadGameButton.clicked += () =>
        {
            GameManager.instance.LoadGame();
        };
    }
}
