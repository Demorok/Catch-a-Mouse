using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    [SerializeField] int height;
    [SerializeField] int width;

    public static Queue<QueueItem> queue = new Queue<QueueItem>();

    Chip[,] chips;
    Vector3[,] chipPositions;

    int winCondition;
    int miceInHole = 0;

    BoxCollider2D fieldFrame;

    void Start()
    {
        fieldFrame = GetComponent<BoxCollider2D>();
        fieldFrame.size = new Vector2(width, height);
        gameObject.transform.localScale = new Vector3(height, width, 1);

        Initialise_Field();
    }

    void Update()
    {
        Check_Queue(queue);
        Check_Win_Condition();
    }

    void Check_Queue(Queue<QueueItem> queue)
    {
        while(queue.Count > 0)
        {
            QueueItem item = queue.Dequeue();
            if (item.searchObject == null)
                Move_Chip(item.index);
            else
                Field_Event(item);
        }
    }

    void Check_Win_Condition()
    {
        if (miceInHole == winCondition)
            Reload();
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

    void Field_Event(QueueItem item)
    {
        int[] targetIndex = Look_Adjacent_Places(item);
        if (targetIndex == null)
            return;
        if (chips[item.index[0], item.index[1]].GetType() != typeof(HoleChip))
            Reload();
        else
        {
            miceInHole += 1;
            Transform_Chip<Chip>(targetIndex, GlobalVariables.CHIPPREFAB);
        }
    }

    private void Reload()
    {
        Instantiate(GlobalVariables.GAMEPPREFAB, transform.parent.position, Quaternion.identity);
        Destroy(gameObject.transform.parent.gameObject);
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

    int[] Look_Adjacent_Places(QueueItem item)
    {
        return Look_Adjacent_Places(item.index, item.searchObject);
    }

    void Transform_Chip<T>(int[] index, GameObject prefab) where T : Chip
    {
        GameObject clone;
        Destroy(chips[index[0], index[1]].gameObject);
        clone = Instantiate(prefab, chipPositions[index[0], index[1]], Quaternion.identity, transform.parent);
        chips[index[0], index[1]] = clone.GetComponent<T>();
        chips[index[0], index[1]].Set_Index(index);
    }

    void Initialise_Field()
    {

        int[] center = new int[] { Mathf.CeilToInt(height / 2), Mathf.CeilToInt(width / 2) };
        int[] empty = new int[] { height - 1, width - 2 };
        List<int[]> mouseSpawn = new List<int[]>
        {
            new int[]{0, 0},
            new int[]{0, width-1},
            new int[]{height-1, 0},
            new int[]{height-1, width-1}
        };
        List<int[]> catSpawn = new List<int[]>
        {
            new int[]{center[0]-1, center[1]-1},
            new int[]{center[0]-1, center[1]+1},
            new int[]{center[0]+1, center[1]-1},
            new int[]{center[0]+1, center[1]+1},
        };

        winCondition = mouseSpawn.Count;

        chips = new Chip[height, width];
        chipPositions = new Vector3[height, width];

        Collider2D coll;
        GameObject clone;

        for (int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {

                //fill positions
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

                //spawn chips
                if (mouseSpawn.Count > 0 && i == mouseSpawn[0][0] && j == mouseSpawn[0][1]) //MouseChip
                {
                    clone = Instantiate(GlobalVariables.MOUSECHIPPREFAB, chipPositions[i, j], Quaternion.identity, transform.parent);
                    chips[i, j] = clone.GetComponent<MouseChip>();
                    mouseSpawn.RemoveAt(0);
                }
                else if (catSpawn.Count > 0 && i == catSpawn[0][0] && j == catSpawn[0][1]) //CatChip
                {
                    clone = Instantiate(GlobalVariables.CATCHIPPREFAB, chipPositions[i, j], Quaternion.identity, transform.parent);
                    chips[i, j] = clone.GetComponent<CatChip>();
                    catSpawn.RemoveAt(0);
                }
                else if (i == center[0] && j == center[1])
                {
                    clone = Instantiate(GlobalVariables.HOLECHIPPREFAB, chipPositions[i, j], Quaternion.identity, transform.parent);
                    chips[i, j] = clone.GetComponent<HoleChip>();
                }
                else
                {
                    clone = Instantiate(GlobalVariables.CHIPPREFAB, chipPositions[i, j], Quaternion.identity, transform.parent);
                    chips[i, j] = clone.GetComponent<Chip>();
                }
                chips[i, j].Set_Index(new int[] { i, j });
            }
        }

        fieldFrame.enabled = false;
    }
}
