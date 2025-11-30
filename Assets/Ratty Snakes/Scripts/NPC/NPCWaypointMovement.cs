using UnityEngine;
using System;

public class NPCWaypointMovement : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Movement")]
    public float speed = 1f;
    public float rotationSpeed = 5f;

    [Header("State")]
    public bool isWaitingDecision = false;

    private int currentIndex = 0;
    private bool isMoving = false;

    // Callback al llegar a WP_1 (para mostrar UI)
    public Action onReachedWaypoint1;

    // Callback al llegar al último waypoint (WP_5)
    public Action onReachedLastWaypoint;

    void Update()
    {
        if (!isMoving || waypoints == null || waypoints.Length == 0 || currentIndex >= waypoints.Length)
            return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;

        // Movimiento
        transform.position += direction * speed * Time.deltaTime;

        // Rotación suave hacia el waypoint
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Llegada al waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            if (currentIndex == 1)
            {
                // Llega al WP_1 y espera decisión
                isMoving = false;
                isWaitingDecision = true;
                onReachedWaypoint1?.Invoke();
            }
            else if (currentIndex == waypoints.Length - 1)
            {
                // Último waypoint: destruir NPC y notificar al manager
                onReachedLastWaypoint?.Invoke();
                Destroy(gameObject);
            }
            else
            {
                currentIndex++;
            }
        }
    }

    public void StartWalking()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        currentIndex = 0;
        isMoving = true;
    }

    // Continuar desde WP_1 hacia WP_2→WP_5
    public void ContinueWalking()
    {
        if (!isWaitingDecision) return;

        isWaitingDecision = false;

        if (currentIndex == 1)
            currentIndex = 2;

        isMoving = true;
    }
}
