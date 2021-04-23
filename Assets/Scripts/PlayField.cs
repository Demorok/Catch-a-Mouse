using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    [SerializeField] int height;
    [SerializeField] int width;

    Chip[,] chips;
    Vector3[,] chipPositions;

    BoxCollider2D fieldFrame;

    // Start is called before the first frame update
    void Start()
    {
        fieldFrame = GetComponent<BoxCollider2D>();
        Initialise_Field();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Initialise_Field()
    {
        fieldFrame.size = new Vector2(width, height);
        gameObject.transform.localScale = new Vector3(height, width, 1);

        int[] center = new int[] { Mathf.CeilToInt(height / 2), Mathf.CeilToInt(width / 2) };
        int[] empty = new int[] { height - 1, width - 2 };
        int[,] mouseSpawn = new int[,]
        {
            {0, 0},
            {0, width-1},
            {height-1, 0},
            {height-1, width-1}
        };
        int[,] catSpawn = new int[,]
        {
            {center[0]-1, center[1]-1},
            {center[0]-1, center[1]+1},
            {center[0]+1, center[1]-1},
            {center[0]+1, center[1]+1},
        };

        chips = new Chip[height, width];
        chipPositions = new Vector3[height, width];

        Collider2D coll;
        GameObject clone;
        for (int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if (i == 0 && j == 0)
                    chipPositions[i, j] = new Vector3(-fieldFrame.bounds.extents.x, fieldFrame.bounds.extents.y, fieldFrame.bounds.extents.z);
                else if (j == 0)
                {
                    coll = chips[i - 1, j].GetComponent<Collider2D>();
                    chipPositions[i, j] = coll.bounds.min;
                }
                else
                {
                    coll = chips[i, j - 1].GetComponent<Collider2D>();
                    chipPositions[i, j] = coll.bounds.max;
                }

                clone = Instantiate(GlobalVariables.CHIPPREFAB, chipPositions[i, j], Quaternion.identity, transform.parent);
                chips[i, j] = clone.GetComponent<Chip>();
                chips[i, j].Set_Index(new int[] { i, j });
            }
        }
        Destroy(chips[empty[0], empty[1]].gameObject);
        chips[empty[0], empty[1]] = null;

        fieldFrame.enabled = false;
    }
}
