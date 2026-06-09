using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    [Tooltip("How close the player must be to interact")]
    public float interactionRadius = 2f;

    // Optional reference to stats (null if not a character)
    private CharacterStats stats;

    // Action that gets invoked when interaction happens
    public Action OnInteract;


    void Awake()
    {
        stats = GetComponent<CharacterStats>();

        OnInteract ??= DefaultInteract;
    }

    public CharacterStats GetStats() => stats;

    // Called by click-to-move or player interaction
    public void ShowUI()
    {
        if (stats != null)
        {
            UIManager.Instance.ShowTarget(stats); // character
        }
        else
        {
            UIManager.Instance.ShowGenericInteractable(this); // item/location
        }
    }

    public Vector3 GetInteractionPoint()
    {
        return transform.position; // can customize per interactable
    }

    // Default fallback interaction
    private void DefaultInteract()
    {
        if (stats != null)
        {
            Debug.Log($"Interacting with {stats.CharacterName}");
        }
        else
        {
            Debug.Log($"Interacting with {gameObject.name}");
        }
    }

    public void HideUI()
    {
        UIManager.Instance.HideTarget();
    }
}
