using UnityEngine;

public class HealthEnemy : MonoBehaviour
{
    public float Health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void TakeDamage(float damage)
    {
        Health -= damage;
        Debug.Log(Health);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
