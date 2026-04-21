using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private string taskId;
    [SerializeField] private bool isTaskCompletion = true;
    [SerializeField] private string prompt = "Press E to interact";

    private TaskManager taskManager;
    private UIManager uiManager;
    private bool playerInRange;

    private void Start()
    {
        taskManager = FindObjectOfType<TaskManager>();
        uiManager = FindObjectOfType<UIManager>();

        Collider primary = GetComponent<Collider>();
        primary.isTrigger = true;

        SphereCollider interactionRadius = GetComponent<SphereCollider>();
        if (interactionRadius == null)
        {
            interactionRadius = gameObject.AddComponent<SphereCollider>();
        }

        interactionRadius.isTrigger = true;
        interactionRadius.radius = 2.25f;
    }

    private void Update()
    {
        if (!playerInRange)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && taskManager != null && isTaskCompletion)
        {
            bool completed = taskManager.TryCompleteTask(taskId);
            if (completed)
            {
                uiManager?.SetInteractionPrompt("Task completed!");
                gameObject.SetActive(false);
            }
            else
            {
                uiManager?.SetInteractionPrompt("That is not the current task.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;
        uiManager?.SetInteractionPrompt(prompt);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;
        uiManager?.SetInteractionPrompt(string.Empty);
    }
}
