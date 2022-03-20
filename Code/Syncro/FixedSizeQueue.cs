namespace Syncro;

public class FixedSizeQueue<T>
{
    Queue<T> q = new();
    private object lockObject = new();

    public int Limit { get; set; }
    public void Enqueue(T obj)
    {
        q.Enqueue(obj);
        lock (lockObject)
        {
            T overflow;
            while (q.Count > Limit && q.TryDequeue(out overflow)) ;
        }
    }

    public List<T> ToList()
        {
        return q.ToList();
    }
}
