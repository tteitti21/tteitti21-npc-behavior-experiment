using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetUI : MonoBehaviour
{
    [Header("Target Panel")]
    [SerializeField] private GameObject targetPanel;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider tirednessBar;
    [SerializeField] private Slider hungerBar;

    private CharacterStats targetStats;

    public void ShowTarget(CharacterStats stats)
    {
        if (targetStats != null)
        {
            // Unsubscribe previous
            targetStats.Health.OnValueChanged -= UpdateHealthUI;
            targetStats.Tiredness.OnValueChanged -= UpdateTirednessUI;
            targetStats.Hunger.OnValueChanged -= UpdateHungerUI;
        }

        targetStats = stats;

        if (targetStats != null)
        {
            targetStats.Health.OnValueChanged += UpdateHealthUI;
            targetStats.Tiredness.OnValueChanged += UpdateTirednessUI;
            targetStats.Hunger.OnValueChanged += UpdateHungerUI;

            targetPanel.SetActive(true);
            nameText.text = targetStats.CharacterName;

            UpdateHealthUI();
            UpdateTirednessUI();
            UpdateHungerUI();
        }
    }

    public void HideTarget()
    {
        if (targetStats != null)
        {
            targetStats.Health.OnValueChanged -= UpdateHealthUI;
            targetStats.Tiredness.OnValueChanged -= UpdateTirednessUI;
            targetStats.Hunger.OnValueChanged -= UpdateHungerUI;
        }

        targetPanel.SetActive(false);
        targetStats = null;
    }

    private void UpdateHealthUI()
    {
        healthBar.minValue = 0;
        healthBar.maxValue = targetStats.Health.Max;
        healthBar.value = targetStats.Health.Current;
    }

    private void UpdateTirednessUI()
    {
        tirednessBar.minValue = 0;
        tirednessBar.maxValue = targetStats.Tiredness.Max;
        tirednessBar.value = targetStats.Tiredness.Current;
    }

    private void UpdateHungerUI()
    {
        hungerBar.minValue = 0;
        hungerBar.maxValue = targetStats.Hunger.Max;
        hungerBar.value = targetStats.Hunger.Current;
    }
}
