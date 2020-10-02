using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    #region Variables

    float spawnPositionX = 20f;
    [SerializeField]float moveSpeed = 0.01f;

    int spawnDirection;
    float randPosY;

    #endregion Variables

    #region Start and Update

    private void Start()
    {
        //When the ghost spawns, it creates a sound
        GhostSpawn();
        SoundController.instance.PlaySound("ghostalarm" + (Random.Range(0f,1f) < .5f? "":"1"), .25f);
    }
    void Update()
    {
        GhostMove();
    }

    #endregion Start and Update

    void GhostSpawn()
    {
        //Gives a random Y spawnPosition
        //this should have been piped into the method so it can be controlled from unity
        randPosY = Random.Range(-6.5f, 6f);

        //determines the direction the ghost should move depending on where on the screen the ghost is
        //and then moves it into that direction
        if (transform.position.x>0)
        {
            spawnDirection = -1;
        }
        else
        {
            spawnDirection = 1;
        }
        transform.position = new Vector3(transform.position.x, randPosY, transform.position.z);
        transform.localScale = new Vector3(transform.localScale.x * spawnDirection, 1f, 1f);

    }
    void GhostMove()
    {
        //Moves the ghost depending on the speed and direction
        transform.position = new Vector3(transform.position.x + (moveSpeed * spawnDirection) * Time.deltaTime,transform.position.y);

        //If the ghost reaches either side of the screen, it will be destroyed.
        if ((transform.position.x > spawnPositionX * -spawnDirection && transform.position.x > spawnPositionX * spawnDirection) ||
            (transform.position.x < spawnPositionX * spawnDirection && transform.position.x < spawnPositionX * -spawnDirection))
        {
            Destroy(gameObject);
        }
    }
}
