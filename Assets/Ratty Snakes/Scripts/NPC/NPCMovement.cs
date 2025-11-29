using UnityEngine;
using System;

public class NPCMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.5f;
    public float stopDistance = 0.05f;

    public Transform target;
    public bool hasTarget = false;

    public Action OnReachedTarget;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        hasTarget = true;

        Debug.Log("NPCMovement: Target asignado -> " + newTarget.name);
    }

    void Update()
    {
        if (!hasTarget || target == null)
            return;

        // Mover
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // Rotar
        Vector3 direction = target.position - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 2f * Time.deltaTime);
        }

        // Llegada
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= stopDistance)
        {
            Debug.Log("NPCMovement: NPC ha llegado al target.");

            hasTarget = false;

            if (OnReachedTarget != null)
            {
                Debug.Log("NPCMovement: Ejecutando evento OnReachedTarget...");
                OnReachedTarget.Invoke();
            }
            else
            {
                Debug.LogWarning("NPCMovement: OnReachedTarget es NULL (nadie está escuchando).");
            }
        }
    }
}
