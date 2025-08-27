using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : LivingEntity
{
    private static readonly int hashHasTarget = Animator.StringToHash("HasTarget");
    private static readonly int hashDie = Animator.StringToHash("Die");

    public enum Status
    {
        Idle,
        Trace,
        Attack,
        Die,
    }

    private Status currentStatus;

    public Status CurrentStatus
    {
        get { return currentStatus; }
        set
        {
            var prevStatus = currentStatus;
            currentStatus = value;

            switch (currentStatus)
            {
                case Status.Idle:
                    animator.SetBool(hashHasTarget, false);
                    agent.isStopped = true;
                    break;
                case Status.Trace:
                    animator.SetBool(hashHasTarget, true);
                    agent.isStopped = false;
                    break;
                case Status.Attack:
                    animator.SetBool(hashHasTarget, false);
                    agent.isStopped = true;
                    break;
                case Status.Die:
                    animator.SetTrigger(hashDie);
                    agent.isStopped = true;
                    capsuleCollider.enabled = false;
                    audioSource.PlayOneShot(deathClip);
                    break;
            }
        }
    }

    private Transform target;

    public float traceDistance;
    public float attackDistance;

    public float damage = 10f;
    public float attackInterval = 0.5f;
    public float lastAttackTime;

    public ParticleSystem bloodSprayEffect;
    public AudioClip hitClip;
    public AudioClip deathClip;

    private Animator animator;
    private NavMeshAgent agent;
    private CapsuleCollider capsuleCollider;
    private AudioSource audioSource;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        switch (currentStatus)
        {
            case Status.Idle:
                UpdateIdle();
                break;
            case Status.Trace:
                UpdateTrace();
                break;
            case Status.Attack:
                UpdateAttack();
                break;
            case Status.Die:
                UpdateDie();
                break;
        }
    }

    private void UpdateIdle()
    {
        if(target != null && 
            Vector3.Distance(transform.position, target.position) < traceDistance)
        {
            CurrentStatus = Status.Trace;
        }

        target = FindTarget(traceDistance);
    }

    private void UpdateTrace()
    {
        if (target != null &&
            Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            CurrentStatus = Status.Attack;
            return;
        }

        if (target == null ||
            Vector3.Distance(transform.position, target.position) > traceDistance)
        {
            CurrentStatus= Status.Idle;
            return;
        }

        agent.SetDestination(target.position);
    }

    private void UpdateAttack()
    {
        if (target == null || 
            Vector3.Distance(transform.position, target.position) > attackDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }

        var lookAt = target.position;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);

        if (lastAttackTime + attackInterval < Time.time )
        {
            lastAttackTime = Time.time;
            var damagable = target.GetComponent<IDamagable>();
            if(damagable != null)
            {
                damagable.OnDamage(damage, transform.position, -transform.forward);
            }
        }
    }

    private void UpdateDie()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();

        capsuleCollider.enabled = true;
        currentStatus = Status.Idle;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        bloodSprayEffect.transform.position = hitPoint;
        bloodSprayEffect.transform.forward = hitNormal;
        bloodSprayEffect.Play();
        audioSource.PlayOneShot(hitClip);
    }

    protected override void Die()
    {
        base.Die();
        CurrentStatus = Status.Die;
    }

    public LayerMask targetLayer;

    protected Transform FindTarget(float radius)
    {
        var colliders = Physics.OverlapSphere(transform.position, radius, targetLayer.value);
        if(colliders.Length == 0 )
        {
            return null;
        }

        var target = colliders.OrderBy(
            x => Vector3.Distance(x.transform.position, transform.position)).First();
        return target.transform;
    }
}
