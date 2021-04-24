using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatChip : Chip
{
    public override void Move(Vector3 target, int[] index)
    {
        base.Move(target, index);
    }

    protected IEnumerator MoveTo(Vector3 target, float time, Action onComplete)
    {
        yield return StartCoroutine(MoveTo(target, time));
        onComplete();
    }

    void Search_For_Mice()
    {

    }
}
