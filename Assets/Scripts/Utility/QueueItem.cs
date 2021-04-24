using System;

public class QueueItem
{
    public int[] index;
    public Type searchObject;

    public QueueItem (int[] index, Type searchObject)
    {
        this.index = index;
        this.searchObject = searchObject;
    }
}
