using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class FPSInputActions : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float crouchSpeed = 2.5f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;

    [Header("Look")]
    public Transform cameraTransform;
    public float lookSensitivity = 0.12f; // mouse delta multiplier (tune to taste)
    public float minPitch = -89f;
    public float maxPitch = 89f;

    [Header("Crouch")]
    public float standingHeight = 1.8f;
    public float crouchingHeight = 1.1f;   // controller height
    public float standingEye = 1.6f;       // camera local y when standing
    public float crouchingEye = 1.0f;      // camera local y when crouched
    public float eyeLerpSpeed = 12f;       // smooth camera height

    public PlayerInputActions InputActions => inputActions;

    private PlayerInputActions inputActions;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput; // mouse delta
    private Vector3 velocity;
    private float pitch = 0f;
    private bool isCrouching = false;
    private float targetEyeY;

    void Awake()
    {
        inputActions = new PlayerInputActions();

        if (cameraTransform == null)
            cameraTransform = Camera.main ? Camera.main.transform : null;

        controller = GetComponent<CharacterController>();

        // Initialize controller capsule
        controller.height = standingHeight;
        controller.center = new Vector3(0f, controller.height * 0.5f, 0f);

        // Initialize camera eye height target
        targetEyeY = standingEye;
        if (cameraTransform != null)
        {
            var lp = cameraTransform.localPosition;
            cameraTransform.localPosition = new Vector3(lp.x, targetEyeY, lp.z);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        inputActions.Enable();

        // Movement
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;

        // Look (mouse delta / right stick)
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += _ => lookInput = Vector2.zero;

        // Jump
        inputActions.Player.Jump.performed += _ => TryJump();

        // Crouch (hold-to-crouch style; switch to .performed for toggle)
        inputActions.Player.Crouch.started += _ => StartCrouch();
        inputActions.Player.Crouch.canceled += _ => StopCrouch();
    }

    void OnDisable()
    {
        // Unsubscribe (avoids duplicate handlers on domain reloads)
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= _ => moveInput = Vector2.zero;
        inputActions.Player.Look.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled -= _ => lookInput = Vector2.zero;
        inputActions.Player.Jump.performed -= _ => TryJump();
        inputActions.Player.Crouch.started -= _ => StartCrouch();
        inputActions.Player.Crouch.canceled -= _ => StopCrouch();

        inputActions.Disable();
    }

    void Update()
    {
        // ----- Look -----
        // Input System's mouse delta is "per-frame" units, so no deltaTime here.
        float mouseX = lookInput.x * (lookSensitivity * 100f);
        float mouseY = lookInput.y * (lookSensitivity * 100f);

        transform.Rotate(Vector3.up * mouseX);
        pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);
        if (cameraTransform) cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);

        // clear look delta each frame so deltas don't stack
        lookInput = Vector2.zero;

        // ----- Grounding -----
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f; // stick to ground

        // ----- Move -----
        Vector3 wish = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        float moveSpeed = isCrouching ? crouchSpeed : speed;
        controller.Move(wish * moveSpeed * Time.deltaTime);

        // ----- Gravity -----
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ----- Smooth camera eye height -----
        if (cameraTransform != null)
        {
            var lp = cameraTransform.localPosition;
            float y = Mathf.Lerp(lp.y, targetEyeY, Time.deltaTime * eyeLerpSpeed);
            cameraTransform.localPosition = new Vector3(lp.x, y, lp.z);
        }
    }

    void TryJump()
    {
        if (controller.isGrounded && !isCrouching)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void StartCrouch()
    {
        isCrouching = true;
        controller.height = crouchingHeight;
        controller.center = new Vector3(0f, controller.height * 0.5f, 0f);
        targetEyeY = crouchingEye;
    }

    void StopCrouch()
    {
        isCrouching = false;
        controller.height = standingHeight;
        controller.center = new Vector3(0f, controller.height * 0.5f, 0f);
        targetEyeY = standingEye;
    }

 
 
}
