using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [Serializable]
    public class TaskDefinition
    {
        public string id;
        public string title;
        public int reward;
        public float angerPenalty;
    }

    public static TaskManager Instance { get; private set; }

    private readonly List<TaskDefinition> tasks = new List<TaskDefinition>();
    private int currentTaskIndex;

    public TaskDefinition CurrentTask => currentTaskIndex < tasks.Count ? tasks[currentTaskIndex] : null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeTasks();
    }

    private void Start()
    {
        UpdateTaskUI();
    }

    public void CompleteTask(string taskId)
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (CurrentTask == null)
        {
            return;
        }

        if (!string.Equals(CurrentTask.id, taskId, StringComparison.OrdinalIgnoreCase))
        {
            GameManager.Instance.AdjustMomAnger(CurrentTask.angerPenalty);
            if (UIManager.Instance != null)
            {
                UIManager.Instance.SetTaskDisplay($"Wrong task! Do: {CurrentTask.title}");
            }
            return;
        }

        GameManager.Instance.AddBrowniePoints(CurrentTask.reward);
        GameManager.Instance.AdjustMomAnger(-5f);

        currentTaskIndex++;
        UpdateTaskUI();

        if (CurrentTask == null && UIManager.Instance != null)
        {
            UIManager.Instance.SetTaskDisplay("All tasks complete! Great job!");
        }
    }

    private void InitializeTasks()
    {
        tasks.Clear();
        tasks.Add(new TaskDefinition { id = "MorningRoutine", title = "Morning Routine (use Sink)", reward = 10, angerPenalty = 10f });
        tasks.Add(new TaskDefinition { id = "RoomCleaning", title = "Room Cleaning (pick up Toy)", reward = 15, angerPenalty = 10f });
        tasks.Add(new TaskDefinition { id = "HelpMomCook", title = "Help Mom Cook (use Stove)", reward = 20, angerPenalty = 10f });
        tasks.Add(new TaskDefinition { id = "HelpFriend", title = "Help Friend (talk to Friend)", reward = 20, angerPenalty = 10f });
        tasks.Add(new TaskDefinition { id = "PlayResponsibly", title = "Play Responsibly (visit PlayArea)", reward = 25, angerPenalty = 10f });
    }

    private void UpdateTaskUI()
    {
        if (UIManager.Instance == null)
        {
            return;
        }

        if (CurrentTask == null)
        {
            UIManager.Instance.SetTaskDisplay("All tasks complete! Great job!");
        }
        else
        {
            UIManager.Instance.SetTaskDisplay($"Current Task: {CurrentTask.title}");
        }
    }
}
