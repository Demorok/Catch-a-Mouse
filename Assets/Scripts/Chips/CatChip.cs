using System;
using System.Collections;
using UnityEngine;

public class CatChip : Chip
{
    public override void Move(Vector3 target, int[] index)
    {
        Move(target, index, typeof(MouseChip));
    }

    protected void Move(Vector3 target, int[] index, Type type)
    {
        StartCoroutine(MoveTo(target, timeToMove, () => Search(type)));
        Set_Index(index);
    }

    protected IEnumerator MoveTo(Vector3 target, float time, Action onComplete)
    {
        yield return StartCoroutine(MoveTo(target, time));
        onComplete();
    }

    protected void Search(Type type)
    {
        PlayField.queue.Enqueue(new QueueItem(index, type));
    }
}
