using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour
{
    public float timeToMove;
    int[] index;

    protected const float EPS = 0.001f;

    public void Move(Transform target, int[] index)
    {
        StartCoroutine(MoveTo(target, timeToMove));
        Set_Index(index);
    }

    public void Set_Index(int[] index)
    {
        this.index = index;
    }

    IEnumerator MoveTo(Transform target, float time)
    {
        float distance = Vector3.Distance(target.position, gameObject.transform.position);
        float speed = distance / time;
        while (Mathf.Abs(target.position.sqrMagnitude - gameObject.transform.position.sqrMagnitude) > EPS)
        {
            gameObject.transform.position += Vector3.MoveTowards(gameObject.transform.position, target.position, speed * Time.fixedUnscaledDeltaTime);
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("pow");
    }
}
