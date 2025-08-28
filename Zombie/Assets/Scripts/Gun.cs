using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public UiManager uiManager;

    public enum State
    {
        Ready,
        Empty,
        Reloading,
    }

    private State currentState = State.Ready;

    public State CurrentState
    {
        get { return currentState; }
        private set
        {
            currentState = value;
            switch (currentState)
            {
                case State.Ready:
                    break;
                case State.Empty:
                    break;
                case State.Reloading:
                    break;
            }
        }
    }

    public GunData gundata;

    public ParticleSystem muzzleEffect;
    public ParticleSystem shellEffect;

    private LineRenderer lineRenderer;
    private AudioSource audioSource;

    public Transform firePosition;

    public int ammoRemain;
    public int magAmmo;

    private float lastFireTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }

    private void OnEnable()
    {
        ammoRemain = gundata.startAmmoRemain;
        magAmmo = gundata.magCapacity;
        lastFireTime = 0f;

        CurrentState = State.Ready;

        uiManager.SetAmmoText(magAmmo, ammoRemain);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Ready:
                UpdateReady();
                break;
            case State.Empty:
                UpdateEmpty();
                break;
            case State.Reloading:
                UpdateReloading();
                break;
        }
    }

    private void UpdateReady()
    {

    }

    private void UpdateEmpty()
    {

    }

    private void UpdateReloading()
    {

    }

    private IEnumerator CoShotEffect(Vector3 hitPosition)
    {
        audioSource.PlayOneShot(gundata.shootClip);

        muzzleEffect.Play();
        shellEffect.Play();
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePosition.position);
        lineRenderer.SetPosition(1, hitPosition);

        yield return new WaitForSeconds(0.2f);

        lineRenderer.enabled = false;
    }

    public void Fire()
    {
        if(currentState == State.Ready && Time.time > (lastFireTime + gundata.timeBetFire))
        {
            lastFireTime = Time.time;
            Shoot();
        }
    }

    public void Shoot()
    {
        Vector3 hitPosition = Vector3.zero;

        RaycastHit hit;
        if(Physics.Raycast(firePosition.position, firePosition.forward, 
            out hit, gundata.fireDistance))
        {
            hitPosition = hit.point;

            var target = hit.collider.GetComponent<IDamagable>();
            if(target != null)
            {
                target.OnDamage(gundata.damage, hit.point, hit.normal);
            }
        }
        else
        {
            hitPosition = firePosition.position + firePosition.forward * gundata.fireDistance;
        }

        StartCoroutine(CoShotEffect(hitPosition));

        --magAmmo;
        uiManager.SetAmmoText(magAmmo, ammoRemain);
        if (magAmmo == 0)
        {
            CurrentState = State.Empty;
        }
    }

    public bool Reload()
    {
        if (ammoRemain <= 0 || magAmmo == gundata.magCapacity || 
            currentState == State.Reloading)
            return false;

        StartCoroutine(CoReload());

        return true;        
    }

    public IEnumerator CoReload()
    {
        currentState = State.Reloading;
        audioSource.PlayOneShot(gundata.reloadClip);

        yield return new WaitForSeconds(gundata.reloadTime); 

        var reloadAmmo = Mathf.Min(gundata.magCapacity - magAmmo, ammoRemain);
        magAmmo += reloadAmmo;
        ammoRemain -= reloadAmmo;
        uiManager.SetAmmoText(magAmmo, ammoRemain);

        currentState = State.Ready;
    }

    public void AddAmmo(int amount)
    {
        ammoRemain = Mathf.Min(ammoRemain + amount, gundata.startAmmoRemain);
        uiManager.SetAmmoText(magAmmo, ammoRemain);
    }
}
