using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemiesMobs : NPC
{
	#region Private fields

	#endregion

	#region Properties

    #endregion

    #region Methods
    void Start()
	{
        base.Start();
        Target = GameManager.Instance.Player.transform;
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

    void FixedUpdate()
    {
        base.FixedUpdate();
        ChooseDirectionByTarget();
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
                    Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    Rigidbody.velocity = new Vector2(speed * MoveDirection, Rigidbody.velocity.y);

                    //Check if we can jump
                    Vector3Int intPosition = GameManager.Instance.World.BlockTilemap.WorldToCell(transform.position);
                    if (GameManager.Instance.ObjectsData[intPosition.x + MoveDirection, intPosition.y].IsSolidBlock() && !IsTargetInAttackArea)
                    {
                        IsJumping = true;
                        Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, JumpForce);
                    }
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
    #endregion
}
