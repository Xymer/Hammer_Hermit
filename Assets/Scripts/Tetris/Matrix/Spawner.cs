using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<Tetromino> tetrominoTypes;

    List<Tetromino> queue = new List<Tetromino>();

    List<Tetromino> bag = new List<Tetromino>();

    List<Tetromino> history = new List<Tetromino>();

    List<Tetromino> drought;

    [SerializeField] Matrix matrix = null;

    Dictionary<Mino.MinoType, int> lastTypes = new Dictionary<Mino.MinoType, int>();

    int solidCheck = 0;
    Dictionary<Tetromino.Type, int> count = new Dictionary<Tetromino.Type, int>();

    private void Awake()
    {
        lastTypes.Add(Mino.MinoType.breakable, 3);

        foreach (var tetromino in tetrominoTypes)
        {
            tetromino.matrix = matrix;
        }
        InstantiateBag();
    }

    /// <summary>
    /// Instaniate variables and pre-game necessities such as the first tetromino and the queue. As well as resets the game in some way.
    /// </summary>
    public void InstantiateBag()
    {
        CleanQueue();

        drought = tetrominoTypes;
        bag = new List<Tetromino>();
        history = new List<Tetromino>();
        count = new Dictionary<Tetromino.Type, int>();
        queue = new List<Tetromino>();
        foreach (var tetromino in tetrominoTypes)
        {
            count.Add(tetromino.type, 0);
            for (int i = 0; i < 5; i++)
            {
                bag.Add(tetromino);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            history.Add(tetrominoTypes[2]);
            history.Add(tetrominoTypes[6]);
        }

        List<Tetromino> firstSelections = new List<Tetromino>();

        for (int i = 0; i < tetrominoTypes.Count; i++)
        {
            if (tetrominoTypes[i].type != Tetromino.Type.S && tetrominoTypes[i].type != Tetromino.Type.Z && tetrominoTypes[i].type != Tetromino.Type.O)
            {
                firstSelections.Add(tetrominoTypes[i]);
            }
        }

        var first = Random.Range(0, 4);
        Mino.MinoType newType = GetNewTetrominoType();
        Tetromino tf = Instantiate(firstSelections[first], new Vector3(9999, 9999), Quaternion.identity);
        foreach (Transform mino in tf.transform)
        {
            mino.GetComponent<Mino>().SetType(newType);
        }
        queue.Add(tf);

        while (queue.Count <= 2)
        {
            newType = GetNewTetrominoType();
            Tetromino t = Instantiate(NextTetromino(), new Vector3(9999, 9999), Quaternion.identity);
            foreach (Transform mino in t.transform)
            {
                mino.GetComponent<Mino>().SetType(newType);
            }
            queue.Add(t);
        }
    }

    private Mino.MinoType GetNewTetrominoType()
    {
        Mino.MinoType tetrominoBlockType = Mino.MinoType.solid;

        if (GameRules.instance.rules.blockDifficulty >= 7)
        {
            return Mino.MinoType.star;
        }

        int lowestKeyValue = int.MaxValue;
        Mino.MinoType value = Mino.MinoType.solid;

        foreach (var item in lastTypes)
        {
            if (item.Value < lowestKeyValue)
            {
                value = item.Key;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            tetrominoBlockType = (Mino.MinoType)Random.Range(0, GameRules.instance.rules.blockDifficulty);

            if (solidCheck > 3)
            {
                if (tetrominoBlockType == Mino.MinoType.solid)
                {
                    solidCheck = 0;
                    return tetrominoBlockType;
                }
            }
            else
            {
                solidCheck++;
            }

            if (!lastTypes.ContainsKey(tetrominoBlockType))
            {
                lastTypes.Add(tetrominoBlockType, 0);
            }
            else
            {
                lastTypes[tetrominoBlockType]++;
            }
            if (tetrominoBlockType == value)
            {
                break;
            }
        }

        if (lastTypes.ContainsKey(tetrominoBlockType))
        {
            lastTypes.Remove(tetrominoBlockType);
        }

        return tetrominoBlockType;
    }

    /// <summary>
    /// Get the next tetromino in the bag through this complicated yet amazing randomizer.
    /// </summary>
    Tetromino NextTetromino()
    {
        Tetromino selected = tetrominoTypes[0];

        for (int i = 0; i < 6; i++)
        {
            int r = Random.Range(0, bag.Count);
            selected = bag[r];
            if (!history.Contains(selected))
            {
                count[selected.type]++;
                if (!(selected == drought[0]) && i > 0 && !count.TryGetValue(0, out _))
                {
                    bag[r] = drought[0];
                }
                drought.Remove(selected);
                drought.Add(selected);
                history.RemoveAt(0);
                history.Add(selected);
                break;
            }
            if (i < 5)
            {
                bag[r] = drought[0];
            }
        }

        return selected;
    }

    /// <summary>
    /// Get the next tetromino in the queue.
    /// </summary>
    public Tetromino NextInQueue()
    {
        Tetromino selected;

        selected = Instantiate(queue[0], transform.position, Quaternion.identity, matrix.transform);

        Destroy(queue[0].gameObject);

        queue.RemoveAt(0);

        Tetromino t = Instantiate(NextTetromino(), new Vector3(9999, 9999), Quaternion.identity);
        var newType = GetNewTetrominoType();
        foreach (Transform mino in t.transform)
        {
            mino.GetComponent<Mino>().SetType(newType);
        }
        queue.Add(t);

        return selected;
    }

    /// <summary>
    /// Don't call this twice in the same frame or I swear to god. Changes the queue positions.
    /// </summary>
    public void UpdateQueue()
    {
        GetComponentInChildren<Queue>().UpdateQueue(queue);
    }
    public void CleanQueue()
    {
        for(int i = queue.Count - 1; i >= 0; i--)
        {
            Destroy(queue[i].gameObject);
            queue.Remove(queue[i]);
        }

    }
}
