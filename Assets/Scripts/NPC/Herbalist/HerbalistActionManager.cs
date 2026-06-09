using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;
using System.Collections;

public class HerbalistActionManager : MonoBehaviour
{
    [Header("NavMesh")]
    private NavMeshAgent agent;

    [Header("Target Tags (priority order)")]
    private string[] priorityTags = { "Enemy", "Berrybush", "Checkpoint" };

    [Header("NPC Settings")]

    private bool monologHasStarted = false;
    private NPCMonologue monologue;
    private Transform currentTarget;

    private Dictionary<string, Action<Transform>> playerFamiliarityActions;
    private Dictionary<string, Action<Transform>> npcFamiliarityActions;
    private FieldOfView fov;

    [Header("Targeting")]
    private float giveUpDistance = 300f;       // Stop immediately if target beyond this
    private float followTimeout = 4f;         // Seconds to give up if player walks away
    private Vector3 lastTargetPosition;
    private bool allowChasing = true;
    private float followTimer = 0f;

    void Awake()
    {
        monologue = GetComponent<NPCMonologue>();
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();

        // Setup actions for each familiarity type
        playerFamiliarityActions = new Dictionary<string, Action<Transform>>
        {
            { "friendly", (target) =>
                {
                    //agent.SetDestination(currentTarget.position);
                    if (allowChasing){
                        SetTarget(target);
                    }
                    if (!monologHasStarted && allowChasing){
                        monologue.StartMonologue();
                        monologHasStarted = true;
                    }
                    // currentTarget is already set below
                }
            },
            { "neutral", (target) =>
                {
                    // neutral behavior
                }
            },
            { "hostile", (target) =>
                {
                    // hostile behavior
                }
            },
            { "loyalfriend", (target) =>
                {
                    // loyal friend behavior
                }
            }
        };

        // Setup actions for each familiarity type
        npcFamiliarityActions = new Dictionary<string, Action<Transform>>
        {
            { "friendly", (target) =>
                {
                    Debug.Log("NPC fam action");
                    // currentTarget is already set below
                }
            },
            { "neutral", (target) =>
                {
                    // neutral behavior
                }
            },
            { "hostile", (target) =>
                {
                    // hostile behavior
                }
            },
            { "loyalfriend", (target) =>
                {
                    // loyal friend behavior
                }
            }
        };
    }

    void Update()
    {
        // 1. Detect any visible targets in FOV
        List<Transform> visibleTargets = fov.GetVisibleTargets();

        Transform playerTarget = null;

        List<CharacterStats> npcTargets = new();

        // Get this NPC’s stats once
        CharacterStats selfStats = GetComponent<CharacterStats>();

        foreach (var target in visibleTargets)
        {
            // Skip anything without CharacterStats
            if (!target.TryGetComponent<CharacterStats>(out var targetStats)) continue;

            // Determine relationship between self and target
            string relation = RelationshipDatabase.GetRelationship(
                selfStats.NpcId, selfStats.Faction,
                targetStats.NpcId, targetStats.Faction
            );
            if (target.CompareTag("Player"))
            {
                playerTarget = target;

                // Invoke player action based on computed relation
                if (playerFamiliarityActions.ContainsKey(relation))
                    playerFamiliarityActions[relation].Invoke(playerTarget);
            }
            else if (target.CompareTag("NPC"))
            {
                npcTargets.Add(targetStats);
            }
        }

        // Handle each NPC target after collecting them
        foreach (var npcStats in npcTargets)
        {
            string relation = RelationshipDatabase.GetRelationship(
                selfStats.NpcId, selfStats.Faction,
                npcStats.NpcId, npcStats.Faction
            );

            if (npcFamiliarityActions.ContainsKey(relation))
                npcFamiliarityActions[relation].Invoke(npcStats.transform);
        }
        // If player was found, stop this iteration.
        if (playerTarget != null)
        {
            return;
        }
        // 2. No player in FOV → stop monologue
        monologHasStarted = false;
        monologue.StopMonologue();


        // 3. Face current target if stopped
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude < 0.01f)
        {
            TargetingHelper.FaceTarget(transform, currentTarget);
        }

        // 4. Move to priority tags
        foreach (string tag in priorityTags)
        {
            GameObject target = TargetingHelper.FindClosestWithTag(transform, tag);
            if (target != null)
            {
                SetTarget(target.transform);
                return;
            }
        }

        // 5. No targets
        currentTarget = null;
        agent.ResetPath();
    }


    private void SetTarget(Transform target)
    {   // --- No target ----
        if (target == null)
        {
            agent.ResetPath();
            followTimer = 0f;
            currentTarget = null;
            return;
        }

        // Face the target every frame
        TargetingHelper.FaceTarget(transform, target);

        // --- Too far away ----
        // Stop immediately if target is beyond give-up distance
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > giveUpDistance)
        {
            agent.ResetPath();
            followTimer = 0f;
            currentTarget = null;
            monologue.StopChasingResponse();
            return;
        }

        // --- Player walking away from the npc ----
        // Calculate how the target moved since last frame
        float distanceChange = distanceToTarget - Vector3.Distance(transform.position, lastTargetPosition);

        // If the target is moving away (distance increasing), increment follow timer
        if (distanceChange > 0.01f) // small threshold to avoid jitter
        {
            followTimer += Time.deltaTime;
            if (followTimer >= followTimeout)
            {
                agent.ResetPath();
                followTimer = 0f;
                currentTarget = null;
                monologue.StopChasingResponse();
                allowChasing = false;
                StartCoroutine(ReallowChasing(20f));
                return;
            }
        }
        else
        {
            followTimer = 0f; // reset timer if target is approaching or stationary
        }

        // --- Either new target or keep chasing moving target. ----
        // Only update path if target moved significantly or is a new target
        if (target != currentTarget || Vector3.Distance(target.position, lastTargetPosition) > 0.01f)
        {
            currentTarget = target;
            agent.SetDestination(currentTarget.position);
        }

        // Update last position for next frame
        lastTargetPosition = target.position;
    }
    // Character can chase again after set time.
    IEnumerator ReallowChasing(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        allowChasing = true;
    }
}
