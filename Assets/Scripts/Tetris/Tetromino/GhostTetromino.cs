using UnityEngine;

public class GhostTetromino : MonoBehaviour
{
    public bool ghostEnabled;
    Tetromino ghost;

    /// <summary>
    /// Instaniate the piece highlight.
    /// </summary>
    public void CreateGhost(Tetromino tetromino)
    {
        if (!tetromino || !ghostEnabled)
        {
            return;
        }
        ghost = Instantiate(tetromino, tetromino.transform.position, Quaternion.identity);
        foreach (Transform mino in ghost.transform)
        {
            mino.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);
        }
    }

    /// <summary>
    /// Destroy the piece highlight.
    /// </summary>
    public void DestroyGhost()
    {
        if (!ghost)
        {
            return;
        }
        Destroy(ghost.gameObject);
    }

    /// <summary>
    /// Change the position of the piece highlight.
    /// </summary>
    public void UpdateGhost(Tetromino tetromino)
    {
        if (!tetromino || !ghostEnabled || !ghost)
        {
            return;
        }

        ghost.transform.position = tetromino.transform.position;
        ghost.transform.rotation = tetromino.transform.rotation;
        foreach (Transform mino in ghost.transform)
        {
            mino.rotation = tetromino.transform.GetChild(0).rotation;
        }

        ghost.Drop();
    }
}
