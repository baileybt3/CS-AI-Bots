using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]

public class NPCRoam : MonoBehaviour
{
    [Header("Roam")]
    public float roamRadius = 20f;
    public float repathDelay = 1.0f;

    [Header("Animation")]
    public string speedParam = "Velocity";
    public float animDamp = 0.12f;

    NavMeshAgent agent;
    Animator anim;
    Vector3 home;
    float repathTimer;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        home = transform.position;

        if (!agent.isOnNavMesh)
        {
            if(NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    void Start()
    {
        PickNewDestination();
    }

    void Update()
    {
        float normalized = agent.velocity.magnitude / Mathf.Max(agent.speed, 0.01f);
        anim.SetFloat(speedParam, Mathf.Clamp01(normalized), animDamp, Time.deltaTime);

        repathTimer -= Time.deltaTime;
        bool arrived = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        if (repathTimer <= 0f && (arrived || agent.velocity.sqrMagnitude < 0.0001f))
        {
            PickNewDestination();
            repathTimer = repathDelay;
        }
    }

    void PickNewDestination()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3 random = home + Random.insideUnitSphere * roamRadius;
            if (NavMesh.SamplePosition(random, out var hit, 4f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }
}
