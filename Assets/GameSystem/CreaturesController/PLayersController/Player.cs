using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Creatures
{  
  public CharacterController controller;
    public float speed = 12f;
    public float runMultiplier = 2f;
    public float gravity = -11f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    
    public Animator animator;

    public float crouchSpeed = 6f;
    public float normalHeight = 1.9f;
    public float crouchHeight = 1f;
    private bool isCrouching;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            currentSpeed *= runMultiplier;
        }

        if (isCrouching)
        {
            currentSpeed /= 2f; // Halve the speed when crouching
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        bool isWalking = x != 0 || z != 0;
        bool isRunning = isWalking && Input.GetKey(KeyCode.LeftShift);
        bool isJump = Input.GetKey(KeyCode.Space);

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isCrouch", isCrouching);
        animator.SetBool("isJump", isJump);

        HandleCrouch();
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }

        float targetHeight = isCrouching ? crouchHeight : normalHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSpeed);

        // Adjust position to avoid sinking into the ground when standing up
        if (!isCrouching && controller.height < normalHeight)
        {
            controller.Move(Vector3.up * Time.deltaTime);
        }
    }
}
