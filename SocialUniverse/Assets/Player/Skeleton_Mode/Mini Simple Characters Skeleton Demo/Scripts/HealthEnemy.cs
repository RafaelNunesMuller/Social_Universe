using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;   
public class HealthEnemy : MonoBehaviour
{
    public Slider HealthSlider;
    public float Health;
    public float MaxHealth = 100;
    public Animator anim;

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
            anim.SetBool("STAYDEAD", true); // Ativa animação de morte
            anim.ResetTrigger("isAttacking"); // Reseta o ataque
            var agent = GetComponent<NavMeshAgent>();
            agent.enabled = false; // Desativa o movimento
            Destroy(gameObject, 3f); // Destroi o objeto após 3 segundos
        }
    }
    
}
