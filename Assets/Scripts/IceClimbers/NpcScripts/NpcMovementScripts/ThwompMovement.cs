using System.Collections.Generic;
using UnityEngine;

public class ThwompMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Transform target;
    [SerializeField, Range(0, 20)] private float moveSpeed = 2f;
    [SerializeField, Range(0, 20)] private float returnSpeed = 1f;
    [SerializeField, Range(0, 10)] private float cooldownDuration = 5f; 
    [SerializeField] private float xGridOffset = 11f;
    [SerializeField] private Matrix matrix;

    private float startOffset;
    private float cooldownTime;
    private Vector2Int predictedPosition;
    private Vector2 startSide = Vector2.zero;

    private bool isAttacking = false;
    private bool isReturning = false;

    private Animator thwompAnimator;

    private bool OnCooldown => Time.time <= cooldownTime;

    #endregion
    #region Unity methods
    private void Awake()
    {
        thwompAnimator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        matrix = GameObject.FindGameObjectWithTag("Matrix").GetComponent<Matrix>();
    }
    private void Start()
    {
        startPosition = transform.position;
        if (startPosition.x > 9)
        {
            xGridOffset = -xGridOffset;
            startSide = Vector2.right;
        }
        else
        {
            startSide = Vector2.left;
        }
        startOffset = xGridOffset;
    }
    private void Update()
    {
        PredictPosition();
        if (IsPlayerInHeight())
        {
            OnThompAttack();
        }
        ThwompAnimator();
        ThwompAttack();
        if (BackAtStart())
        {
            isReturning = false;
            xGridOffset = startOffset;
        }
        BreakableInPath();
        UnbreakableInPath();
    }
    #endregion
    #region My methods
    private void OnThompAttack()
    {
        if (OnCooldown)
            return;
        SoundController.instance.PlaySound("trapalarm", 0.5f);
        isAttacking = true;
        cooldownTime = cooldownDuration + Time.time;
    }

    private void ThwompAnimator()
    {
        thwompAnimator.SetBool("IsAttacking", isAttacking);
        thwompAnimator.SetBool("HasReturned", BackAtStart());
    }
    private void ThwompAttack()
    {
        if (isAttacking && !isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(xGridOffset, transform.position.y), moveSpeed * Time.deltaTime);
            if (transform.position.x >= xGridOffset && startSide == Vector2.left)
            {
                isAttacking = false;
                isReturning = true;
            }
            else if (transform.position.x <= xGridOffset && startSide == Vector2.right)
            {
                isAttacking = false;
                isReturning = true;
            }
            if (!isAttacking)
                SoundController.instance.PlaySound("trapthump" + (Random.Range(0f, 1f) < .5f ? "" : "1"));
        }
        else if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(startPosition.x, transform.position.y), returnSpeed * Time.deltaTime);
        }
    }
    private bool IsPlayerInHeight()
    {
        if (!isAttacking && !isReturning)
        {
            if (Mathf.FloorToInt(transform.position.y) == Mathf.FloorToInt(target.position.y + 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private bool BackAtStart()
    {
        if (transform.position.x == startPosition.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void BreakableInPath()
    {
        if (!isAttacking)
            return;
        List<Vector2Int> thwompCollisionPositions = new List<Vector2Int>();
        thwompCollisionPositions.Add(new Vector2Int(Mathf.RoundToInt(predictedPosition.x), Mathf.RoundToInt(transform.position.y + GetComponent<SpriteRenderer>().bounds.extents.y)));
        thwompCollisionPositions.Add(new Vector2Int(Mathf.RoundToInt(predictedPosition.x), Mathf.RoundToInt(transform.position.y - GetComponent<SpriteRenderer>().bounds.extents.y)));
        thwompCollisionPositions.Add(new Vector2Int(Mathf.RoundToInt(predictedPosition.x), Mathf.RoundToInt(transform.position.y)));
        foreach (Vector2Int position in thwompCollisionPositions)
        {
            if (matrix.IsPositionObstructed(position))
            {
                Transform mino = matrix.GetNodeFromGrid(position);
                if (mino && mino.GetComponent<Mino>().IsBreakable && !isReturning)
                {
                    if (startSide == Vector2.left)
                    {
                        matrix.DestroyBlock(position);
                    }
                    if (startSide == Vector2Int.right)
                    {
                        matrix.DestroyBlock(position);
                    }

                }
            }

        }
    }
    private void UnbreakableInPath()
    {
        List<Vector2Int> thwompCollisionPositions = new List<Vector2Int>();
        thwompCollisionPositions.Add(new Vector2Int(Mathf.RoundToInt(predictedPosition.x), Mathf.RoundToInt(transform.position.y + GetComponent<SpriteRenderer>().bounds.extents.y)));
        thwompCollisionPositions.Add(new Vector2Int(Mathf.RoundToInt(predictedPosition.x), Mathf.RoundToInt(transform.position.y - GetComponent<SpriteRenderer>().bounds.extents.y)));
        thwompCollisionPositions.Add(new Vector2Int(Mathf.RoundToInt(predictedPosition.x), Mathf.RoundToInt(transform.position.y)));
        foreach (Vector2Int position in thwompCollisionPositions)
        {
            if (matrix.IsPositionObstructed(position))
            {
                Transform mino = matrix.GetNodeFromGrid(position);
                Vector2 newPosition = new Vector2(transform.position.x, transform.position.y);
                if (mino && ((!mino.GetComponent<Mino>().IsBreakable || mino.GetComponent<Mino>().IsFalling) && mino.GetComponent<Mino>().IsWalkable))
                {
                    if (startSide == Vector2.left && !isReturning)
                    {
                        xGridOffset = predictedPosition.x - GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
                        transform.position = newPosition;
                        KillPlayerOnObstruced(position);
                    }
                    else if (startSide == Vector2Int.right && !isReturning)
                    {
                        xGridOffset = predictedPosition.x + GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
                        transform.position = newPosition;
                        KillPlayerOnObstruced(position);
                    }
                }
            }

        }
    }
    private void KillPlayerOnObstruced(Vector2Int position)
    {
        if (Mathf.RoundToInt(target.position.x) == Mathf.RoundToInt(transform.position.x) && Mathf.RoundToInt(target.position.y) == Mathf.RoundToInt(position.y)
            || Mathf.RoundToInt(target.position.x) == Mathf.RoundToInt(transform.position.x) && Mathf.RoundToInt(target.position.y + 1) == Mathf.RoundToInt(position.y))
        {
            target.GetComponent<IceClimberMovement>().UponKillingBlow();
        }
    }
    private void PredictPosition()
    {
        if (startSide == Vector2.left)
        {
            predictedPosition = new Vector2Int(Mathf.CeilToInt(transform.position.x + 1), Mathf.CeilToInt(transform.position.y));
        }
        else if (startSide == Vector2.right)
        {
            predictedPosition = new Vector2Int(Mathf.CeilToInt(transform.position.x - 1), Mathf.CeilToInt(transform.position.y));
        }
    }
    #endregion
}
