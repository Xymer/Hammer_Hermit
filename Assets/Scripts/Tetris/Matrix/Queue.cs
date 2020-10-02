using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Queue : MonoBehaviour
{
    [SerializeField] List<GameObject> nextQueue = new List<GameObject>();

    /// <summary>
    /// Gets the current queue tetrominos and places them in these boxes neatly.
    /// </summary>
    /// <param name="queue"></param>
    public void UpdateQueue(List<Tetromino> queue)
    {
        if (nextQueue[0].transform.childCount > 0)
        {
            for (int i = 0; i < nextQueue.Count; i++)
            {
                Destroy(nextQueue[i].transform.GetChild(0).gameObject);
            }
        }
        for (int i = 0; i < nextQueue.Count; i++)
        {
            queue[i].transform.position = nextQueue[i].transform.position;
            queue[i].transform.localScale = nextQueue[i].transform.localScale;
        }
    }

    public void CleanQueue()
    {
        if (nextQueue[0].transform.childCount > 0)
        {
            for (int i = 0; i < nextQueue.Count; i++)
            {
                Destroy(nextQueue[i].transform.GetChild(0).gameObject);
            }
        }
    }
}
