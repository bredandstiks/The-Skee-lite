using UnityEngine;
using UnityEngine.InputSystem;

public class globalInputManager : MonoBehaviour
{
    // Static instance so any script can access it easily
    public static globalInputManager Instance { get; private set; }

    // The generated input actions asset
    private SkeeliteInputActions inputActions;

    // Public accessors so other scripts can subscribe to specific maps
    public SkeeliteInputActions.PlayerActions Player => inputActions.Player;
    public SkeeliteInputActions.SkeeballActions Skeeball => inputActions.Skeeball;
    public SkeeliteInputActions.GlobalActions Global => inputActions.Global;

    void Awake()
    {
        // Singleton pattern - only one globalInputManager should exist
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene loads

        inputActions = new SkeeliteInputActions();
    }

    void OnEnable()
    {
        // Enable the Global map here since it should always be active
        inputActions.Global.Enable();

        // Subscribe to the Pause action directly here
        inputActions.Global.Pause.performed += OnPause;
    }

    void OnDisable()
    {
        inputActions.Global.Pause.performed -= OnPause;
        inputActions.Global.Disable();
    }

    void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        Debug.Log("Pause pressed!");
        // Add your pause logic here, e.g.:
        // PauseMenu.Instance.TogglePause();
    }

    // Call these from other managers to activate/deactivate input maps
    public void EnablePlayerControls()
    {
        inputActions.Player.Enable();
        inputActions.Skeeball.Disable();
    }

    public void EnableSkeeballControls()
    {
        inputActions.Skeeball.Enable();
        inputActions.Player.Disable();
    }

    public void DisableAllGameplayControls()
    {
        inputActions.Player.Disable();
        inputActions.Skeeball.Disable();
    }
}