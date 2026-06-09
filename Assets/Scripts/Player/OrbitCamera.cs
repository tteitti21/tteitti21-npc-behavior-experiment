using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;          // player
    public float distance = 15f;      // distance from player
    public float followSpeed = 5f;    // smooth follow

    [Header("Rotation")]
    private float rotationSpeed = 180f; // degrees per second
    private float currentYaw = 45f;    // default yaw
    private float currentPitch = 30f;  // default pitch

    [Header("Pitch Limits")]
    private float minPitch = 5f;
    private float maxPitch = 70f;
    [Header("Zoom")]
    private float minDistance = 5f;
    private float maxDistance = 25f;
    private float zoomSpeed = 150f;

    private InputAction scrollAction;

    // Input actions
    private InputAction lookAction;
    private InputAction rightMouseAction;

    void Awake()
    {
        // Dragging (mouse delta or touch delta)
        lookAction = new InputAction(type: InputActionType.Value, binding: "<Pointer>/delta");

        // Right mouse held down
        rightMouseAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/rightButton");
        rightMouseAction.AddBinding("<Touchscreen>/primaryTouch/press");

        // Scroll wheel (PC)
        scrollAction = new InputAction(type: InputActionType.Value, binding: "<Mouse>/scroll");

        // Pinch (mobile) – requires Unity InputSystem multitouch enabled
        //scrollAction.AddBinding("<Touchscreen>/pinch");
    }

    void OnEnable()
    {
        lookAction.Enable();
        rightMouseAction.Enable();
        scrollAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();
        rightMouseAction.Disable();
        scrollAction.Disable();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Only rotate if right mouse or touch is held
        if (rightMouseAction.IsPressed())
        {
            Vector2 delta = lookAction.ReadValue<Vector2>();
            currentYaw += delta.x * rotationSpeed * Time.deltaTime * 0.1f;
            currentPitch -= delta.y * rotationSpeed * Time.deltaTime * 0.1f;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }

        // Zoom input
        Vector2 scroll = scrollAction.ReadValue<Vector2>();
        float zoomDelta = -scroll.y * zoomSpeed * Time.deltaTime; // scroll.y > 0 = zoom in
        distance = Mathf.Clamp(distance + zoomDelta, minDistance, maxDistance);

        // Convert yaw/pitch to offset
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        //offset.y += height;

        // Smooth follow
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // Always look at player
        transform.LookAt(target.position + Vector3.up * 1.5f); // look at chest/head height
    }
}
