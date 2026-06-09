using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;

    [Header("Player Stats Reference")]
    private PlayerSheet playerStats;

    [Header("Sliders")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider tirednessBar;
    [SerializeField] private Slider hungerBar;

    [Header("Text (Optional)")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text tirednessText;
    [SerializeField] private TMP_Text hungerText;

    void Start()
    {
        if (playerStats == null)
            playerStats = PlayerSheet.Instance;

        nameText.text = playerStats.CharacterName;

        // Subscribe to events
        playerStats.Health.OnValueChanged += UpdateHealthUI;
        playerStats.Tiredness.OnValueChanged += UpdateTirednessUI;
        playerStats.Hunger.OnValueChanged += UpdateHungerUI;

        // Initial update
        UpdateHealthUI();
        UpdateTirednessUI();
        UpdateHungerUI();
    }

    private void UpdateHealthUI()
    {
        healthBar.minValue = 0;
        healthBar.maxValue = playerStats.Health.Max;
        healthBar.value = playerStats.Health.Current;
        if (healthText != null)
            healthText.text = $"{playerStats.Health.Current} / {playerStats.Health.Max}";
    }

    private void UpdateTirednessUI()
    {
        tirednessBar.minValue = 0;
        tirednessBar.maxValue = playerStats.Tiredness.Max;
        tirednessBar.value = playerStats.Tiredness.Current;
        if (tirednessText != null)
            tirednessText.text = $"{playerStats.Tiredness.Current} / {playerStats.Tiredness.Max}";
    }

    private void UpdateHungerUI()
    {
        hungerBar.minValue = 0;
        hungerBar.maxValue = playerStats.Hunger.Max;
        hungerBar.value = playerStats.Hunger.Current;
        if (hungerText != null)
            hungerText.text = $"{playerStats.Hunger.Current} / {playerStats.Hunger.Max}";
    }
}
