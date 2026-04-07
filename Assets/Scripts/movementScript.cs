using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class movementScript : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float Speed = 6.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;

    public float jumpHeight = 6f;

    float velocityY;
    bool isGrounded;

    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;

    // Input
    Vector2 moveInput;
    Vector2 lookInput;
    bool jumpPressed;
    bool isLoading = false;
    [SerializeField] levelLoaderScript levelLoader;

    void Start()
    {
        globalInputManager.Instance.DisableAllGameplayControls();

        controller = GetComponent<CharacterController>();

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        globalInputManager.Instance.EnablePlayerControls();
    }

    void OnEnable()
    {
        var instance = globalInputManager.Instance;
        if (instance == null)
        {
            Debug.LogWarning("globalInputManager.Instance is null in OnEnable - skipping input hookup.");
            return;
        }

        var input = instance.Player;

        input.Move.performed += OnMove;
        input.Move.canceled += OnMove;

        input.Look.performed += OnLook;
        input.Look.canceled += OnLook;

        input.Jump.performed += OnJump;

        input.Interact.performed += OnInteract;

        Debug.Log(instance);
    }

    void OnDisable()
    {
        var instance = globalInputManager.Instance;
        if (instance == null) return;

        var input = instance.Player;

        input.Move.performed -= OnMove;
        input.Move.canceled -= OnMove;

        input.Look.performed -= OnLook;
        input.Look.canceled -= OnLook;

        input.Jump.performed -= OnJump;

        input.Interact.performed -= OnInteract;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        jumpPressed = true;
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (isLoading) return;

        isLoading = true;
        Debug.Log("INTERACT PRESSED, LOADING NEXT LEVEL...");
        if (levelLoader == null)
        {
            levelLoader = FindObjectOfType<levelLoaderScript>();
        }

        if (levelLoader != null)
        {
            levelLoader.LoadLevelIndex(1);
        }
        else
        {
            Debug.LogError("levelLoaderScript not assigned and none found in scene.");
            isLoading = false;
        }
    }

    void Update()
    {
        UpdateMouse();
        UpdateMove();
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = lookInput;

        currentMouseDelta = Vector2.SmoothDamp(
            currentMouseDelta,
            targetMouseDelta,
            ref currentMouseDeltaVelocity,
            mouseSmoothTime
        );

        cameraCap -= currentMouseDelta.y * mouseSensitivity;
        cameraCap = Mathf.Clamp(cameraCap, -90f, 90f);

        playerCamera.localEulerAngles = Vector3.right * cameraCap;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        // Fix ground sticking
        if (isGrounded && velocityY < 0)
        {
            velocityY = -2f;
        }

        Vector2 targetDir = moveInput.normalized;

        currentDir = Vector2.SmoothDamp(
            currentDir,
            targetDir,
            ref currentDirVelocity,
            moveSmoothTime
        );

        velocityY += gravity * Time.deltaTime;

        Vector3 velocity =
            (transform.forward * currentDir.y +
             transform.right * currentDir.x) * Speed
            + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && jumpPressed)
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        jumpPressed = false;
    }
}