using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelEntitySpawner : MonoBehaviour
{
    public float meterInterval = 10;
    [NonSerialized] public float heightLevel = 1;
    public GameObject[] objects;
    private Matrix matrix;

    void Start()
    {
        matrix = FindObjectOfType<Matrix>();
    }
    void Update()
    {
        if (ScoreController.instance.Level >= meterInterval * heightLevel)
            HeightReached();
    }
    void HeightReached()
    {
        //Debug.Log(ScoreController.instance.Level);
        heightLevel++;
        if (Random.Range(0f, 1f) < .125f)
        {
            bool randomSide = Random.Range(0f,1f) < .5f;
            matrix.SpawnObsticle(randomSide ? 0 : matrix.width - 1, Mino.MinoType.solid, randomSide ? 1 : -1);
        }
        else
            SpawnEntity();
    }
    void SpawnEntity()
    {
        GameObject gameObject = objects[Random.Range(0, objects.Length)];
        gameObject = Instantiate(gameObject, matrix.transform);
        gameObject.GetComponent<LevelEntity>().UponSpawn();
    }
}
