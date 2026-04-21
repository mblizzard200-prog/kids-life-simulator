using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [System.Serializable]
    public class TaskData
    {
        public string taskId;
        public string taskName;
        [TextArea] public string description;
        public int brownieReward;
        public bool completed;
    }

    [SerializeField] private TaskData[] tasks =
    {
        new TaskData { taskId = "Sink", taskName = "Morning Routine", description = "Brush your teeth at the bathroom sink.", brownieReward = 10 },
        new TaskData { taskId = "Toys", taskName = "Room Cleaning", description = "Put away the toys in the bedroom.", brownieReward = 15 },
        new TaskData { taskId = "Stove", taskName = "Help Mom Cook", description = "Help mom cook in the kitchen.", brownieReward = 20 },
        new TaskData { taskId = "Friend", taskName = "Help Friend", description = "Help your friend in the living room.", brownieReward = 15 },
        new TaskData { taskId = "PlayArea", taskName = "Play Responsibly", description = "Play responsibly in the bedroom play area.", brownieReward = 10 }
    };

    private int currentTaskIndex;

    public bool AreAllTasksComplete => currentTaskIndex >= tasks.Length;

    public string GetCurrentTaskDisplayText()
    {
        if (AreAllTasksComplete)
        {
            return "All tasks complete!";
        }

        TaskData current = tasks[currentTaskIndex];
        return $"Task {currentTaskIndex + 1}/{tasks.Length}: {current.taskName}\n{current.description}";
    }

    public bool TryCompleteTask(string taskId)
    {
        if (AreAllTasksComplete)
        {
            return false;
        }

        TaskData current = tasks[currentTaskIndex];
        if (!string.Equals(current.taskId, taskId, System.StringComparison.OrdinalIgnoreCase))
        {
            GameManager.Instance?.AddMomAnger(8f);
            return false;
        }

        if (current.completed)
        {
            return false;
        }

        current.completed = true;
        tasks[currentTaskIndex] = current;

        GameManager.Instance?.AddBrowniePoints(current.brownieReward);
        currentTaskIndex++;
        GameManager.Instance?.NotifyTaskProgressChanged();

        return true;
    }
}
