using System.Collections.Generic;
using UnityEngine;

public class BackgroundElementSpawner : MonoBehaviour
{
    //This script is for spawning animated sprites in the background
    //This randomizes the sprites on given PositionObjects you pipe in to the list
    //The script could have been done better by instead of adding objects to lsits for positions,
    //you have a ghroup object and just pipe in the children to the lists.
    //That way, its easier to add and remove positions without doing manual labour.

    #region Variables

    //Also you can specify the color and a random sorting order depending on a range you give
    public List<GameObject> templePrefabList = new List<GameObject>();
    public List<GameObject> templePositionList = new List<GameObject>();
    public Color templeColor;
    public Vector2 templeSortingOrderRange;

    //There are different similiar lists so the GameDesigner can add different assets 
    //to different parts of the background
    public List<GameObject> mountainPrefabList = new List<GameObject>();
    public List<GameObject> mountainPositionList = new List<GameObject>();
    public Color mountainColor;
    public Vector2 mountainSortingOrderRange;

    //Ditto
    public List<GameObject> sunsetPrefabList = new List<GameObject>();
    public List<GameObject> sunsetPositionList = new List<GameObject>();
    public Color sunsetColor;
    public Vector2 sunsetSortingOrderRange;

    //It also randomizes the scale randomzly between the range you specify
    public Vector2 sizeRange;

    #endregion Variables

    #region Start and Update

    private void Awake()
    {
        //These variables is used for readability in the script
        int templeListCount = templePositionList.Count;
        int mountainListCount = mountainPositionList.Count;
        int sunsetListCount = sunsetPositionList.Count;

        //Does checks so the lists arent empty before excecuting
        if (templePrefabList!=null&& templePositionList != null)
        {
            //Spawns as many object as there are positions to fill
            for(int i=0;i < templeListCount; i++)
            {
                LevelSpawn(templePrefabList, templePositionList, templeColor, sizeRange, templeSortingOrderRange);
            }
        }
        //Ditto
        if (mountainPrefabList != null && mountainPositionList != null)
        {
            for (int i = 0; i < mountainListCount; i++)
            {
                LevelSpawn(mountainPrefabList, mountainPositionList, mountainColor, sizeRange, mountainSortingOrderRange);
            }
        }
        //Ditto
        if (sunsetPrefabList != null && sunsetPositionList != null)
        {
            for (int i = 0; i < sunsetListCount; i++)
            {
                LevelSpawn(sunsetPrefabList, sunsetPositionList, sunsetColor, sizeRange, sunsetSortingOrderRange);
            }
        }
    }

    #endregion Start and Update

    //Method that spawn objects
    void LevelSpawn(List<GameObject> objectList, List<GameObject> positionList, Color color, Vector2 size, Vector2 sortingOrder)
    {
        //Starts by randomizing objects,positions, scale and sorting orders
        int randObjIndex = Random.Range(0, objectList.Count);
        int randPosIndex = Random.Range(0, positionList.Count);
        float randSize = Random.Range(size.x, size.y);
        int randSorting = Random.Range(Mathf.RoundToInt(sortingOrder.x), Mathf.RoundToInt(sortingOrder.y));

        //Instantiate an object and adds the info into a variable
        GameObject instance = Instantiate(objectList[randObjIndex], new Vector2(positionList[randPosIndex].transform.position.x,
                                                  positionList[randPosIndex].transform.position.y),
                                                  Quaternion.identity, gameObject.transform);

        //Used to turn the objects to the middle of the screen depending on which side they are
        if (positionList[randPosIndex].transform.position.x > 0)
        {
            instance.transform.localScale = new Vector3(-randSize, randSize, randSize);
        }
        else
        {
            instance.transform.localScale = new Vector3(randSize, randSize, randSize);
        }

        //Change the color of the sprite
        SpriteRenderer spriteRenderer = instance.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = randSorting;

        //Finally removes the used position from the list, so I can't get two objects on the same spot
        positionList.Remove(positionList[randPosIndex]);
    }
}
