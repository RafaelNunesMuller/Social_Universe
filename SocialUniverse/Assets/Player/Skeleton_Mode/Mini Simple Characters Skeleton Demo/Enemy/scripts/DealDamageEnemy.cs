using UnityEngine;

public class DealDamageEnemy : MonoBehaviour
{
     public float damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public BoxCollider weaponCollider;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthPlayer Player = other.GetComponent<HealthPlayer>();
            Player.TakeDamage(damage);
        }
    }
    void Start()
    {
        OnTriggerEnter(null);
        weaponCollider = GetComponent<BoxCollider>();

        weaponCollider.enabled = false;
    }

    void Update()
    {
        
    }


    public void EnableWeaponCollider()
    {

        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }
}
