using UnityEngine;
using UnityEngine.UIElements;

public class FighterUI : MonoBehaviour
{
    [SerializeField] private Fighter fighter;
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private float transitionSpeed = 2f;

    private VisualElement root;

    private VisualElement hpFill;
    private VisualElement hpTransition;
    private float hpCurrentRatio;

    private VisualElement staminaFill;
    private VisualElement staminaTransition;
    private float staminaCurrentRatio;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("UI root is null.");
            return;
        }

        var hpBar = root.Q<VisualElement>("hp-bar");
        hpFill = hpBar?.Q<VisualElement>(className: "bar-fill");
        hpTransition = hpBar?.Q<VisualElement>(className: "bar-transition");

        var staminaBar = root.Q<VisualElement>("stamina-bar");
        staminaFill = staminaBar?.Q<VisualElement>(className: "bar-fill");
        staminaTransition = staminaBar?.Q<VisualElement>(className: "bar-transition");

        if (hpFill == null || hpTransition == null)
            Debug.LogError("HP bar elements missing.");
        if (staminaFill == null || staminaTransition == null)
            Debug.LogError("Stamina bar elements missing.");

        // Init ratios
        hpCurrentRatio = fighter.currentHealth / fighter.maxHealth;
        staminaCurrentRatio = fighter.currentStamina / fighter.maxStamina;
    }

    private void Update()
    {
        float hpTarget = fighter.currentHealth / fighter.maxHealth;
        float staminaTarget = fighter.currentStamina / fighter.maxStamina;

        // Update immediate bars
        hpFill.style.width = Length.Percent(hpTarget * 100f);
        staminaFill.style.width = Length.Percent(staminaTarget * 100f);

        // Lerp transitions
        hpCurrentRatio = Mathf.MoveTowards(hpCurrentRatio, hpTarget, Time.deltaTime * transitionSpeed);
        staminaCurrentRatio = Mathf.MoveTowards(staminaCurrentRatio, staminaTarget, Time.deltaTime * transitionSpeed);

        hpTransition.style.width = Length.Percent(hpCurrentRatio * 100f);
        staminaTransition.style.width = Length.Percent(staminaCurrentRatio * 100f);
    }
}
