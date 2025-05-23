using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider HealthSlider;
    public float MaxHealth = 100f;
    public float Health;
    private Animator anim;

    void Start()
    {
        Health = MaxHealth;

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (HealthSlider.value != Health)
        {
            HealthSlider.value = Health;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            takeDamage(10);
        }

        if (Health == 0)
        {
            anim.SetBool("DEATH", true);
        }

    }

    void takeDamage(float damage)
    {
        Health -= damage;
    }
}
