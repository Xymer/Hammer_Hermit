using UnityEngine;

public class Mino : MonoBehaviour
{

    Matrix matrix;
    SpriteRenderer sprite;
    Animator tratorAnimation;
    float speed = 0;
    float gravity = 0.015f;
    float maxSpeed = 0.30f;
    int currentY;
    public bool falling = false;
    public int direction;

    private void Update()
    {
        if (matrix)
        {
            return;
        }
        if (transform.parent.TryGetComponent(out Tetromino tetromino))
        {
            if (tetromino.matrix)
            {
                matrix = tetromino.matrix;
            }
        }
    }

    //You will never reach the star tile Diavolo.
    public enum MinoType
    {
        solid,
        breakable,
        cloud,
        fragile,
        falling,
        trator,
        star
    }

    public MinoType type;

    //Now that's a lot of properties I could probably have made easier but hey.
    public bool IsBreakable { get { return type == MinoType.breakable || type == MinoType.fragile || type == MinoType.star; } }

    public bool IsFragile { get { return type == MinoType.fragile; } }

    public bool IsWalkable { get { return type != MinoType.cloud; } }

    public bool IsFalling { get { return type == MinoType.falling; } }

    public bool IsObsticle;

    public bool IsTrator { get { return type == MinoType.trator; } }
    public void SetType(MinoType type)
    {
        if (!sprite)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        this.type = type;

        if (!tratorAnimation && IsTrator)
        {
            tratorAnimation = GetComponent<Animator>();
        }

        if (IsTrator)
        {
            if (tratorAnimation)
            {
                tratorAnimation.enabled = true;
                direction = -1;
                tratorAnimation.SetFloat("direction", direction);
            }
            tratorAnimation.runtimeAnimatorController = GameRules.instance.animatedTile;
        }
        else
        {
            if (tratorAnimation)
            {
                tratorAnimation.enabled = false;
            }
            sprite.sprite = GameRules.instance.blockTypes[(int)type];
        }

        if (!IsWalkable)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
        else
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
    }

    /// <summary>
    /// Sets the mino type as an obsticle
    /// </summary>
    public void SetType(int direction)
    {
        if (!sprite)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        type = MinoType.solid;
        IsObsticle = true;
        sprite.sprite = GameRules.instance.obsticleTypes[Mathf.Clamp(direction, 0, 1)];
    }

    /// <summary>
    /// What happens when this mino is deth.
    /// </summary>
    public void UponDestroy()
    {
        if (type == MinoType.star)
        {
            ScoreController.instance.UpdateScore(50, transform.position);
        }
        else
        {
            ScoreController.instance.UpdateScore(10, transform.position);
        }
        if (IsFragile)
            SoundController.instance.PlaySound("ice", .25f);
        else if (IsBreakable)
            SoundController.instance.PlaySound("softdig", .75f);
        Destroy(Instantiate(matrix.breakParticle.gameObject, gameObject.transform.position, Quaternion.identity), matrix.breakParticle.main.duration);
        Destroy(gameObject);
    }

    /// <summary>
    /// Flips the direction of the conveyer belt block thing.
    /// </summary>
    public void ReverseTractorBlock()
    {
        if (!tratorAnimation)
        {
            tratorAnimation = GetComponent<Animator>();
        }

        direction = direction * -1;

        tratorAnimation.SetFloat("direction", direction);
    }

    //Falling block updates here!
    private void FixedUpdate()
    {
        if (falling)
        {
            speed += gravity;
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            transform.Translate(Vector2.down * speed);

            if (Mathf.FloorToInt(transform.position.y) != currentY)
            {
                currentY = Mathf.FloorToInt(transform.position.y);

                if (matrix.IsPositionObstructedNoMatrix(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.FloorToInt(transform.position.y))))
                {
                    transform.position = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.CeilToInt(transform.position.y), 0);
                    matrix.PlaceBlock(Vector2Int.RoundToInt(transform.position), gameObject);
                    falling = false;
                }
                if (matrix.IsPositionBellowMatrix(Vector2Int.CeilToInt(transform.position)))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
