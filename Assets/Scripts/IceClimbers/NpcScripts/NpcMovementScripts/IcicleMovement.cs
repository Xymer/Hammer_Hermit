using System.Collections.Generic;
using UnityEngine;

public class IcicleMovement : MonoBehaviour
{
    //Script  to drop the icicle from the ceiling if the player moves under,
    //or after a given time
    //This script is intended to be used on a spawner object
    ////This could've done better, by using a class that held the icicle information
    ////instead of using alot of lists
    ////but this is what i knew how to work with at the time

    #region Variables

    public GameObject player;
    public GameObject iciclePrefab;

    public Vector2 spawnPosRange;
    public float fallRange;
    public float icicleFallSpeed;
    public float icicleSpawnHeight;
    public float icicleDespawnPos;
    public float icicleMaxLifeTime;
    public bool isActive=false;

    float timer;
    public float spawnTimer;
    float animTimer;

    List<GameObject> icicleList = new List<GameObject>();
    List<bool> isFallingList = new List<bool>();
    List<IcicleCollision> icicleCollisionList = new List<IcicleCollision>();
    List<Animator> animatorList = new List<Animator>();
    List<float> lifeTimerList = new List<float>();

    #endregion Variables

    #region Start and Update

    void Update()
    {
        //isActive is used to make sure the icicles only spawn at the higher levels
        isActive = ScoreController.instance.Level > GameRules.instance.gameMode.maxLevel / 3;

        //Spawn the icicles
        IcicleCreation();
        IcicleMove();
    }

    #endregion Start and Update

    void IcicleCreation()
    {
        //Check to see if the icicle is on the right level before spawning
        if (isActive)
        {
            timer += Time.deltaTime;

            //Uses a timer to determine when to spawn the icicles
            if (timer >= spawnTimer)
            {
                //Default state of not falling
                bool isFalling = false;

                //Instance of the object
                GameObject icicleInstance = Instantiate(iciclePrefab, new Vector3(Random.Range(spawnPosRange.x, spawnPosRange.y),
                                                                                  icicleSpawnHeight, 0f), Quaternion.identity, transform);

                //default value of a timer to be used to determine when the icicle falls
                float lifeTime = 0;

                //Adding variables, object and animators to different lists to be accesed later on
                icicleList.Add(icicleInstance);
                icicleCollisionList.Add(icicleInstance.GetComponent<IcicleCollision>());
                animatorList.Add(icicleInstance.GetComponent<Animator>());
                isFallingList.Add(isFalling);
                lifeTimerList.Add(lifeTime);

                //Lastly, resets the timer so a new icicle can spawn at the next spawnTimer
                timer = 0f;
            }
        }
    }

    void IcicleMove()
    {
        //checks to see the icicle object list is not empty before moving the icicles
        if (icicleList != null)
        {
            //loop that goes through all icicle objects in the list
            for (int i = 0; i < icicleList.Count; i++)
            {
                //starts the counter of the icicle that determines how long it will be alive before falling
                lifeTimerList[i] += Time.deltaTime;

                //if the timer has exceeded the given time and while object isnt colliding with anything,
                //or if the object is within the range of being above the player
                //the icicle will fall
                if (lifeTimerList[i] > icicleMaxLifeTime && icicleCollisionList[i].isHit == false)
                {
                    isFallingList[i] = true;
                }
                else if ((player.transform.position.x >= icicleList[i].transform.position.x - fallRange) &&
                         (player.transform.position.x <= icicleList[i].transform.position.x + fallRange) &&
                         (icicleCollisionList[i].isHit == false))
                {
                    isFallingList[i] = true;
                }

                //if the icicle is determined to be falling, the aniamtion for that will play
                //and the icicle will move downwards
                if (isFallingList[i])
                {
                    animatorList[i].SetBool("IsFalling", true);
                    icicleList[i].transform.position = new Vector2(icicleList[i].transform.position.x,
                                                                   icicleList[i].transform.position.y - icicleFallSpeed);
                }

                //And if the icicle collides with anything it will stop falling and play the shatter animation
                //also starts an animation timer, which is used to compare with the end of the animation loop
                if (icicleCollisionList[i].isHit)
                {
                    animTimer += Time.deltaTime;
                    isFallingList[i] = false;
                    animatorList[i].SetTrigger("Shatter");
                }
                //when the icicle hit something, the icicle get destroyed
                //and all the variables gets deleted from their lists
                //and the animation timer resets
                if (icicleList[i].transform.position.y < icicleDespawnPos || (icicleCollisionList[i].isHit && animTimer > animatorList[i].GetCurrentAnimatorStateInfo(0).length))
                {
                    Destroy(icicleList[i]);
                    icicleList.Remove(icicleList[i]);
                    isFallingList.Remove(isFallingList[i]);
                    icicleCollisionList.Remove(icicleCollisionList[i]);
                    animatorList.Remove(animatorList[i]);
                    lifeTimerList.Remove(lifeTimerList[i]);
                    animTimer = 0f;
                }
            }
        }
    }
}