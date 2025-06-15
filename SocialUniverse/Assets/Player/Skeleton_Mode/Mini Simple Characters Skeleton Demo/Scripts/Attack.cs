using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject sword_wood;
    public BoxCollider boxCollider; 

    public void EnableDamage()
    {
        boxCollider.enabled = true; // Ativa o BoxCollider para permitir dano
    }

    public void DisableDamage()
    {
        boxCollider.enabled = false; // Desativa o BoxCollider para não permitir dano
    }
}
