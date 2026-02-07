using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DragonAnimator))]
public class DragonMovement : MonoBehaviour
{
    private NavMeshAgent _agent;
    private DragonAnimator _animator;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<DragonAnimator>();
    }

    private void Update()
    {
        // FIX 1: Force Animation
        // Instead of calculating math, we just ask: "Are we moving?"
        // If yes, tell Animator "Speed = 1" (Full Run).
        if (_agent.hasPath && _agent.velocity.sqrMagnitude > 0.1f)
        {
            _animator.UpdateMovementSpeed(1.0f);
        }
        else
        {
            _animator.UpdateMovementSpeed(0f);
        }
    }

    public void MoveTo(Vector3 destination)
    {
        if (_agent.enabled)
        {
            _agent.updateRotation = true;
            _agent.SetDestination(destination);
            _agent.isStopped = false;
        }
    }

    public void StopMoving()
    {
        if (_agent.enabled)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _animator.UpdateMovementSpeed(0);
        }
    }

    public void LookAtTarget(Vector3 target)
    {
        if (_agent.enabled) _agent.updateRotation = false;

        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20f);
        }
    }
}