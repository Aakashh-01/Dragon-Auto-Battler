using UnityEngine;
using System.Collections;
using UnityEngine.AI;

// We now require the component, but we will handle the split logic in Start()
[RequireComponent(typeof(AudioSource))]
public class DragonAutoBattler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats myStats;
    [SerializeField] private DragonMovement movement;
    [SerializeField] private DragonAnimator animator;
    [SerializeField] private Transform enemyTarget;

    // OPTIMIZATION: Cache the enemy stats
    private CharacterStats _targetStats;

    // AUDIO FIX: Separated Sources
    private AudioSource _movementAudio; // For Looping Footsteps
    private AudioSource _sfxAudio;      // For One-Shot Attacks

    private NavMeshAgent _agent;

    [Header("Combat Settings")]
    [SerializeField] private float autoAttackRange = 3.5f;
    [SerializeField] private float spacing = 3.0f;
    [SerializeField] private float attackSpeed = 2.0f;
    [SerializeField] private int basicDamage = 10;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip basicAttackSound;
    [SerializeField] private float basicAttackSoundDelay = 0.3f;
    [SerializeField] private GameObject basicAttackVFX;

    [Space(10)]
    [Tooltip("Looping sound for running/flying")]
    [SerializeField] private AudioClip movementSound;
    [SerializeField][Range(0, 1)] private float movementVolume = 0.5f;

    [Header("AI Behavior")]
    [Tooltip("If false, Enemy will stand still and only fight when you get close.")]
    [SerializeField] private bool shouldChase = false;
    [SerializeField] private float reactionDelay = 2.0f;

    [Header("Abilities")]
    public DragonAbility fireAttack;
    public DragonAbility tailAttack;
    public DragonAbility flyAttack;

    private enum State { Idle, Moving, Combat, Cooldown }
    [SerializeField] private State _currentState = State.Idle;

    // Flags
    private bool _isMovingToGround = false;
    private bool _isChasingEnemy = false;
    private bool _hasDied = false;

    private float _lastBasicAttackTime;
    private float _cooldownTimer = 0f;
    private bool _isAttacking = false;
    private Coroutine _activeAttackRoutine;

    private void Start()
    {
        // --- AUDIO SETUP START ---
        // 1. Setup Movement Audio (Existing Component)
        _movementAudio = GetComponent<AudioSource>();
        _movementAudio.spatialBlend = 0; // Force 2D
        _movementAudio.loop = true;

        // 2. Create SFX Audio (New Component) - Solves the cutting off issue
        _sfxAudio = gameObject.AddComponent<AudioSource>();
        _sfxAudio.spatialBlend = 0; // Force 2D
        _sfxAudio.playOnAwake = false;
        // --- AUDIO SETUP END ---

        _agent = movement.GetComponent<NavMeshAgent>();

        if (_agent != null)
        {
            _agent.stoppingDistance = spacing;
            _agent.autoBraking = true;
        }

        if (enemyTarget != null)
        {
            _targetStats = enemyTarget.GetComponent<CharacterStats>();
        }
    }

    private void Update()
    {
        // 0. EXIT GAME INPUT
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // 1. DEATH CHECK
        if (myStats.IsDead)
        {
            if (!_hasDied) Die();
            return;
        }

        // 2. AUDIO & ANIMATION SAFEGUARD
        if (_isAttacking)
        {
            // Lock position
            if (_agent != null) _agent.isStopped = true;

            // Stop movement sound only (SFX sound is safe on the other source!)
            if (_movementAudio.isPlaying) _movementAudio.Stop();
        }
        else
        {
            if (_agent != null) _agent.isStopped = false;
            HandleMovementAudio();
        }

        // 3. GAMEPLAY LOGIC
        HandleInput();

        switch (_currentState)
        {
            case State.Idle: CheckForEnemies(); break;
            case State.Moving: HandleMovementState(); break;
            case State.Combat: HandleCombat(); break;
            case State.Cooldown: HandleCooldown(); break;
        }
    }

    private void Die()
    {
        _hasDied = true;
        animator.TriggerDeath();

        movement.StopMoving();
        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Stop all sounds
        _movementAudio.Stop();
        _sfxAudio.Stop();

        if (_activeAttackRoutine != null) StopCoroutine(_activeAttackRoutine);

        this.enabled = false;
    }

    private void HandleMovementAudio()
    {
        if (_agent == null || movementSound == null) return;

        bool isMoving = _agent.velocity.sqrMagnitude > 0.1f;

        if (isMoving)
        {
            if (!_movementAudio.isPlaying || _movementAudio.clip != movementSound)
            {
                _movementAudio.clip = movementSound;
                _movementAudio.volume = movementVolume;
                _movementAudio.Play();
            }
        }
        else
        {
            if (_movementAudio.isPlaying)
            {
                _movementAudio.Stop();
            }
        }
    }

    private void HandleInput()
    {
        if (!gameObject.CompareTag("Player")) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                CancelCombat();

                if (hit.transform == enemyTarget)
                {
                    _isChasingEnemy = true;
                    _isMovingToGround = false;
                    movement.MoveTo(hit.transform.position);
                }
                else
                {
                    _isChasingEnemy = false;
                    _isMovingToGround = true;
                    movement.MoveTo(hit.point);
                }

                _currentState = State.Moving;
            }
        }
    }

    private void HandleMovementState()
    {
        if (_agent.pathPending) return;

        if (_isChasingEnemy && enemyTarget != null)
        {
            float distToEnemy = Vector3.Distance(transform.position, enemyTarget.position);

            if (distToEnemy <= autoAttackRange + spacing)
            {
                StartCombat();
            }
            else
            {
                movement.MoveTo(enemyTarget.position);
            }
            return;
        }

        if (_isMovingToGround)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance + 0.1f)
            {
                _isMovingToGround = false;
                _currentState = State.Idle;
            }
            return;
        }
    }

    private void CheckForEnemies()
    {
        if (enemyTarget == null) return;
        if (_agent.velocity.sqrMagnitude > 0.1f) return;

        float distance = Vector3.Distance(transform.position, enemyTarget.position);

        if (distance <= autoAttackRange + spacing)
        {
            StartCombat();
        }
        else
        {
            if (!gameObject.CompareTag("Player") && shouldChase)
            {
                movement.MoveTo(enemyTarget.position);
                _currentState = State.Moving;
            }
        }
    }

    private void StartCombat()
    {
        if (_currentState != State.Combat)
        {
            _currentState = State.Combat;
            _isMovingToGround = false;
            _isChasingEnemy = false;
            movement.StopMoving();
        }
    }

    private void HandleCombat()
    {
        if (_targetStats == null || _targetStats.IsDead)
        {
            _currentState = State.Idle;
            return;
        }

        if (!_isAttacking) movement.LookAtTarget(enemyTarget.position);

        if (_isAttacking) return;

        float distance = Vector3.Distance(transform.position, enemyTarget.position);
        if (distance > autoAttackRange + spacing + 1.5f)
        {
            EnterCooldown();
            return;
        }

        if (TryUseAbility(flyAttack, "FlyAttack")) return;
        if (TryUseAbility(fireAttack, "FireAttack")) return;
        if (TryUseAbility(tailAttack, "TailAttack")) return;

        if (Time.time >= _lastBasicAttackTime + attackSpeed) PerformBasicAttack();
    }

    private void EnterCooldown()
    {
        _currentState = State.Cooldown;
        _cooldownTimer = reactionDelay;
        movement.StopMoving();
    }

    private void HandleCooldown()
    {
        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer <= 0)
        {
            _currentState = State.Idle;
        }
    }

    private void CancelCombat()
    {
        if (_activeAttackRoutine != null) StopCoroutine(_activeAttackRoutine);
        _isAttacking = false;
        if (_agent != null) _agent.isStopped = false;
    }

    private void PerformBasicAttack()
    {
        _lastBasicAttackTime = Time.time;
        _activeAttackRoutine = StartCoroutine(AttackRoutine(1.0f, 0, basicDamage, basicAttackVFX, basicAttackSound, basicAttackSoundDelay));
    }

    private bool TryUseAbility(DragonAbility ability, string triggerName)
    {
        if (ability != null && ability.IsReady())
        {
            ability.Use();
            int animIndex = 0;
            if (triggerName == "FireAttack" || ability.abilityName == "fire") animIndex = 1;
            else if (triggerName == "TailAttack" || ability.abilityName == "tail") animIndex = 2;
            else if (triggerName == "FlyAttack" || ability.abilityName == "fly") animIndex = 3;

            _activeAttackRoutine = StartCoroutine(AttackRoutine(1.5f, animIndex, ability.damage, ability.vfxPrefab, ability.soundEffect, ability.soundDelay));
            return true;
        }
        return false;
    }

    private IEnumerator AttackRoutine(float duration, int animIndex, int damage, GameObject vfx, AudioClip soundClip, float soundDelay)
    {
        _isAttacking = true;

        // Use the new TRIGGER method (if you updated DragonAnimator) or standard call
        animator.TriggerAttack(animIndex);

        float timer = 0f;
        bool soundPlayed = false;
        bool damageDealt = false;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (!soundPlayed && timer >= soundDelay)
            {
                // FORCE ATTACK SOUND ON SEPARATE CHANNEL
                if (soundClip != null)
                {
                    _sfxAudio.PlayOneShot(soundClip, 1.0f); // Always plays at volume 1.0
                }
                soundPlayed = true;
            }

            if (!damageDealt && timer >= 0.5f)
            {
                if (_targetStats != null && !_targetStats.IsDead)
                {
                    float dist = Vector3.Distance(transform.position, enemyTarget.position);
                    if (dist <= autoAttackRange + spacing + 3.0f)
                    {
                        if (vfx != null) Instantiate(vfx, enemyTarget.position + Vector3.up, Quaternion.identity);
                        _targetStats.TakeDamage(damage);
                    }
                }
                damageDealt = true;
            }
            yield return null;
        }
        _isAttacking = false;
        if (_agent != null) _agent.isStopped = false;
    }
}