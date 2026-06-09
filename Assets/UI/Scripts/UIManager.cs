using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Target UI")]
    public TargetUI targetUI;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowTarget(CharacterStats stats)
    {
        targetUI.ShowTarget(stats);
    }

    public void ShowGenericInteractable(Interactable interactable)
    {
        //targetUI.ShowGenericInteractable(interactable);
    }

    public void HideTarget()
    {
        targetUI.HideTarget();
    }
}
