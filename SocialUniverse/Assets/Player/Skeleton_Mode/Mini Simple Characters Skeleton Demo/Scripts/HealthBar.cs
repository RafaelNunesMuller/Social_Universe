using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider HealthSlider;
    public float MaxHealth = 100;
    public float Health;

    void Start()
    {
        Health = MaxHealth;
    }

    void Update()
    {
        if (HealthSlider.value != Health)
        {
            HealthSlider.value = Health;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            TakeDamage(10);
        }

    }

    void TakeDamage(float damage)
    {
        Health -= damage;
    }

}
