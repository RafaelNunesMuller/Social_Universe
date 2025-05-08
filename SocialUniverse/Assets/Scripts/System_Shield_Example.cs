using UnityEngine;

public class System_Shield_Example : MonoBehaviour
{
    public float parryWindow = 0.5f; // Tempo em segundos para o parry
    public float damageReduction = 0.5f; // Redução de dano se o parry falhar
    public float shieldCooldown = 1.0f; // Tempo de cooldown para reativar o escudo
    private bool isShieldActive = false;
    private float shieldActivationTime;
    private float shieldCooldownTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= shieldCooldownTime) // Ativa o escudo ao pressionar a tecla Espaço
        {
            ActivateShield();
        }

        if (isShieldActive && Time.time - shieldActivationTime > parryWindow)
        {
            // Se o tempo de parry passou, desativa o parry
            isShieldActive = false;
        }
    }

    void ActivateShield()
    {
        isShieldActive = true;
        shieldActivationTime = Time.time;
        shieldCooldownTime = Time.time + shieldCooldown; // Define o tempo de cooldown
    }

    public void OnHit()
    {
        if (isShieldActive && Time.time - shieldActivationTime <= parryWindow)
        {
            // Parry bem-sucedido
            Debug.Log("Parry bem-sucedido!");
        }
        else
        {
            // Parry falhou, dano reduzido
            Debug.Log("Parry falhou, dano reduzido pela metade.");
            // Aqui você pode aplicar a lógica de redução de dano
        }
    }
}
