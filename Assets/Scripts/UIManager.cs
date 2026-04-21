using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI browniePointsText;
    [SerializeField] private Image angerMeterBar;
    [SerializeField] private TextMeshProUGUI angerText;
    [SerializeField] private TextMeshProUGUI taskDisplayText;

    [Header("Prompt")]
    [SerializeField] private TextMeshProUGUI interactionPromptText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private void Awake()
    {
        EnsureUIExists();
    }

    public void SetBrowniePoints(int points)
    {
        if (browniePointsText != null)
        {
            browniePointsText.text = $"Brownie Points: {points}";
        }
    }

    public void SetMomAnger(float anger, float maxAnger)
    {
        float normalized = maxAnger <= 0f ? 0f : Mathf.Clamp01(anger / maxAnger);

        if (angerMeterBar != null)
        {
            angerMeterBar.fillAmount = normalized;
            angerMeterBar.color = Color.Lerp(Color.green, Color.red, normalized);
        }

        if (angerText != null)
        {
            angerText.text = $"Mom's Anger: {Mathf.RoundToInt(normalized * 100f)}%";
        }
    }

    public void SetTaskText(string text)
    {
        if (taskDisplayText != null)
        {
            taskDisplayText.text = text;
        }
    }

    public void SetInteractionPrompt(string prompt)
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.text = prompt;
        }
    }

    public void ShowGameOver(bool won, int score)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.text = won
                ? $"Great job! You finished all tasks!\nFinal Brownie Points: {score}"
                : $"Mom got too angry!\nFinal Brownie Points: {score}";
        }
    }

    public TextMeshProUGUI GetTaskDisplayText()
    {
        return taskDisplayText;
    }

    private void EnsureUIExists()
    {
        if (browniePointsText != null && angerMeterBar != null && angerText != null && taskDisplayText != null)
        {
            return;
        }

        Canvas existingCanvas = FindObjectOfType<Canvas>();
        Canvas canvas;

        if (existingCanvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }
        else
        {
            canvas = existingCanvas;

            if (canvas.GetComponent<CanvasScaler>() == null)
            {
                CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }

            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        if (browniePointsText == null)
        {
            browniePointsText = CreateLabel(canvas.transform, "BrowniePointsText", new Vector2(20, -20), TextAlignmentOptions.TopLeft, 32);
        }

        if (angerText == null)
        {
            angerText = CreateLabel(canvas.transform, "AngerText", new Vector2(0, -20), TextAlignmentOptions.Top, 28);
        }

        if (taskDisplayText == null)
        {
            taskDisplayText = CreateLabel(canvas.transform, "TaskDisplayText", new Vector2(0, -180), TextAlignmentOptions.Center, 36);
        }

        if (interactionPromptText == null)
        {
            interactionPromptText = CreateLabel(canvas.transform, "InteractionPromptText", new Vector2(0, 120), TextAlignmentOptions.Bottom, 28);
        }

        if (angerMeterBar == null)
        {
            GameObject meterBackground = new GameObject("AngerMeterBackground", typeof(RectTransform), typeof(Image));
            meterBackground.transform.SetParent(canvas.transform, false);
            RectTransform bgRect = meterBackground.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.5f, 1f);
            bgRect.anchorMax = new Vector2(0.5f, 1f);
            bgRect.pivot = new Vector2(0.5f, 1f);
            bgRect.anchoredPosition = new Vector2(0f, -60f);
            bgRect.sizeDelta = new Vector2(360f, 30f);

            Image bgImage = meterBackground.GetComponent<Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.35f);

            GameObject meterBar = new GameObject("AngerMeterBar", typeof(RectTransform), typeof(Image));
            meterBar.transform.SetParent(meterBackground.transform, false);

            RectTransform barRect = meterBar.GetComponent<RectTransform>();
            barRect.anchorMin = Vector2.zero;
            barRect.anchorMax = Vector2.one;
            barRect.offsetMin = Vector2.zero;
            barRect.offsetMax = Vector2.zero;

            angerMeterBar = meterBar.GetComponent<Image>();
            angerMeterBar.type = Image.Type.Filled;
            angerMeterBar.fillMethod = Image.FillMethod.Horizontal;
            angerMeterBar.fillOrigin = 0;
            angerMeterBar.fillAmount = 0f;
        }

        if (gameOverPanel == null)
        {
            gameOverPanel = new GameObject("GameOverPanel", typeof(RectTransform), typeof(Image));
            gameOverPanel.transform.SetParent(canvas.transform, false);

            RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = gameOverPanel.GetComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.7f);

            gameOverText = CreateLabel(gameOverPanel.transform, "GameOverText", Vector2.zero, TextAlignmentOptions.Center, 46);
            gameOverPanel.SetActive(false);
        }
    }

    private static TextMeshProUGUI CreateLabel(Transform parent, string name, Vector2 anchoredPosition, TextAlignmentOptions alignment, float fontSize)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(1200f, 180f);
        rectTransform.anchoredPosition = anchoredPosition;

        TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
        label.fontSize = fontSize;
        label.color = Color.white;
        label.alignment = alignment;
        label.enableWordWrapping = true;

        if (alignment == TextAlignmentOptions.TopLeft)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.sizeDelta = new Vector2(700f, 120f);
        }
        else if (alignment == TextAlignmentOptions.Top)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.sizeDelta = new Vector2(700f, 120f);
        }
        else if (alignment == TextAlignmentOptions.Bottom)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.sizeDelta = new Vector2(1000f, 100f);
        }

        return label;
    }
}
