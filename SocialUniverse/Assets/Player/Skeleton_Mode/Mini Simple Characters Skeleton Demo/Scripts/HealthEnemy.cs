using UnityEngine;
using UnityEngine.UI;

public class HealthEnemy : MonoBehaviour
{
    public Slider HealthSlider;
    public float Health;
    public float MaxHealth = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void TakeDamage(float damage)
    {
        Health -= damage;
        Debug.Log(Health);
    }
    void Start()
    {
        Health = MaxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (HealthSlider.value != Health)
        {
            HealthSlider.value = Health;
        }

    }
}
