using System.Collections.Generic;
using UnityEngine;

public class cloudRandomizer : MonoBehaviour
{
    //This script creates fog in the temple level and clouds in the mountains
    //It also randomizes sorting orders to give depth

    #region Variables

    public GameObject[] cloudObject;
    public int cloudCount;

    public Vector2 cloudScaleRange=new Vector2(1f,3f);
    public Vector2 cloudPosXRange = new Vector2(-20f, 20f);
    public Vector2 cloudPosYRange = new Vector2(43f, 96f);
    public Vector2 cloudPosZRange = new Vector2(1f, 10f);
    public Vector2Int cloudSortingRange = new Vector2Int(-21, -34);

    public float cloudSpeedMultiplier;

    public GameObject[] fogObject;
    public int fogCount;

    public Vector2 fogScaleRange=new Vector2(1f,2f);
    public Vector2 fogPosXRange = new Vector2(-20f, 20f);
    public Vector2 fogPosYRange= new Vector2(-10f, 30f);
    public Vector2 fogPosZRange=new Vector2(15f,33f);
    public Vector2Int fogSortingRange= new Vector2Int(-9, -24);

    public float fogSpeedMultiplier = 0.25f;

    List<GameObject> cloudList = new List<GameObject>();
    List<GameObject> fogList = new List<GameObject>();

    public float posCutOff = 25f;
    public int direction = 1;

    #endregion Variables

    #region Start and Update

    private void Start()
    {
        CreateCloud(cloudObject, cloudScaleRange, cloudPosXRange, cloudPosYRange, cloudPosZRange, cloudSortingRange,cloudList);
        CreateCloud(fogObject, fogScaleRange, fogPosXRange, fogPosYRange, fogPosZRange, fogSortingRange, fogList);
    }

    private void Update()
    {
        CloudMove(cloudList,posCutOff,direction, cloudSpeedMultiplier*0.001f);
        CloudMove(fogList,posCutOff,direction,fogSpeedMultiplier*0.001f);
    }

    #endregion Start and Update

    void CreateCloud(GameObject[] npc,Vector2 scale, Vector2 posX, Vector2 posY, Vector2 posZ,Vector2Int sorting,List<GameObject> list)
    {
        for (int i = 0; i < fogCount; i++)
        {
            //Starts by randomizing positions, scale and sortingOrder
            float randScale = Random.Range(scale.x, scale.y);
            float randPosX = Random.Range(posX.x, posX.y);
            float randPosY = Random.Range(posY.x, posY.y);
            float randPosZ = Random.Range(posZ.x, posZ.y);
            float randSorting = Random.Range(sorting.x, sorting.y);

            //Instantiate the object as a child of the spawner
            GameObject instance = Instantiate(npc[Random.Range(0, npc.Length)], new Vector3(randPosX, randPosY, randPosZ), Quaternion.identity, gameObject.transform);
            instance.transform.localScale = new Vector3(randScale, randScale, randScale);

            //Gives a sorting order to the object
            SpriteRenderer renderer = instance.GetComponent<SpriteRenderer>();
            renderer.sortingOrder = (int)randSorting;

            //Ends by adding the object to a list which I use to move the the obejcts in the CloudMove Method
            list.Add(instance);
        }
    }

    void CloudMove(List<GameObject> list, float cutOff,int direction, float speedMultiplier)
    {
        //Goes through all objects in the list and moves them one at a time
        for (int i = 0; i < list.Count; i++)
        {
            //Uses the objects Z position to determine how fast the object should move
            float moveSpeed = list[i].transform.position.z * speedMultiplier;

            //Moves the object by adding the moveSPeed and a speed multiplier
            list[i].transform.position = new Vector3(list[i].transform.position.x + (moveSpeed * direction * Time.deltaTime), list[i].transform.position.y, list[i].transform.position.z);

            //If the object reaches the cutoff position, it moves back to the inverted position and starts over
            if (list[i].transform.position.x > cutOff)
            {
                list[i].transform.position = new Vector3(-list[i].transform.position.x, list[i].transform.position.y, list[i].transform.position.z);
            }
        }
    }
}
