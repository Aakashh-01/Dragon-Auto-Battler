using UnityEngine;

public class DragonAnimator : MonoBehaviour
{
    private Animator _animator;

    // Hashes for performance
    private readonly int _speedHash = Animator.StringToHash("Speed");
    private readonly int _attackTriggerHash = Animator.StringToHash("Attack");
    private readonly int _indexHash = Animator.StringToHash("Index");
    private readonly int _dieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdateMovementSpeed(float speed)
    {
        _animator.SetFloat(_speedHash, speed);
    }

    public void TriggerAttack(int attackIndex)
    {
        // DEBUG: Shows which attack is requested
        Debug.Log($"[ANIMATION] Triggering Attack Index: {attackIndex} on {gameObject.name}");

        _animator.SetInteger(_indexHash, attackIndex);
        _animator.SetTrigger(_attackTriggerHash);
    }

    public void TriggerDeath()
    {
        Debug.Log($"[ANIMATION] Triggering Death on {gameObject.name}");
        _animator.SetTrigger(_dieHash);
    }
}