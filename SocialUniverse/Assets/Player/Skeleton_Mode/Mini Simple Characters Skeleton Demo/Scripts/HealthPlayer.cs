using UnityEngine;
using UnityEngine.UI;

public class HealthPlayer : MonoBehaviour
{
    public Slider HealthSlider;
    public float Health;
    public float MaxHealth = 100;
    public Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void TakeDamage(float damage)
    {
        Health -= damage;
        Debug.Log(Health);
    }
    void Start()
    {
        Health = MaxHealth;
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (HealthSlider.value != Health)
        {
            HealthSlider.value = Health;
        }

        if (Health <= 0)
        {
            
            anim.SetBool("DEATH", true); // Ativa animação de morte
            Destroy(gameObject, 3f); // Destroi o objeto após 3 segundos
        }
    }
}
