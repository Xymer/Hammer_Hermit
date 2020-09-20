using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntity : MonoBehaviour
{
    [SerializeField] public float horizontalSpawnPosition = 0;

    private const float verticalSpawnPosition = 22;

    [SerializeField] private float horizontalSpawnOffset = 0;

    public void UponSpawn()
    {
        float horizontalPosition = Random.Range(0f,1f) < .5f ? -horizontalSpawnPosition : horizontalSpawnPosition;
        horizontalPosition += 9;
        transform.localPosition = new Vector3(
            Random.Range(horizontalPosition - horizontalSpawnOffset, horizontalPosition + horizontalSpawnOffset),
            verticalSpawnPosition, transform.localPosition.z);
    }
    public void MoveDownwards(int units)
    {
        transform.Translate(Vector3.down * units);
    }
}
