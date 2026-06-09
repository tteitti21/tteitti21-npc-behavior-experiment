using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundMask;
    public LayerMask interactableMask;

    private NavMeshAgent agent;
    private Interactable currentInteractable;
    /* Will not trigger new interaction when within distance until reclicked.*/
    private bool interactionPending = false;

    // Input actions
    private InputAction clickAction;
    private InputAction pointerAction;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // One action for "click" or "tap"
        clickAction = new InputAction(
            type: InputActionType.Button,
            binding: "<Mouse>/leftButton"
        );
        // Add a touchscreen binding
        clickAction.AddBinding("<Touchscreen>/primaryTouch/tap");

        // One action for pointer position (mouse cursor or finger)
        pointerAction = new InputAction(
            type: InputActionType.Value,
            binding: "<Pointer>/position"
        );
    }

    void OnEnable()
    {
        clickAction.Enable();
        pointerAction.Enable();
        clickAction.performed += OnClick;
    }

    void OnDisable()
    {
        clickAction.performed -= OnClick;
        clickAction.Disable();
        pointerAction.Disable();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = pointerAction.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        // 1. Check if clicked interactable
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableMask))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                SetInteractTarget(interactable);
                return;
            }
        }

        // 2. Otherwise, move to ground
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            agent.SetDestination(hit.point);
            // clear pending interaction availabilty and hide target UI
            interactionPending = false;
            ClearInteractTarget();
        }
    }

    void Update()
    {
        if (currentInteractable != null)
        {
            // Check distance (default to 2 units if not defined)
            float interactionDistance = currentInteractable.interactionRadius > 0 ? currentInteractable.interactionRadius : 2f;
            float distance = Vector3.Distance(transform.position, currentInteractable.transform.position);

            if (distance <= interactionDistance && interactionPending)
            {
                agent.ResetPath();
                // Trigger interaction if defined
                currentInteractable.OnInteract?.Invoke();
                interactionPending = false;
            }
        }
    }

    private void SetInteractTarget(Interactable interactable)
    {
        if (currentInteractable == interactable)
        {
            interactionPending = true;
            agent.SetDestination(interactable.GetInteractionPoint());
            return;
        }
        currentInteractable = interactable;
        interactable.ShowUI();
    }

    private void ClearInteractTarget()
    {
        if (currentInteractable != null)
        {
            currentInteractable.HideUI();
            currentInteractable = null;
        }
    }
}
