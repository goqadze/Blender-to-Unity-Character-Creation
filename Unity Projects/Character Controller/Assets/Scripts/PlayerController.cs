using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float gravity = -12;
    public float jumpHeight = 1;
    [Range(0, 1)]
    public float airControlPercent;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    Animator animator;
    Transform cameraTransform;

    CharacterController contoller;

    void Start()
    {
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        contoller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;


        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        bool running = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = (running ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        velocityY += Time.deltaTime * gravity;
        // transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        contoller.Move(velocity * Time.deltaTime);

        if (contoller.isGrounded)
        {
            velocityY = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        currentSpeed = new Vector2(contoller.velocity.x, contoller.velocity.z).magnitude;

        // animation
        float animationSpeedPercent = running ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    void Jump()
    {
        // Debug.Log("Jump");
        if (contoller.isGrounded)
        {
            // Debug.Log("Grounded");
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
        }
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (contoller.transform.position.y <= .69f)
        {
            return smoothTime;
        }

        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }

        return smoothTime / airControlPercent;
    }
}
