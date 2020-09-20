using System.Collections.Generic;
using UnityEngine;

public class PlayerHammer : MonoBehaviour
{
    #region Variables
    [SerializeField] Matrix matrix;
    [SerializeField] private int hammerOffset = 1;
    [SerializeField] private float activeHitboxTime;
    [SerializeField] private float activeHitboxDuration = 0.25f;
  
    private Vector2Int playerPosition;
   
    private int horizontalDirection = -1;

    Animator animator;
    IceClimberMovement playerMovement;
    #endregion
    #region Unity Methods
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<IceClimberMovement>();
        activeHitboxTime = activeHitboxDuration;
    }
    private void Update()
    {        
        if (playerMovement.input.Horizontal != 0)
        {
            horizontalDirection = Mathf.RoundToInt(playerMovement.input.Horizontal);
        }
        if (playerMovement.input.IsButtonDown(PlayerButton.B) && !playerMovement.Stunned && !playerMovement.dying && !GameDirector.disableControls)
        {
            animator.SetTrigger("IsAttacking");
            HitHammer();
        }
        if (activeHitboxTime < 0)
            transform.GetChild(0).gameObject.SetActive(false);
        else
            activeHitboxTime -= Time.deltaTime;
    }
    #endregion
    #region My Methods
    private void HitHammer()
    {
        List<Vector2Int> hammerCollisionPositions = new List<Vector2Int>();
        playerPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.FloorToInt(transform.position.y) + 1);
        hammerCollisionPositions.Add(new Vector2Int(Mathf.RoundToInt(playerPosition.x) + horizontalDirection, Mathf.FloorToInt(playerPosition.y) + hammerOffset));
        hammerCollisionPositions.Add(new Vector2Int(Mathf.RoundToInt(playerPosition.x) + horizontalDirection, Mathf.FloorToInt(playerPosition.y)));
        bool hitAnything = false;
        foreach (Vector2Int position in hammerCollisionPositions)
        {
            if (matrix.IsPositionObstructed(position))
            {
                Transform mino = matrix.GetNodeFromGrid(position);
                if (mino && mino.GetComponent<Mino>().IsWalkable)
                {
                    matrix.DestroyBlock(position);
                    if (mino.GetComponent<Mino>().IsBreakable)
                    {
                        hitAnything = true;
                        break;
                    }
                }
            }   
        }
        if (!hitAnything)
            SoundController.instance.PlaySound("emptyswing");
        transform.GetChild(0).gameObject.SetActive(true);
        activeHitboxTime = activeHitboxDuration;
    }
    #endregion
}

