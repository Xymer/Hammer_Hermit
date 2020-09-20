using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoStorageBoxDisplay : MonoBehaviour
{
    /// <summary>
    /// Gizmos are fun.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + new Vector3(-1.5f * transform.localScale.x, -1.5f * transform.localScale.y), (transform.position + new Vector3(2.5f * transform.localScale.x, -1.5f * transform.localScale.y)));
        Gizmos.DrawLine(transform.position + new Vector3(-1.5f * transform.localScale.x, 0.5f * transform.localScale.y), transform.position + new Vector3(2.5f * transform.localScale.x, 0.5f * transform.localScale.y));
        Gizmos.DrawLine(transform.position + new Vector3(-1.5f * transform.localScale.x, -1.5f * transform.localScale.y), transform.position + new Vector3(-1.5f * transform.localScale.x, 0.5f * transform.localScale.y));
        Gizmos.DrawLine(transform.position + new Vector3(2.5f * transform.localScale.x, -1.5f * transform.localScale.y), transform.position + new Vector3(2.5f * transform.localScale.x, 0.5f * transform.localScale.y));
    }
}
