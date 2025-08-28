using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    private static readonly int hashDie = Animator.StringToHash("Die");

    public Slider healthSlider;

    public AudioClip deathClip;
    public AudioClip hitClip;
    public AudioClip itemPickupClip;

    private AudioSource audioSource;
    private Animator animator;

    private PlayerMovement movement;
    private PlayerShooter shooter;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        shooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        healthSlider.gameObject.SetActive(true);
        healthSlider.value = Health / MaxHealth;

        movement.enabled = true;
        shooter.enabled = true;
    }

    private void Update()
    {
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead)
            return;

        base.OnDamage(damage, hitPoint, hitNormal);
        healthSlider.value = Health / MaxHealth;
        audioSource.PlayOneShot(hitClip);
    }

    protected override void Die()
    {
        base.Die();

        healthSlider.gameObject.SetActive(false);
        animator.SetTrigger(hashDie);
        audioSource.PlayOneShot(deathClip);

        movement.enabled = false;
        shooter.enabled = false;
    }

    public void Heal(float amount)
    {
        Health = Mathf.Max(MaxHealth, Health + amount);
        healthSlider.value = Health / MaxHealth;
    }
}
