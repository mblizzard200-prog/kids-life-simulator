using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scoring")]
    [SerializeField] private int browniePoints;

    [Header("Mom Anger")]
    [SerializeField] private float momAnger;
    [SerializeField] private float maxAnger = 100f;
    [SerializeField] private float passiveAngerIncreasePerSecond = 1f;

    private UIManager uiManager;
    private TaskManager taskManager;
    private bool isGameOver;

    public int BrowniePoints => browniePoints;
    public float MomAnger => momAnger;
    public float MaxAnger => maxAnger;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        taskManager = FindObjectOfType<TaskManager>();

        RefreshUI();
        ApplyCurrentTaskToUI();
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (taskManager != null && !taskManager.AreAllTasksComplete)
        {
            AddMomAnger(passiveAngerIncreasePerSecond * Time.deltaTime);
        }

        if (momAnger >= maxAnger)
        {
            EndGame(false);
        }
    }

    public void AddBrowniePoints(int amount)
    {
        if (isGameOver)
        {
            return;
        }

        browniePoints = Mathf.Max(0, browniePoints + amount);
        AddMomAnger(-amount * 0.5f);
        RefreshUI();
    }

    public void AddMomAnger(float amount)
    {
        if (isGameOver)
        {
            return;
        }

        momAnger = Mathf.Clamp(momAnger + amount, 0f, maxAnger);
        RefreshUI();
    }

    public void NotifyTaskProgressChanged()
    {
        ApplyCurrentTaskToUI();

        if (taskManager != null && taskManager.AreAllTasksComplete)
        {
            EndGame(true);
        }
    }

    private void ApplyCurrentTaskToUI()
    {
        if (uiManager == null || taskManager == null)
        {
            return;
        }

        uiManager.SetTaskText(taskManager.GetCurrentTaskDisplayText());
    }

    private void RefreshUI()
    {
        if (uiManager == null)
        {
            return;
        }

        uiManager.SetBrowniePoints(browniePoints);
        uiManager.SetMomAnger(momAnger, maxAnger);
    }

    private void EndGame(bool won)
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        if (uiManager != null)
        {
            uiManager.ShowGameOver(won, browniePoints);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
