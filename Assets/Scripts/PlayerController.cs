using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 1.15f;

    [Header("Look")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 60f;

    private Rigidbody rb;
    private float pitch;
    private Vector2 moveInput;
    private bool jumpQueued;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (cameraHolder == null)
        {
            Transform found = transform.Find("CameraHolder");
            if (found != null)
            {
                cameraHolder = found;
            }
        }

        if (groundMask == 0)
        {
            groundMask = Physics.DefaultRaycastLayers;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up, mouseX);

        if (cameraHolder != null)
        {
            pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);
            cameraHolder.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            jumpQueued = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        Vector3 velocity = rb.velocity;
        velocity.x = moveDirection.x * moveSpeed;
        velocity.z = moveDirection.z * moveSpeed;
        rb.velocity = velocity;

        if (jumpQueued)
        {
            jumpQueued = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
    }
}
