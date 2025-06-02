using UnityEngine;

public class Attack : MonoBehaviour
{
    public Animator anim;
    public Collider weaponCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.GetComponent<Animator>();
        weaponCollider = GetComponent<Collider>();
        weaponCollider.enabled = false;
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
