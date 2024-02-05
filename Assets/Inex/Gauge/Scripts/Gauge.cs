using UnityEngine;
using UnityEngine.Events;

public class Gauge : MonoBehaviour
{
    public UnityEvent<float> OnValueChange;

    public float value { get; private set; }

    private void OnTriggerEnter2D(Collider2D col)
    {
        value = float.Parse(col.name);
        OnValueChange?.Invoke(value);
    }
}