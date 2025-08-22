using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public ParticleSystem muzzleEffect;
    public ParticleSystem shellEffect;

    private LineRenderer lineRenderer;
    private AudioSource audioSource;

    public Transform firePosition;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CoShotEffect());
        }
    }

    private IEnumerator CoShotEffect()
    {
        muzzleEffect.Play();
        shellEffect.Play();
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, firePosition.position);

        Vector3 endPos = firePosition.position + firePosition.forward * 10f;
        lineRenderer.SetPosition(1, endPos);

        yield return new WaitForSeconds(0.2f);

        lineRenderer.enabled = false;
    }
}
