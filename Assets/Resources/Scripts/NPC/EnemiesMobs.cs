using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemiesMobs : NPC
{
	#region Private fields
	[Header("Atack Properties")]
	[SerializeField] private Transform _target;
    [SerializeField] private float _attackSpeedMultiplier;
    [SerializeField] private bool _isTargetInAttackArea;
	[SerializeField] private float _attackCooldown;
	[SerializeField] private float _selfShootingCooldown;
    [SerializeField] private float _extraDistance;
    [SerializeField] private GameObject _attackCollider;
    [SerializeField] private GameObject _hitCollider;
    private bool _isNeedToCheckGround;
	#endregion

	#region Properties
	public Transform Target
	{
		get
		{
			return _target;
		}

		set
		{
			_target = value;
		}
	}

    public float AttackCooldown
    {
        get
        {
            return _attackCooldown;
        }

        set
        {
            _attackCooldown = value;
        }
    }

    public float AttackSpeedMultiplier
    {
        get
        {
            return _attackSpeedMultiplier;
        }

        set
        {
            _attackSpeedMultiplier = value;
        }
    }

    public float SelfShootingCooldown
    {
        get
        {
            return _selfShootingCooldown;
        }

        set
        {
            _selfShootingCooldown = value;
        }
    }

    public bool IsTargetInAttackArea
    {
        get
        {
            return _isTargetInAttackArea;
        }

        set
        {
            _isTargetInAttackArea = value;
        }
    }

    public float ExtraDistance
    {
        get
        {
            return _extraDistance;
        }

        set
        {
            _extraDistance = value;
        }
    }


    #endregion

    #region Methods
    private new void OnValidate()
	{
		base.OnValidate();
		GameObject target = GameObject.FindGameObjectWithTag("Player");
		if (target != null)
		{
			Target = target.transform;
		}

		if (!transform.Find("AttackCollider"))
		{
            _attackCollider = new GameObject("AttackCollider");
            _attackCollider.AddComponent<MobAttackController>();
            _attackCollider.transform.parent = transform;
            _attackCollider.transform.position = transform.position;
		}

		if (!transform.Find("HitCollider"))
		{
            _hitCollider = new GameObject("HitCollider");
            _hitCollider.AddComponent<PolygonCollider2D>();
            _hitCollider.GetComponent<PolygonCollider2D>().isTrigger = true;
            _hitCollider.AddComponent<MobHitController>();
            _hitCollider.transform.parent = transform;
            _hitCollider.transform.position = transform.position;
		}
	}

    void Start()
	{
        base.Start();
        switch (Id)
        {
            case NPCID.Slime:
                {
					IsWalking = true;
                }
                break;
            case NPCID.SkeletonSword:
                {
                    IsWalking = true;
                }
                break;
            default:
                break;
        }
	}

    private void Update()
    {
        Animate();
    }

    void FixedUpdate()
    {
        ActionCooldown += Time.fixedDeltaTime;
        ChooseDirection();
        switch (Id)
        {
            case NPCID.Slime:
                {
                    SlimeAI();
                }
                break;
            case NPCID.SkeletonSword:
                {
                    SkeletonSwordAI();
                }
                break;
            default:
                break;
        }
    }

	private void SkeletonSwordAI()
	{
        //Set flags
        if (!IsJumping)
        {
            IsGrounded = GroundCheck();
        }
        else
        {
            IsGrounded = false;
        }
        IsFalling = FallingCheck();

        //Nothing-Idle
        if (CurrentAction == MobAction.Nothing && IsGrounded)
        {
            CurrentAction = MobAction.Idle;
            ActionCooldown = 0f;
        }
        //Idle-Walk
        if (CurrentAction == MobAction.Idle)
        {
            CurrentAction = MobAction.Walk;
            Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        //Idle-Attack
        if (CurrentAction == MobAction.Idle && IsTargetInAttackArea && ActionCooldown >= AttackCooldown)
        {
            CurrentAction = MobAction.Attack;
            IsAttacking = true;
        }
        //Walk-Attack
        if (CurrentAction == MobAction.Walk && IsTargetInAttackArea && ActionCooldown >= AttackCooldown)
        {
            CurrentAction = MobAction.Attack;
            IsAttacking = true;
        }
        //Walk-Jump
        if (CurrentAction == MobAction.Walk && IsJumping)
        {
            CurrentAction = MobAction.Jump;
        }
        //Any-Fall
        if (IsFalling)
        {
            CurrentAction = MobAction.Fall;
            IsJumping = false;
        }
        //Fall-Idle
        if (CurrentAction == MobAction.Fall && IsGrounded)
        {
            CurrentAction = MobAction.Idle;
            ActionCooldown = 0f;
        }
        //Run-Attack
        if (CurrentAction == MobAction.Run && IsTargetInAttackArea && ActionCooldown >= AttackCooldown)
        {
            CurrentAction = MobAction.Attack;
            IsAttacking = true;
        }
        //Attack-Idle
        if (CurrentAction == MobAction.Attack && !IsAttacking)
        {
            CurrentAction = MobAction.Idle;
            IsWalking = true;
            ActionCooldown = 0f;

        }
        float speed = Random.Range(MinSpeed, MaxSpeed);

        switch (CurrentAction)
        {
            case MobAction.Nothing:
                break;
            case MobAction.Idle:
                {
                    Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
                break;
            case MobAction.Walk:
                {
                    ChangeScaleByMoveDirection();
                    Rigidbody.velocity = new Vector2(speed * MoveDirection, Rigidbody.velocity.y);

                    //Check if we can jump
                    //Vector3Int intPosition = GameManager.Instance.World.BlockTilemap.WorldToCell(transform.position);
                    //if (GameManager.Instance.ObjectsData[intPosition.x + MoveDirection, intPosition.y].IsSolidBlock() && !IsTargetInAttackArea)
                    //{

                    //     IsJumping = true;
                    //        Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, JumpForce);
                        
                        
                    //}
                }
                break;
            case MobAction.Run:
                break;
            case MobAction.Jump:
                {

                    Rigidbody.velocity = new Vector2(MoveDirection*JumpXForce, Rigidbody.velocity.y);
                    
                }
                break;
            case MobAction.Fall:
                break;
            case MobAction.Attack:
                {
                    Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                    Rigidbody.velocity = Vector2.zero;
                    IsWalking = false;
                }
                break;
            case MobAction.Death:
                break;
            default:
                break;
        }
    }

	private void SlimeAI()
	{
		//Set flags
        if (!IsJumping)
        {
            IsGrounded = GroundCheck();
        }
        else
        {
            IsGrounded = false;
        }
        IsFalling = FallingCheck();

        //Nothing-Idle
        if (CurrentAction == MobAction.Nothing && IsGrounded)
        {
            CurrentAction = MobAction.Idle;
            ActionCooldown = 0f;
        }
        //Idle-Attack
        if (CurrentAction == MobAction.Idle && IsTargetInAttackArea && ActionCooldown >= AttackCooldown)
        {
            CurrentAction = MobAction.Attack;
        }
        //Attack-Jump
        if (CurrentAction == MobAction.Attack && IsJumping)
        {
            CurrentAction = MobAction.Jump;
        }
        //Idle-Walk
        if (CurrentAction == MobAction.Idle && IsWalking && ActionCooldown >= SelfShootingCooldown)
        {
            CurrentAction = MobAction.Walk;
        }
        //Walk-Jump
        if (CurrentAction == MobAction.Walk && IsJumping)
        {
            CurrentAction = MobAction.Jump;
        }
        //Any-Fall
        if (IsFalling)
        {
            CurrentAction = MobAction.Fall;
            IsJumping = false;
        }
        //Fall-Idle
        if (CurrentAction == MobAction.Fall && IsGrounded)
        {
            CurrentAction = MobAction.Idle;
            ActionCooldown = 0f;
        }
        float speed = Random.Range(MinSpeed, MaxSpeed);


        Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        switch (CurrentAction)
		{
            case MobAction.Nothing:
                break;
			case MobAction.Idle:
				{
                    ChangeScaleToNormal();
                    Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
				}
                break;
			case MobAction.Walk:
                {
                    IsJumping = true;
                    ChangeScaleByMoveDirection();
                    Rigidbody.velocity = new Vector2(speed * MoveDirection, JumpForce);
                }
				break;
			case MobAction.Run:
				break;
            case MobAction.Jump:
                break;
            case MobAction.Fall:
                break;
            case MobAction.Attack:
				{
                    IsJumping = true;
                    ChangeScaleByMoveDirection();
                    Rigidbody.velocity = new Vector2(speed * MoveDirection * AttackSpeedMultiplier, JumpForce);
				}
				break;
			case MobAction.Death:
				break;
			default:
				break;
		}
	}

    public void EndAttack()
    {
        IsAttacking = false;
    }

	private void ChooseDirection()
	{
        if (IsJumping) 
        {
            return;
        }
		if (transform.position.x - Target.position.x < 0)
		{
			MoveDirection = 1;
		}
		else
		{
			MoveDirection = -1;
		}
	}

    private void ChangeScaleByMoveDirection()
    {
        if (IsFacingRight && MoveDirection < 0)
        {
            Flip();
        }
        if (!IsFacingRight && MoveDirection > 0)
        {
            Flip();
        }
    }

    private void ChangeScaleToNormal()
    {
        if (!IsFacingRight)
        {
            Flip();
        }
    }

    public void UpdatePhysicsShape()
    {
        List<Vector2> physicsShape = new List<Vector2>();
        SpriteRenderer.sprite.GetPhysicsShape(0, physicsShape);
        _hitCollider.GetComponent<PolygonCollider2D>().SetPath(0, physicsShape);
    }
    #endregion
}
