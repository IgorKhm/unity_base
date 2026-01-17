using UnityEngine;
using UnityEngine.InputSystem;

public class AimWithMouse2D : MonoBehaviour
{
    public Camera cam;
    public Transform gunPivot;

    private PlayerInput playerInput;
    private InputAction aimAction;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        // aimAction = playerInput.currentActionMap.FindAction("Aim", false);
        aimAction = playerInput.actions["Aim"]; // must match action name exactly
   
    }

    void Update()
    {
        if (cam == null || gunPivot == null || aimAction == null) return;

        Vector2 screenPos = aimAction.ReadValue<Vector2>();
        
        // if (aimAction == null)
            // aimAction = playerInput.currentActionMap.FindAction("Aim", false);

        // Debug: uncomment for 1 second to confirm it changes when you move the mouse
        // Debug.Log(screenPos);

        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        Vector2 dir = (Vector2)(world - gunPivot.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        gunPivot.right = dir.normalized;
    }
}