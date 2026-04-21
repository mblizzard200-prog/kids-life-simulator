using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private int browniePoints;
    [SerializeField, Range(0f, 100f)] private float momsAnger;

    public int BrowniePoints => browniePoints;
    public float MomsAnger => momsAnger;
    public bool IsGameOver { get; private set; }

    public event Action<int, float, Color> OnGameStateChanged;
    public event Action OnGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        EnsureSceneSetup();
        PublishState();
    }

    public void AddBrowniePoints(int amount)
    {
        if (IsGameOver || amount == 0)
        {
            return;
        }

        browniePoints = Mathf.Max(0, browniePoints + amount);
        PublishState();
    }

    public void AdjustMomAnger(float delta)
    {
        if (IsGameOver || Mathf.Approximately(delta, 0f))
        {
            return;
        }

        momsAnger = Mathf.Clamp(momsAnger + delta, 0f, 100f);
        PublishState();

        if (momsAnger >= 100f)
        {
            TriggerGameOver();
        }
    }

    public Color GetAngerColor()
    {
        if (momsAnger < 40f)
        {
            return Color.green;
        }

        if (momsAnger < 70f)
        {
            return Color.yellow;
        }

        return Color.red;
    }

    private void PublishState()
    {
        OnGameStateChanged?.Invoke(browniePoints, momsAnger, GetAngerColor());

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateGameUI(browniePoints, momsAnger, GetAngerColor());
        }
    }

    private void TriggerGameOver()
    {
        IsGameOver = true;
        OnGameOver?.Invoke();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver("Game Over! Mom is too angry!");
        }
    }

    private void EnsureSceneSetup()
    {
        EnsureLayerSetup();
        EnsurePlayer();
        EnsureGround();
        EnsureInteractiveObjects();
    }

    private static void EnsureLayerSetup()
    {
        if (LayerMask.NameToLayer("Ground") == -1)
        {
            Debug.LogWarning("Ground layer does not exist in TagManager. Ground object will use Default layer.");
        }
    }

    private static void EnsurePlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            return;
        }

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1f, 0f);

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        player.AddComponent<PlayerController>();

        GameObject cameraHolder = new GameObject("CameraHolder");
        cameraHolder.transform.SetParent(player.transform, false);
        cameraHolder.transform.localPosition = new Vector3(0f, 0.8f, 0f);

        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(cameraHolder.transform, false);
        cameraObject.transform.localPosition = new Vector3(0f, 1.2f, -4f);
        cameraObject.transform.localRotation = Quaternion.identity;
    }

    private static void EnsureGround()
    {
        if (GameObject.Find("Ground") != null)
        {
            return;
        }

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0f, -1f, 0f);
        ground.transform.localScale = new Vector3(20f, 1f, 20f);

        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer >= 0)
        {
            ground.layer = groundLayer;
        }

        Renderer renderer = ground.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.45f, 0.45f, 0.45f, 1f);
        }
    }

    private static void EnsureInteractiveObjects()
    {
        EnsureInteractiveObject("Sink", PrimitiveType.Cube, new Vector3(5f, 1f, 0f), new Color(0.5f, 0.4f, 0.35f), "MorningRoutine");
        EnsureInteractiveObject("Toy", PrimitiveType.Cube, new Vector3(-5f, 1f, 0f), new Color(0.95f, 0.2f, 0.9f), "RoomCleaning");
        EnsureInteractiveObject("Stove", PrimitiveType.Cube, new Vector3(0f, 1f, 5f), new Color(0.85f, 0.1f, 0.1f), "HelpMomCook");
        EnsureInteractiveObject("Friend", PrimitiveType.Capsule, new Vector3(0f, 1f, -5f), new Color(0.2f, 0.45f, 0.95f), "HelpFriend");
        EnsureInteractiveObject("PlayArea", PrimitiveType.Cube, new Vector3(-5f, 1f, 3f), new Color(0.9f, 0.85f, 0.1f), "PlayResponsibly");
    }

    private static void EnsureInteractiveObject(string objectName, PrimitiveType primitiveType, Vector3 position, Color color, string taskId)
    {
        if (GameObject.Find(objectName) != null)
        {
            return;
        }

        GameObject interactiveObject = GameObject.CreatePrimitive(primitiveType);
        interactiveObject.name = objectName;
        interactiveObject.transform.position = position;

        Collider collider = interactiveObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        InteractiveObject script = interactiveObject.GetComponent<InteractiveObject>();
        if (script == null)
        {
            script = interactiveObject.AddComponent<InteractiveObject>();
        }

        script.Configure(taskId, "Press E to interact");

        Renderer renderer = interactiveObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}
