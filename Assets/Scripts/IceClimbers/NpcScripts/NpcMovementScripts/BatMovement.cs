using UnityEngine;

public class BatMovement : MonoBehaviour
{
    #region Variables and objects
    [SerializeField] private Transform target;
    [SerializeField, Range(0, 10)] private float moveSpeed = 2.5f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float radiusOffset = 2f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float cooldown = 1.25f;
    [SerializeField] private Vector3 offset = Vector3.zero;
    private bool hasHitPlayer = false;
    private bool hasSeenPlayer = false;
    private bool isAttacking = false;
    private bool isFlying = false;
    private bool isHitByHammer = false;
    private Animator batAnimator;
    private float distanceFromTarget;
    private float direction;
    private float tick;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Start()
    {
        batAnimator = GetComponent<Animator>();
        tick = cooldown;
    }
    private void Update()
    {
        GetDistanceFromPlayer();
        tick -= Time.deltaTime;
        if (tick <= 0)
        {
            hasHitPlayer = false;
            if (!hasHitPlayer)
            {
                this.GetComponent<Collider2D>().enabled = true;
            }
            tick = cooldown;
        }

        if (hasSeenPlayer && !isHitByHammer)
        {
            batAnimator.SetBool("HasSeenPlayer", hasSeenPlayer);
            if (!hasHitPlayer)
            {
                MoveBat();
            }
            BatAttack();
        }

        if (isHitByHammer)
        {
            BatFalling();
        }
        HaveBatSeenPlayer();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !target.GetComponent<IceClimberMovement>().Invulnerable)
        {
            target.GetComponent<IceClimberMovement>().LaunchAway(target.position);
            hasHitPlayer = true;
            SoundController.instance.PlaySound("batflap2");
            if (hasHitPlayer && !isHitByHammer)
            {
                this.GetComponent<Collider2D>().enabled = false;
            }
        }
        else if (collision.CompareTag("Hammer"))
        {
            GetComponent<CircleCollider2D>().enabled = false;

            batAnimator.SetTrigger("IsHitByHammer");
            if (!isHitByHammer)
            {
                ScoreController.instance.UpdateScore(250, transform.position);
            }
            isHitByHammer = true;
            Destroy(this.gameObject, 2);
            SoundController.instance.PlaySound("batdeath");
            SoundController.instance.PlaySound("batbattered", .5f);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endregion

    #region My Methods
    private void GetDistanceFromPlayer()
    {
        distanceFromTarget = Vector3.Distance(target.position, (transform.position));
        direction = target.TransformDirection(target.position - transform.position).x;
    }
    private void BatFalling()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y - moveSpeed * Time.deltaTime);
        GetComponent<CircleCollider2D>().enabled = false;
    }
    private void MoveBat()
    {
        isFlying = true;
        batAnimator.SetBool("IsFlying", isFlying);
        if (direction != 0)
        {
            transform.localScale = new Vector3(One(-direction), 1, 1);
        }
        transform.position = Vector2.MoveTowards(transform.position, target.position + offset, moveSpeed * Time.deltaTime);
    }
    private int One(float one)
    {
        if (one < 0)
        {
            return -1;
        }
        else if (one > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    private void HaveBatSeenPlayer()
    {
        if (!hasSeenPlayer && distanceFromTarget <= radius + radiusOffset)
        {
            hasSeenPlayer = true;
            SoundController.instance.PlaySound("batalarm", .5f);
        }
    }
    private void BatAttack()
    {
        if (distanceFromTarget < attackRange)
        {
            isAttacking = true;

        }
        else if (distanceFromTarget > attackRange)
        {
            isAttacking = false;
        }
        batAnimator.SetBool("IsAttacking", isAttacking);
    }

    #endregion
}
