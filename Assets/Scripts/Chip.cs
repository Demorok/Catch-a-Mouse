using System.Collections;
using UnityEngine;

public class Chip : MonoBehaviour
{
    public float timeToMove;
    protected static bool chipMoving = false;
    protected int[] index;

    protected const float EPS = 0.001f;

    public virtual void Move(Vector3 target, int[] index)
    {
        StartCoroutine(MoveTo(target, timeToMove));
        Set_Index(index);
    }

    public void Set_Index(int[] index)
    {
        this.index = index;
    }

    protected IEnumerator MoveTo(Vector3 target, float time)
    {
        chipMoving = true;
        float distance = Vector3.Distance(target, gameObject.transform.position);
        float speed = distance / time;
        while ((target - gameObject.transform.position).sqrMagnitude > EPS)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, speed * Time.fixedUnscaledDeltaTime);
            yield return new WaitForSecondsRealtime(Time.fixedUnscaledDeltaTime);
        }
        chipMoving = false;
    }

    private void OnMouseDown()
    {
        if (!chipMoving && Input.GetMouseButtonDown(0))
            PlayField.moveQueue.Enqueue(index);
    }
}
