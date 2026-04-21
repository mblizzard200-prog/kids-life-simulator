using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD References")]
    [SerializeField] private TextMeshProUGUI browniePointsText;
    [SerializeField] private Image angerMeterBar;
    [SerializeField] private TextMeshProUGUI angerText;
    [SerializeField] private TextMeshProUGUI taskDisplayText;
    [SerializeField] private TextMeshProUGUI interactionPromptText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsureUIExists();
        AutoAssignFromScene();
    }

    private void Start()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            UpdateGameUI(GameManager.Instance.BrowniePoints, GameManager.Instance.MomsAnger, GameManager.Instance.GetAngerColor());
        }
    }

    public void UpdateGameUI(int browniePoints, float angerPercent, Color angerColor)
    {
        if (browniePointsText != null)
        {
            browniePointsText.text = $"Brownie Points: {browniePoints}";
        }

        if (angerMeterBar != null)
        {
            angerMeterBar.fillAmount = Mathf.Clamp01(angerPercent / 100f);
            angerMeterBar.color = angerColor;
        }

        if (angerText != null)
        {
            angerText.text = $"Mom's Anger: {Mathf.RoundToInt(angerPercent)}%";
            angerText.color = angerColor;
        }
    }

    public void SetTaskDisplay(string taskText)
    {
        if (taskDisplayText != null)
        {
            taskDisplayText.text = taskText;
        }
    }

    public void SetInteractionPrompt(string promptText, bool visible)
    {
        if (interactionPromptText == null)
        {
            return;
        }

        interactionPromptText.text = promptText;
        interactionPromptText.gameObject.SetActive(visible);
    }

    public void ShowGameOver(string message)
    {
        if (gameOverText == null)
        {
            return;
        }

        gameOverText.text = message;
        gameOverText.gameObject.SetActive(true);
    }

    private void AutoAssignFromScene()
    {
        if (browniePointsText == null)
        {
            GameObject go = GameObject.Find("BrowniePointsText");
            if (go != null) browniePointsText = go.GetComponent<TextMeshProUGUI>();
        }

        if (angerMeterBar == null)
        {
            GameObject go = GameObject.Find("AngerMeterBar");
            if (go != null) angerMeterBar = go.GetComponent<Image>();
        }

        if (angerText == null)
        {
            GameObject go = GameObject.Find("AngerText");
            if (go != null) angerText = go.GetComponent<TextMeshProUGUI>();
        }

        if (taskDisplayText == null)
        {
            GameObject go = GameObject.Find("TaskDisplayText");
            if (go != null) taskDisplayText = go.GetComponent<TextMeshProUGUI>();
        }

        if (interactionPromptText == null)
        {
            GameObject go = GameObject.Find("InteractionPromptText");
            if (go != null) interactionPromptText = go.GetComponent<TextMeshProUGUI>();
        }

        if (gameOverText == null)
        {
            GameObject go = GameObject.Find("GameOverText");
            if (go != null) gameOverText = go.GetComponent<TextMeshProUGUI>();
        }
    }

    private void EnsureUIExists()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (GameObject.Find("BrowniePointsText") == null)
        {
            CreateTMPText("BrowniePointsText", canvas.transform, new Vector2(15f, -15f), new Vector2(0f, 1f), "Brownie Points: 0", 32, TextAlignmentOptions.TopLeft);
        }

        if (GameObject.Find("AngerMeterBackground") == null)
        {
            GameObject background = CreateUIObject("AngerMeterBackground", canvas.transform, true);
            RectTransform rect = background.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -20f);
            rect.sizeDelta = new Vector2(500f, 40f);

            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.color = new Color(0f, 0f, 0f, 0.45f);
        }

        if (GameObject.Find("AngerMeterBar") == null)
        {
            Transform parent = GameObject.Find("AngerMeterBackground")?.transform ?? canvas.transform;
            GameObject bar = CreateUIObject("AngerMeterBar", parent, true);
            RectTransform rect = bar.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = new Vector2(4f, 4f);
            rect.offsetMax = new Vector2(-4f, -4f);

            Image image = bar.GetComponent<Image>();
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillAmount = 0f;
            image.color = Color.green;
        }

        if (GameObject.Find("AngerText") == null)
        {
            CreateTMPText("AngerText", canvas.transform, new Vector2(0f, -70f), new Vector2(0.5f, 1f), "Mom's Anger: 0%", 30, TextAlignmentOptions.Top);
        }

        if (GameObject.Find("TaskDisplayText") == null)
        {
            CreateTMPText("TaskDisplayText", canvas.transform, new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), "Current Task: Morning Routine (use Sink)", 34, TextAlignmentOptions.Center);
        }

        if (GameObject.Find("InteractionPromptText") == null)
        {
            CreateTMPText("InteractionPromptText", canvas.transform, new Vector2(0f, -120f), new Vector2(0.5f, 0f), "Press E to interact", 28, TextAlignmentOptions.Bottom);
        }

        if (GameObject.Find("GameOverText") == null)
        {
            TextMeshProUGUI gameOver = CreateTMPText("GameOverText", canvas.transform, Vector2.zero, new Vector2(0.5f, 0.5f), "Game Over! Mom is too angry!", 48, TextAlignmentOptions.Center);
            gameOver.color = Color.red;
        }
    }

    private static TextMeshProUGUI CreateTMPText(
        string objectName,
        Transform parent,
        Vector2 anchoredPosition,
        Vector2 anchor,
        string text,
        float fontSize,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = CreateUIObject(objectName, parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(900f, 90f);

        TextMeshProUGUI tmp = textObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = alignment;
        return tmp;
    }

    private static GameObject CreateUIObject(string name, Transform parent, bool includeImage)
    {
        GameObject uiObject = includeImage
            ? new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image))
            : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        uiObject.transform.SetParent(parent, false);
        return uiObject;
    }
}
