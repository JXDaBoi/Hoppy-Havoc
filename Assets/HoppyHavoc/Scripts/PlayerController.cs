using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float acceleration = 10.0f;
    public float rotationSpeed = 2.0f;
    public float jumpForce = 6.0f;
    public float gravity = -9.81f;
    public float maxStamina = 5.0f;
    public float staminaRecoveryRate = 1.0f;
    public float staminaDrainRate = 1.0f;

    public Transform rotationAxis;
    public Camera playerCamera;
    public Vector3 cameraOffset = new Vector3(0, 5, -15);
    public float cameraSmoothSpeed = 0.125f;
    public Vector3 backwardCameraOffset = new Vector3(0, 5, 15);

    // New public transform variable for the player model
    public Transform playermodel__;
    public float AD_rotate_amount = 10f;

    private float currentStamina;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 inputDirection;
    [SerializeField]
    private bool isGrounded;
    private bool isFacingCamera = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;
    }

    void Update()
    {
       
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * mouseX * rotationSpeed * Time.deltaTime);

        Vector3 cameraRotation = playerCamera.transform.eulerAngles;
        cameraRotation.x -= mouseY;
        playerCamera.transform.eulerAngles = cameraRotation;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        ProcessMovement();

        if (isGrounded && Input.GetButton("Jump"))
        {
            JumpAnimation();
        }

        UpdateCameraPosition();


        // below were in update
        if (Cursor.lockState != CursorLockMode.Locked) return;

        HandleMovementInput();
        if (!isFacingCamera)
        {
            HandleCameraRotation(); // Handle free camera rotation when not facing the player
        }

        RotatePlayerModel(); // Rotate the playermodel__ based on velocity heading
    }

    private void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        inputDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (verticalInput < 0) // Pressing S to face the camera
        {
            RotateCharacterToFaceCamera();
            isFacingCamera = true; // The player is now facing the camera
        }
        else if (verticalInput > 0 && isFacingCamera) // Pressing W after facing the camera
        {
            RotateCameraBehindCharacter();
            isFacingCamera = false;
        }
        else if (horizontalInput != 0) // Handle A and D for 90-degree horizontal rotation
        {
            RotateCharacterSideways(horizontalInput);
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }

    private void ProcessMovement()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
        float targetSpeed = isSprinting ? runSpeed : walkSpeed;

        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        Vector3 targetVelocity = inputDirection * targetSpeed;
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity.y = rb.velocity.y;
        rb.velocity = Vector3.MoveTowards(rb.velocity, targetVelocity, acceleration * Time.deltaTime);

        animator.SetFloat("Velocity", new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude / runSpeed);
    }

    // Rotate the character smoothly to face the camera
    private void RotateCharacterToFaceCamera()
    {
        Vector3 cameraDirection = playerCamera.transform.position - transform.position;
        cameraDirection.y = 0; // Ignore vertical differences

        Quaternion targetRotation = Quaternion.LookRotation(cameraDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Rotate the character 90 degrees left or right when pressing A or D
    private void RotateCharacterSideways(float horizontalInput)
    {
        float targetAngle = horizontalInput > 0 ? AD_rotate_amount : -AD_rotate_amount; // Target 90 degrees based on input
        Vector3 targetDirection = Quaternion.Euler(0, targetAngle, 0) * transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Move the camera back behind the character when pressing W after facing the camera
    private void RotateCameraBehindCharacter()
    {
        Vector3 newCameraPosition = transform.position + transform.TransformDirection(cameraOffset);
        Vector3 smoothedPosition = Vector3.Lerp(playerCamera.transform.position, newCameraPosition, cameraSmoothSpeed);
        playerCamera.transform.position = smoothedPosition;

        playerCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);
    }

    // Update the camera position, switching between forward and backward offsets
    private void UpdateCameraPosition()
    {
        Vector3 desiredPosition = isFacingCamera
            ? transform.position + backwardCameraOffset // Use backward offset when facing the camera
            : transform.position + transform.TransformDirection(cameraOffset); // Use normal offset otherwise

        Vector3 smoothedPosition = Vector3.Lerp(playerCamera.transform.position, desiredPosition, cameraSmoothSpeed);
        playerCamera.transform.position = smoothedPosition;

        playerCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);
    }

    private void JumpAnimation()
    {
        if (inputDirection.magnitude < 0.2f)
        {
            animator.SetTrigger("Jump");
        }
        else
        {
            animator.SetTrigger("RunningJump");
        }
        animator.ResetTrigger("Landing");
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetTrigger("Landing");
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("RunningJump");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // New method to rotate playermodel__ based on the script attachee's velocity direction
    private void RotatePlayerModel()
    {
        Vector3 movementDirection = rb.velocity;
        movementDirection.y = 0; // Ignore the Y-axis for horizontal rotation

        if (movementDirection.magnitude > 0.1f) // Only rotate if there's significant movement
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            playermodel__.rotation = Quaternion.Slerp(playermodel__.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
