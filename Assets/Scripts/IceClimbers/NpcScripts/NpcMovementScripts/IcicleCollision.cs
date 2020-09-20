using UnityEngine;
using UnityEngine.Assertions;

public class IcicleCollision : MonoBehaviour
{
    #region Variables

    public bool isHit=false;
    public Matrix matrix;

    #endregion Variables

    #region Start and Update

    private void Start()
    {
        matrix = GameObject.FindGameObjectWithTag("Matrix").GetComponent<Matrix>();
        Assert.IsNotNull(matrix, "No GameObject with that Matrix found");
    }

    private void Update()
    {
        //Checks to see if the icicle hits anything unwalkable in the matrix grid
        //If so, changes isHit to true, to be used elsewhere, and turns off the collider
        if (matrix.IsPositionObstructed(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y-0.85f))))
        {
            isHit = true;
            GetComponent<PolygonCollider2D>().enabled = false;
        }
    }

    #endregion Start and Update

    //If the icicle hits a trigger, change the state of isHit to true
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isHit = true;
    }
}
