using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class IceClimberMovement : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;

    [SerializeField] private float walkSpeed = 6;
    [SerializeField] private float gravityScale = 2000f;
    [SerializeField] private float gravityDown = 0.5f;
    [SerializeField] float jumpSpeed = 1f;
    [SerializeField] float jumpDecreaseSpeed = 1f;
    private bool jumping;
    private float moveUp = 0;

    [SerializeField] private float stunDuration = .5f;
    private float stunTime;
    public bool Stunned { get => stunTime > 0; }
    private Vector2 stunDirectionModifier = Vector2.one;

    [SerializeField] private float invulnerableDuration;
    private float invulnerableTime;
    public bool Invulnerable { get => invulnerableTime > 0; }

    private bool isHitByGhost;
    [NonSerialized] public bool dying = false;
    [NonSerialized] public UnityEvent directorevent;

    [SerializeField] private Matrix matrix;
    private Animator animator;

    private const float GRAVITATIONALDIRECTION = -1;
    private const float JUMPHEIGHTURNINGPOINT = -0.5f;
    private const float FEETOFFSET = -.5f;
    private const float HEADOFFSET = 2f;

    float verticalDirection { get { return moveUp < 0 ? FEETOFFSET : HEADOFFSET; } }
    int horizontalDirection = -1;

    [SerializeField] private LayerMask terrainLayer;

    private bool waitingForCountdown;
    public bool WaitingForCountdown
    {
        get { return waitingForCountdown; }
        set
        {
            waitingForCountdown = value;
            animator.SetBool("Countdown", waitingForCountdown);
        }
    }

    public IndividualInput input;

    private void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        if (directorevent == null)
            directorevent = new UnityEvent();
        if (input == null)
        {
            input = new IndividualInput();
            input.SetControllerOption(1);
        }
    }
    public void Initialize()
    {
        animator = GetComponent<Animator>();
        directorevent = new UnityEvent();
        if (input == null)
        {
            input = new IndividualInput();
            input.SetControllerOption(1);
        }
    }

    public void AddDirectorEvent(UnityAction call)
    {
        directorevent.AddListener(call);
    }
    public void RemoveDirectorEvent(UnityAction call)
    {
        directorevent.RemoveListener(call);
    }

    public void ResetToStart()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        jumping = false;
        moveUp = JUMPHEIGHTURNINGPOINT;
        transform.position = startPosition; jumping = true;
        invulnerableTime = invulnerableDuration;
    }

    private void Update()
    {
        input.UpdateAxisValues();
        if (Invulnerable)
        {
            invulnerableTime -= Time.deltaTime;
            isHitByGhost = false;
            GetComponent<Collider2D>().enabled = !isHitByGhost;
        }
        Gravity();

        Jump();

        if (waitingForCountdown)
            return;
        MovePlayer();

        if (Stunned)
        {
            stunTime -= Time.deltaTime;
            if (!Stunned)
                stunDirectionModifier = Vector2.one;
        }

        animator.SetBool("IsStunned", Stunned);

        if (transform.position.y < -12)
            OnDeath();
    }

    public void LaunchAway(Vector3 fromThisPosition, Vector2 stunDirection)
    {
        stunDirectionModifier = Vector2.one * stunDirection;
        LaunchAway(fromThisPosition);
    }
    public void LaunchAway(Vector3 fromThisPosition)
    {
        OnJump();
        horizontalDirection = transform.position.x < fromThisPosition.x ? -1 : 1;
        stunTime = stunDuration;
        invulnerableTime = invulnerableDuration;
        if (!dying)
            SoundController.instance.PlaySound(GetHurtSFXName(), .25f);
        else
            SoundController.instance.PlaySound("hermitdeath", .75f);
        animator.SetTrigger("Knockback");
    }

    private string GetHurtSFXName()
    {
        string[] sfxNames = new string[] {
            "Oooh",
            "OoOoOooh",
            "OoOow",
            "Uhuhuh",
            "AaaAaAah"
        };
        return sfxNames[Random.Range(0, sfxNames.Length)];
    }

    void MovePlayer()
    {
        Debug.Log(input.Horizontal);
        float horizontalMovement = (Stunned || dying) ? (!jumping ? 0 : horizontalDirection) : input.Horizontal;//Input.GetAxisRaw("Horizontal");
        bool isMoving = horizontalMovement != 0;
        if (Stunned)
            Debug.Log("Horizontal Movement: " + horizontalMovement);
        if (!jumping)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalMovement));
        }
        if (isMoving)
        {
            horizontalDirection = Mathf.RoundToInt(horizontalMovement);

            Vector3 moveVector = Vector3.right * horizontalMovement * (walkSpeed * (Stunned ? stunDirectionModifier.x : 1)) * Time.deltaTime;
            if (!dying)
            {
                transform.localScale = new Vector3(-1 * horizontalMovement, 1, 1);
            }
            if (matrix.IsPositionWithinMatrix(Vector2Int.RoundToInt(transform.position + moveVector)))
            {
                transform.position += moveVector;
            }
        }

        float backRayDistance = .125f;
        float rayDistance = (isMoving ? .45f : backRayDistance);
        float widthOffset = (GetComponent<BoxCollider2D>().size.x / 2);
        Vector3 currentForwardDirection = Vector2.right * horizontalDirection;
        Vector3 currentBackwardDirection = Vector2.right * -horizontalDirection;

        //Forward checks
        RaycastHit2D[] frontWallRaycasts = new RaycastHit2D[]
        {
            Physics2D.Raycast(transform.position + (currentForwardDirection * .2f) + (Vector3.up *.05f), currentForwardDirection, rayDistance, terrainLayer),
            Physics2D.Raycast(transform.position + (currentForwardDirection * .2f) + (Vector3.up * 1f), currentForwardDirection, rayDistance, terrainLayer),
            Physics2D.Raycast(transform.position + (currentForwardDirection * .2f) + (Vector3.up * 1.7f), currentForwardDirection, rayDistance, terrainLayer)
        };
        //Backward checks
        RaycastHit2D[] backWallRaycasts = new RaycastHit2D[]
        {
            Physics2D.Raycast(transform.position + (currentBackwardDirection * .2f) + (Vector3.up *.05f), currentBackwardDirection, backRayDistance, terrainLayer),
            Physics2D.Raycast(transform.position + (currentBackwardDirection * .2f) + (Vector3.up * 1f), currentBackwardDirection, backRayDistance, terrainLayer),
            Physics2D.Raycast(transform.position + (currentBackwardDirection * .2f) + (Vector3.up * 1.7f), currentBackwardDirection, backRayDistance, terrainLayer)
        };
        #region Collision Debugs
        /*//Show Back Wall Collision Checks
        Debug.DrawLine(transform.position + (currentForwardDirection * .2f) + (Vector3.up * .05f), transform.position + (Vector3.up * .05f) + (currentForwardDirection * rayDistance), Color.red);
        Debug.DrawLine(transform.position + (currentForwardDirection * .2f) + (Vector3.up * 1f), transform.position + (Vector3.up * 1f) + (currentForwardDirection * rayDistance), Color.red);
        Debug.DrawLine(transform.position + (currentForwardDirection * .2f) + (Vector3.up * 1.7f), transform.position + (Vector3.up * 1.7f) + (currentForwardDirection * rayDistance), Color.red);
        *//*//Show Front Wall Collision Checks
        Debug.DrawLine(transform.position + (currentBackwardDirection * .2f) + (Vector3.up * .05f), transform.position + (Vector3.up * .05f) + (currentBackwardDirection * rayDistance), Color.red);
        Debug.DrawLine(transform.position + (currentBackwardDirection * .2f) + (Vector3.up * 1f), transform.position + (Vector3.up * 1f) + (currentBackwardDirection * rayDistance), Color.red);
        Debug.DrawLine(transform.position + (currentBackwardDirection * .2f) + (Vector3.up * 1.7f), transform.position + (Vector3.up * 1.7f) + (currentBackwardDirection * rayDistance), Color.red);
        */
        #endregion
        RaycastHit2D forwardHit = frontWallRaycasts.FirstOrDefault(ray => ray.collider != null);
        RaycastHit2D backwardHit = backWallRaycasts.FirstOrDefault(ray => ray.collider != null);
        Vector2 newPosition = transform.position;
        if (forwardHit)
            newPosition = new Vector2(forwardHit.point.x + (widthOffset + (isMoving? .125f: .06125f)) * (-horizontalDirection), transform.position.y);
        else if(backwardHit)
            newPosition = new Vector2(backwardHit.point.x + (widthOffset + (.06125f)) * (horizontalDirection), transform.position.y);
        transform.position = newPosition;
    }
    private void Jump()
    {
        if ((!waitingForCountdown && !Stunned && !dying) && input.IsButtonDown(PlayerButton.A) && !jumping)
        {
            OnJump();
            SoundController.instance.PlaySound("fastjump");
        }

        if (jumping)
        {
            transform.position += (Vector3.up * Time.deltaTime * moveUp * gravityScale) * Mathf.Pow(jumpSpeed * (Stunned ? stunDirectionModifier.y : 1), 2);
            moveUp += Time.deltaTime * GRAVITATIONALDIRECTION * jumpDecreaseSpeed;
            animator.SetFloat("VerticalVelocity", moveUp);
        }
    }
    private void OnJump()
    {
        jumping = true;
        animator.SetBool("IsJumping", jumping);
        moveUp = 1;
    }

    public void UponKillingBlow()
    {
        dying = true;
        LaunchAway(transform.position + (Vector3.right * horizontalDirection));
        animator.SetBool("IsDead", true);
    }

    public void OnDeath()
    {
        directorevent.Invoke();
        animator.SetBool("IsDead", false);
        dying = false;
    }

    private void Gravity()
    {
        Vector3 moveVector = Vector3.up * GRAVITATIONALDIRECTION * gravityDown * Time.deltaTime;
        float rayDistance = 1.15f;
        float widthOffset = (GetComponent<BoxCollider2D>().size.x / 2);
        RaycastHit2D[] floorRaycasts = new RaycastHit2D[]
        {
            Physics2D.Raycast(transform.position + (Vector3.up * .5f), Vector2.down, rayDistance, terrainLayer),
            Physics2D.Raycast(transform.position + (Vector3.up * .5f) - Vector3.right * (widthOffset*.9f), Vector2.down, rayDistance, terrainLayer),
            Physics2D.Raycast(transform.position + (Vector3.up * .5f) + Vector3.right * (widthOffset*.9f), Vector2.down, rayDistance, terrainLayer)
        };
        rayDistance = .125f;
        float verticalOffset = 1.75f;
        RaycastHit2D[] ceilingRaycasts = new RaycastHit2D[]
        {
            Physics2D.Raycast(transform.position + (Vector3.up * verticalOffset), Vector2.up, rayDistance, terrainLayer),
            Physics2D.Raycast(transform.position + (Vector3.up * verticalOffset) - Vector3.right * (widthOffset*.9f), Vector2.up, rayDistance, terrainLayer),
            Physics2D.Raycast(transform.position + (Vector3.up * verticalOffset) + Vector3.right * (widthOffset*.9f), Vector2.up, rayDistance, terrainLayer)
        };
        #region Debug Vertical Collisions
        /*//Show Floor Collision Checks
        Debug.DrawLine(transform.position + (Vector3.up * .5f), transform.position + (Vector3.up * .5f) + (Vector3.down * rayDistance), Color.red);
        Debug.DrawLine(transform.position + (Vector3.up * .5f) - Vector3.right * (widthOffset * .9f), transform.position + (Vector3.up * .5f) - Vector3.right * (widthOffset * .9f) + (Vector3.down * rayDistance), Color.red);
        Debug.DrawLine(transform.position + (Vector3.up * .5f) + Vector3.right * (widthOffset * .9f), transform.position + (Vector3.up * .5f) + Vector3.right * (widthOffset * .9f) + (Vector3.down * rayDistance), Color.red);
        *//*//Show Ceiling Collision Checks
        Debug.DrawLine(transform.position + (Vector3.up * verticalOffset), transform.position + (Vector3.up * verticalOffset) + (Vector3.up * rayDistance), Color.red);
        Debug.DrawLine(transform.position + (Vector3.up * verticalOffset) - Vector3.right * (widthOffset * .9f), transform.position + (Vector3.up * verticalOffset) - Vector3.right * (widthOffset * .9f) + (Vector3.up * rayDistance), Color.red);
        Debug.DrawLine(transform.position + (Vector3.up * verticalOffset) + Vector3.right * (widthOffset * .9f), transform.position + (Vector3.up * verticalOffset) + Vector3.right * (widthOffset * .9f) + (Vector3.up * rayDistance), Color.red);
        */
        #endregion
        RaycastHit2D hit = floorRaycasts.FirstOrDefault(ray => ray.collider != null);
        if (hit && moveUp < 0)
        {
            Vector2 newPosition = new Vector2(transform.position.x,
                hit.point.y);
            transform.position = newPosition;

            if (jumping)
            {
                SoundController.instance.PlaySound("footstep");

                if (dying)
                    animator.SetBool("IsDead", true);
                animator.SetTrigger("HasLanded");

                if (waitingForCountdown)
                    directorevent.Invoke();
            }
            Vector2Int[] flooredPositions = new Vector2Int[] {
            new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.FloorToInt(transform.position.y - .1f)),
            new Vector2Int(Mathf.RoundToInt(transform.position.x - widthOffset), Mathf.FloorToInt(transform.position.y - .1f)),
            new Vector2Int(Mathf.RoundToInt(transform.position.x + widthOffset), Mathf.FloorToInt(transform.position.y - .1f))
            };
            foreach (Vector2Int flooredPosition in flooredPositions)
            {
                if (matrix.IsPositionObstructed(flooredPosition))
                {
                    Transform gridNode = matrix.GetNodeFromGrid(flooredPosition);
                    if (gridNode && gridNode.GetComponent<Mino>().IsTrator)
                    {
                        transform.position -= Vector3.right * gridNode.GetComponent<Mino>().direction * Time.deltaTime;
                        break;
                    }
                }
            }

            jumping = false;
            moveUp = JUMPHEIGHTURNINGPOINT;
        }
        else if (floorRaycasts.All(ray => ray.collider == null))
        {
            jumping = true;
            if (moveUp < JUMPHEIGHTURNINGPOINT && jumping)
                transform.position += moveVector;
        }
        hit = ceilingRaycasts.FirstOrDefault(ray => ray.collider != null);
        if (hit)
        {
            if (verticalDirection > 0)
            {
                List<Vector3> headCollisionPositions = new List<Vector3>();
                headCollisionPositions.Add(transform.position + (Vector3.up * Mathf.CeilToInt(2.5f)));
                headCollisionPositions.Add(transform.position + (Vector3.right * (input.Horizontal != 0 ? horizontalDirection : 0)) + (Vector3.up * Mathf.CeilToInt(2.5f)));

                foreach (Vector3 position in headCollisionPositions)
                {
                    Vector2Int flooredPosition = (new Vector2Int(Mathf.RoundToInt(position.x), Mathf.FloorToInt(position.y)));
                    if (matrix.IsPositionObstructed(flooredPosition))
                    {
                        Transform gridNode = matrix.GetNodeFromGrid(flooredPosition);
                        matrix.DestroyBlock(Vector2Int.RoundToInt(gridNode.transform.position));
                        break;
                    }
                }

                moveUp = JUMPHEIGHTURNINGPOINT;
            }
        }
        animator.SetBool("IsJumping", jumping);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Invulnerable||dying)
            return;
        if (!Physics2D.IsTouching(GetComponent<Collider2D>(), collision))
            return;
        if (collision.tag == "Jewel")
        {
            ScoreController.instance.UpdateMultiplier(1);
            SoundController.instance.PlaySound("BonusMultiplier" + Random.Range(0, 2).ToString(), .5f);
            SoundController.instance.PlaySound("gotbonus");
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "Score")
        {
            ScoreController.instance.UpdateScore(500, collision.transform.position);
            SoundController.instance.PlaySound("ScoreBonus" + Random.Range(0, 2).ToString(), .5f);
            SoundController.instance.PlaySound("gotbonus");
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "Tetromino")
        {
            LaunchAway(transform.position + (Vector3.right * horizontalDirection), new Vector2(.75f, 1.25f));
        }
        else if (collision.tag == "Ghost")
        {
            isHitByGhost = true;
            GetComponent<Collider2D>().enabled = !isHitByGhost;
            UponKillingBlow();
        }
        else if (collision.tag == "Icicle")
        {
            LaunchAway(collision.transform.position, new Vector2(.75f, 1.25f));
        }
        else if (collision.tag == "Thwomp")
        {
            LaunchAway(collision.transform.position, new Vector2(1.15f, .85f));
        }
    }
}