using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ThirdPersonController : MonoBehaviour

{
    public Animator anim;
    public CharacterController controller;
    public Transform groundCheck;
    public Transform ThirdPersonCamera;
    public LayerMask groundMask;
  
    public GameObject cam1;
    public GameObject cam2;
    public GameObject PauseUI;

    public float speed = 5f;
    public float jump = 8f;
    public float turnsmoothTime = 0.1f;
    public float gravity = -9.8f;
    public float groundDistance = 0.2f;

    float turnsmoothVelocity;
    Vector3 velocity;
    public bool isGrounded;


    void Start()
    {

        cam1.SetActive(true);
        cam2.SetActive(false);

        Time.timeScale = 1;
    }

    void Update()
    {
        if (Time.timeScale == 1f)
        {
            Grounded();
            //Jump();
            Move();
            PauseGame();
        }

    }

    void Grounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + ThirdPersonCamera.eulerAngles.y;
            float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnsmoothVelocity, turnsmoothTime);
            transform.rotation = Quaternion.Euler(0f, Angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir * speed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        this.anim.SetFloat("vertical", vertical);
        this.anim.SetFloat("horizontal", horizontal);
    }

    //void Jump()
    //{
    //    if (Input.GetButtonDown("Jump") && isGrounded)
    //    {
    //        velocity.y = Mathf.Sqrt(jump * -0.2f * gravity);
    //        //this.anim.SetBool("Jump", true);
    //    }
    //    //else
    //    //{
    //    //    this.anim.SetBool("Jump", false);
    //    //}
    //}

    void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseUI.activeInHierarchy == false)
            {
                Time.timeScale = 0f;
                PauseUI.SetActive(true);
            }

            else if (PauseUI.activeInHierarchy == true)
            {
                Time.timeScale = 1f;
                PauseUI.SetActive(false);
            }
        }
    }


    
}
