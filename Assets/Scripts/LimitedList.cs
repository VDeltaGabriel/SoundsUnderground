using System.Collections.Generic;

public class LimitedList<T>
{
    private readonly int _maxSize;
    private readonly List<T> _data;

    public LimitedList(int maxSize)
    {
        _maxSize = maxSize;
        _data = new List<T>();
    }

    public void Add(T item)
    {
        _data.Add(item);
        if (_data.Count > _maxSize) _data.RemoveAt(0);
    }
    
    public T this[int idx] => _data[idx];
    
    public int Count => _data.Count;
    
    public List<T> ToList() => new List<T>(_data);
}