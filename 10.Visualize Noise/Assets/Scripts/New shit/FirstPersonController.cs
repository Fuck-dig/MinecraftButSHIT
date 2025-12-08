using UnityEngine;
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float walkSpeed = 2f; // Slow walk speed when holding Shift
    public float jumpForce = 8f;
    public float gravity = -20f;
    
    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    
    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isGrounded;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Make sure CharacterController exists
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }
    
    void Update()
    {
        // Check if grounded
        isGrounded = controller.isGrounded;
        
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
        
        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Slow-walk detection
        bool isSlowWalking = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float speed = isSlowWalking ? walkSpeed : moveSpeed;
        controller.Move(move * speed * Time.deltaTime);
        
        // If grounded and holding Shift, prevent falling by sticking to ground
        if (isGrounded)
        {
            if (isSlowWalking)
            {
                velocity.y = -2f; // small downward to keep grounded
            }
            else if (velocity.y < 0f)
            {
                velocity.y = -2f;
            }
        }
        
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
        }
        
        // Apply gravity only when not holding Shift while grounded
        if (!(isSlowWalking && isGrounded))
        {
            velocity.y += gravity * Time.deltaTime;
        }
        
        controller.Move(velocity * Time.deltaTime);
        
        // Unlock cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}