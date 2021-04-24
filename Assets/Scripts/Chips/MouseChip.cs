using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseChip : CatChip
{
    public override void Move(Vector3 target, int[] index)
    {
        Move(target, index, typeof(CatChip));
    }
}
