using UnityEngine;

public class DealDamage : MonoBehaviour
{
     public float damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator anim;
    public BoxCollider weaponCollider;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthEnemy enemy = other.GetComponent<HealthEnemy>();
            enemy.TakeDamage(damage);
        }
    }
    void Start()
    {
        OnTriggerEnter(null);
        anim = GetComponent<Animator>();
        weaponCollider = GetComponent<BoxCollider>();

        weaponCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") )
        {
            anim.SetBool("isAttacking", true);
        }
    }


    
}
