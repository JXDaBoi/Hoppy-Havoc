using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float acceleration = 20.0f; // Acceleration rate
    public float maxSpeed = 20.0f; // Maximum speed
    public float rotationSpeed = 150.0f; // Rotation speed (reduced for smoother rotation)
    public float jumpForce = 6.0f; // Jump force
    private Rigidbody rb; // Rigidbody component
    private Animator animator; // Animator component
    private Vector3 currentVelocity; // Current velocity
    [SerializeField]
    private bool isGrounded; // Ground check flag
    public Transform CameraRotateAxis;
    public GameObject CharacterModel;

    public int trajectoryResolution = 30; // Number of points in the trajectory
    public float trajectoryTimeStep = 0.1f; // Time step for each point in the trajectory
    public LineRenderer lineRenderer; // LineRenderer component to display the path

    private Vector3 jumpDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ShowTrajectory(Vector3 jumpDirection)
    {
        Vector3 initialPosition = rb.position;
        Vector3 gravity = Physics.gravity;

        // Prepare the line renderer to show the trajectory
        lineRenderer.positionCount = trajectoryResolution;

        for (int i = 0; i < trajectoryResolution; i++)
        {
            // Calculate the position at each step using physics equations
            float time = i * trajectoryTimeStep;
            Vector3 displacement = jumpDirection * time + 0.5f * gravity * time * time;
            Vector3 trajectoryPosition = initialPosition + displacement;

            // Set the calculated point in the line renderer
            lineRenderer.SetPosition(i, trajectoryPosition);
        }
    }

    void FixedUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Rotate the camera based on mouse input
            transform.Rotate(Vector3.up * mouseX * rotationSpeed * Time.deltaTime);
            CameraRotateAxis.Rotate(Vector3.left * mouseY * rotationSpeed * Time.deltaTime);

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 targetVelocity = new Vector3(horizontalInput, 0, verticalInput).normalized * maxSpeed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            Vector3 movement = transform.TransformDirection(currentVelocity) * Time.deltaTime;

            if (currentVelocity != Vector3.zero)
            {
                // Calculate movement direction and smoothly rotate character model towards it
                Vector3 movementDirection = transform.forward * verticalInput + transform.right * horizontalInput;
                if (movementDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                    CharacterModel.transform.rotation = Quaternion.Slerp(CharacterModel.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 0.05f);
                }
            }

            rb.MovePosition(rb.position + movement);
            animator.SetFloat("Velocity", currentVelocity.magnitude / maxSpeed);

            // Show the trajectory path if grounded
            if (isGrounded)
            {
                // Calculate the jump direction based on current forward movement
                jumpDirection = CharacterModel.transform.forward * maxSpeed + Vector3.up * jumpForce;

                ShowTrajectory(jumpDirection); // Pass jump direction to trajectory calculation

                if (Input.GetButton("Jump"))
                {
                    if (currentVelocity.magnitude < 0.05f)
                    {
                        animator.SetTrigger("Jump");
                        animator.ResetTrigger("Landing");
                    }
                    else
                    {
                        animator.SetTrigger("RunningJump");
                        animator.ResetTrigger("Landing");
                    }

                }
            }
        }


    }

    public void Jumping()
    {
        rb.AddForce(jumpDirection, ForceMode.Impulse);
        isGrounded = false;
        lineRenderer.positionCount = 0; // Clear trajectory once jumping
    }

    private void Update()
    {
        KeepCharacterUpright();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetTrigger("Landing");
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("RunningJump");
        }
    }

    void KeepCharacterUpright()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}