using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private string taskId;
    [SerializeField] private string interactionPrompt = "Press E to interact";

    private bool playerInRange;
    private bool wasCompleted;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    public void Configure(string id, string prompt)
    {
        taskId = id;
        interactionPrompt = prompt;
    }

    private void Update()
    {
        if (!playerInRange || wasCompleted || GameManager.Instance == null || GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            wasCompleted = true;

            if (TaskManager.Instance != null)
            {
                TaskManager.Instance.CompleteTask(taskId);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.SetInteractionPrompt(string.Empty, false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || wasCompleted)
        {
            return;
        }

        playerInRange = true;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetInteractionPrompt(interactionPrompt, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetInteractionPrompt(string.Empty, false);
        }
    }
}
