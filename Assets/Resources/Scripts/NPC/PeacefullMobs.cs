using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeacefullMobs : NPC
{
    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        switch (Id)
        {
            case NPCID.Bunny:
                {
                    MoveDirection = 0;
                    CurrentRandomDirectionChange = Random.Range(MinRandomDirectionChange, MaxRandomDirectionChange);
                    IsWalking = true;
                }
                break;
            case NPCID.Crow:
                {
                    MoveDirection = 1;
                    CurrentRandomDirectionChange = Random.Range(MinRandomDirectionChange, MaxRandomDirectionChange);
                }
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
        switch (Id)
        {
            case NPCID.Bunny:
                {
                    BunnyAI();
                }
                break;
            case NPCID.Crow:
                {
                    CrowAI();
                }
                break;
            default:
                break;
        }
    }
    private void BunnyAI()
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

        if ((CurrentAction == MobAction.Idle || CurrentAction == MobAction.Walk) &&
            ActionCooldown >= CurrentRandomDirectionChange)
        {
            ChooseRandomDirection();
        }

        //Nothing-Idle
        if (CurrentAction == MobAction.Nothing && IsGrounded)
        {
            CurrentAction = MobAction.Idle;
            ActionCooldown = 0f;
        }
        //Idle-Walk
        if (CurrentAction == MobAction.Idle && MoveDirection != 0)
        {
            CurrentAction = MobAction.Walk;
        }
        //Walk-Idle
        if (CurrentAction == MobAction.Walk && MoveDirection == 0)
        {
            CurrentAction = MobAction.Idle;
        }
        //Walk - Jump
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
        //Fall-Idle / Fall-Walk
        if (CurrentAction == MobAction.Fall && IsGrounded)
        {
            if (MoveDirection == 0)
            {
                CurrentAction = MobAction.Idle;
            }
            else
            {
                CurrentAction = MobAction.Walk;
            }
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

                    // Check if we can jump

                    Vector3Int intPosition = GameManager.Instance.World.BlockTilemap.WorldToCell(transform.position);

                    if (!GameManager.Instance.ObjectsData[intPosition.x + MoveDirection, intPosition.y].IsSolidBlock())
                    {
                        break;
                    }
                    if (GameManager.Instance.ObjectsData[intPosition.x + MoveDirection, intPosition.y + 2].IsSolidBlock())
                    {
                        MoveDirection *= -1;
                        break;
                    }
                    if (GameManager.Instance.ObjectsData[intPosition.x + MoveDirection, intPosition.y + 1].IsSolidBlock())
                    {
                        if (!GameManager.Instance.ObjectsData[intPosition.x, intPosition.y + 1].IsSolidBlock())
                        {
                            IsJumping = true;
                            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, JumpForce);
                        }
                    }
                }
                break;
            case MobAction.Run:
                break;
            case MobAction.Jump:
                {
                    Rigidbody.velocity = new Vector2(MoveDirection * JumpXForce, Rigidbody.velocity.y);
                }
                break;
            case MobAction.Fall:
                break;
            //case MobAction.Attack:
            //    {
            //        Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            //        Rigidbody.velocity = Vector2.zero;
            //        IsWalking = false;
            //    }
            //    break;
            case MobAction.Death:
                break;
            default:
                break;
        }

    }
    private void CrowAI()
    {
        int moveYDir = Random.Range(0,101);
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

        if (ActionCooldown >= CurrentRandomDirectionChange)
        {
            ChooseRandomDirection();
            if ((CurrentAction == MobAction.Jump || CurrentAction == MobAction.Fall) && 
                MoveDirection == 0 )
            {
                int dir =Random.Range(1,3);
                MoveDirection+= dir ==1 ? 1: -1;
            }
        }
        //Nothing-Jump
        if (CurrentAction == MobAction.Nothing )
        {
            CurrentAction = MobAction.Jump;
            ActionCooldown = 0f;
        }
        //Idle-Walk
        if (CurrentAction == MobAction.Idle && MoveDirection != 0)
        {
            CurrentAction = MobAction.Walk;
        }
        //Walk-Idle
        if (CurrentAction == MobAction.Walk && MoveDirection == 0)
        {
            CurrentAction = MobAction.Idle;
        }
        //Walk - Jump
        if (CurrentAction == MobAction.Walk && IsJumping)
        {
            CurrentAction = MobAction.Jump;
        }
        //Idle - Jump
        if (CurrentAction == MobAction.Idle && IsJumping)
        {
            CurrentAction = MobAction.Jump;
        }
        //Jump-Fall
        if (CurrentAction == MobAction.Jump && moveYDir <= 50)
        {
            CurrentAction = MobAction.Fall;
            IsJumping = false;
        }
        //Fall-Jump
        if (CurrentAction == MobAction.Fall && moveYDir >= 50)
        {
            CurrentAction = MobAction.Jump;
            IsJumping = true;
        }
        //Fall-Idle / Fall-Walk
        if (CurrentAction == MobAction.Fall && IsGrounded)
        {
            if (MoveDirection == 0)
            {
                CurrentAction = MobAction.Idle;
            }
            else
            {
                CurrentAction = MobAction.Walk;
            }
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

                    // Check if we can jump

                    Vector3Int intPosition = GameManager.Instance.World.BlockTilemap.WorldToCell(transform.position);

                    if (!GameManager.Instance.ObjectsData[intPosition.x + MoveDirection, intPosition.y].IsSolidBlock())
                    {
                        break;
                    }
                    if (GameManager.Instance.ObjectsData[intPosition.x + MoveDirection, intPosition.y + 2].IsSolidBlock())
                    {
                        MoveDirection *= -1;
                        break;
                    }
                    
                }
                break;
            case MobAction.Run:
                break;
            case MobAction.Jump:
                {
                    Rigidbody.velocity = new Vector2(MoveDirection * JumpXForce, JumpForce);
                }
                break;
            case MobAction.Fall:
                { 
                    Rigidbody.velocity = new Vector2(MoveDirection * JumpXForce, -JumpForce);
                }
                break;
            //case MobAction.Attack:
            //    {
            //        Rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            //        Rigidbody.velocity = Vector2.zero;
            //        IsWalking = false;
            //    }
            //    break;
            case MobAction.Death:
                break;
            default:
                break;
        }
    }
#endregion
}
