using System;
using UnityEngine;

[System.Serializable]
public class CoreStat
{
    public event Action OnValueChanged;
    [SerializeField] private float current;
    [SerializeField] private float max = 100f;

    public float Current => current;
    public float Max => max;
    public float Normalized => Max > 0 ? current / Max : 0f;

    public void Set(float value)
    {
        float clamped = Mathf.Clamp(value, 0, max);
        if (Math.Abs(clamped - current) > Mathf.Epsilon)
        {
            current = clamped;
            OnValueChanged?.Invoke();
        }
    }

    public void Modify(float delta)
    {
        Set(current + delta);
    }
}
