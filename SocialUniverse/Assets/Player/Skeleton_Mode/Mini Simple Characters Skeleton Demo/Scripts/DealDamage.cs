using UnityEngine;

public class DealDamage : MonoBehaviour
{
     public float damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
