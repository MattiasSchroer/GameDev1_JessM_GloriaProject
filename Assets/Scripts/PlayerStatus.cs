using System;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    public Health Health;

    [SerializeField] private float DamageFlickerTime = 0.3f;
    private float DamageTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private bool Invincibility = false;

    public static event Action<Health> OnPlayerHealthChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = new Health(maxHealth);
        Judge.OnValidateInput += TakeDamageOnFailedInput;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Choreographer.OnSwitchToDemoState += ResetInvincibility;
    }

    // Update is called once per frame
    void Update()
    {
        if (DamageTimer > 0f)
        {
            DamageTimer -= Time.deltaTime;
            if (DamageTimer <= 0f) 
            {
                ToggleDamageVisual(false);
            }
        }
    }

    private void TakeDamageOnFailedInput(bool success) 
    {
        if (success) return;
        if (Invincibility) return;

        Invincibility = true;

        Health.TakeDamage(1);
        OnPlayerHealthChange?.Invoke(Health);

        DamageTimer = DamageFlickerTime;
        ToggleDamageVisual(true);
    }

    private void ToggleDamageVisual(bool toggle) 
    {
        if (toggle) spriteRenderer.color = Color.red;
        else spriteRenderer.color = Color.white;
    }

    private void ResetInvincibility() 
    {
        Invincibility = false;
    }
}
