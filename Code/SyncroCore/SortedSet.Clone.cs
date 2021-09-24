namespace SyncroCore;

public static class Utility
{
    public static SortedSet<T> Clone<T>(this SortedSet<T> original)
    {
        var arr = new T[original.Count];
        original.CopyTo(arr, 0);

        return new SortedSet<T>(arr);
    }
}
