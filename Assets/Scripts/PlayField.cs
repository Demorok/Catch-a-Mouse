using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    [SerializeField] int height;
    [SerializeField] int width;

    public static Queue<int[]> moveQueue = new Queue<int[]>();

    Chip[,] chips;
    Vector3[,] chipPositions;

    BoxCollider2D fieldFrame;

    // Start is called before the first frame update
    void Start()
    {
        fieldFrame = GetComponent<BoxCollider2D>();
        fieldFrame.size = new Vector2(width, height);
        gameObject.transform.localScale = new Vector3(height, width, 1);

        Initialise_Field();
    }

    // Update is called once per frame
    void Update()
    {
        Check_Move_Queue();
    }

    void Check_Move_Queue()
    {
        if (moveQueue.Count > 0)
            Move_Chip(moveQueue.Dequeue());
    }

    void Move_Chip(int[] index)
    {
        int[] targetIndex = Look_Adjacent_Places(index, null);
        if (targetIndex == null)
            return;
        chips[index[0], index[1]].Move(chipPositions[targetIndex[0], targetIndex[1]], targetIndex);

        chips[targetIndex[0], targetIndex[1]] = chips[index[0], index[1]]; //swap
        chips[index[0], index[1]] = null;
    }

    int[] Look_Adjacent_Places(int[] index, Type type)
    {
        int[][] adjacent_places = new int[][]
        {
            new int[] {index[0], index[1] + 1},
            new int[] {index[0], index[1] - 1},
            new int[] {index[0] + 1, index[1]},
            new int[] {index[0] - 1, index[1]},
        };
        foreach(int[] place in adjacent_places)
        {
            try
            {
                if (chips[place[0], place[1]].GetType() == type)
                    return place;
            }
            catch (NullReferenceException)
            {
                if (type == null)
                    return place;
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }
        return null;
    }

    void Initialise_Field()
    {

        int[] center = new int[] { Mathf.CeilToInt(height / 2), Mathf.CeilToInt(width / 2) };
        int[] empty = new int[] { height - 1, width - 2 };
        int[][] mouseSpawn = new int[][]
        {
            new int[]{0, 0},
            new int[]{0, width-1},
            new int[]{height-1, 0},
            new int[]{height-1, width-1}
        };
        int[][] catSpawn = new int[][]
        {
            new int[]{center[0]-1, center[1]-1},
            new int[]{center[0]-1, center[1]+1},
            new int[]{center[0]+1, center[1]-1},
            new int[]{center[0]+1, center[1]+1},
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
                else if (j == 0 || (i == height - 1 && j == width - 1))
                {
                    coll = chips[i - 1, j].GetComponent<Collider2D>();
                    chipPositions[i, j] = coll.bounds.min;
                }
                else
                {
                    coll = chips[i, j - 1].GetComponent<Collider2D>();
                    chipPositions[i, j] = coll.bounds.max;
                    if (i == empty[0] && j == empty[1])
                        continue;
                }

                clone = Instantiate(GlobalVariables.CHIPPREFAB, chipPositions[i, j], Quaternion.identity, transform.parent);
                chips[i, j] = clone.GetComponent<Chip>();
                chips[i, j].Set_Index(new int[] { i, j });
            }
        }
        chips[empty[0], empty[1]] = null;
        fieldFrame.enabled = false;
    }
}
