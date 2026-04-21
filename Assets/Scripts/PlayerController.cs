using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float cameraDistance = 4f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private float yaw;
    private float pitch = 15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (cameraHolder == null)
        {
            Transform holder = transform.Find("CameraHolder");
            if (holder != null)
            {
                cameraHolder = holder;
            }
        }
    }

    private void Start()
    {
        if (cameraHolder == null)
        {
            GameObject holderObject = new GameObject("CameraHolder");
            holderObject.transform.SetParent(transform);
            holderObject.transform.localPosition = Vector3.up * cameraHeight;
            cameraHolder = holderObject.transform;
        }

        Camera cameraComponent = Camera.main;
        if (cameraComponent == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraComponent = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        cameraComponent.transform.SetParent(cameraHolder);
        cameraComponent.transform.localPosition = new Vector3(0f, 0f, -cameraDistance);
        cameraComponent.transform.localRotation = Quaternion.identity;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch = Mathf.Clamp(pitch - mouseY, -25f, 70f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cameraHolder != null)
        {
            cameraHolder.localPosition = Vector3.up * cameraHeight;
            cameraHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 worldDirection = transform.TransformDirection(inputDirection);

        Vector3 targetVelocity = worldDirection * moveSpeed;
        Vector3 currentVelocity = rb.velocity;
        Vector3 velocityChange = new Vector3(targetVelocity.x - currentVelocity.x, 0f, targetVelocity.z - currentVelocity.z);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}
