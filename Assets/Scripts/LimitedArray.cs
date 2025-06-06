using System.Collections.Generic;
using Unity.Collections;

public class LimitedArray<T>
{
    private T[] data;
    private int capacity;
    private int count;
    private int start;
    
    public LimitedArray(int capacity)
    {
        this.capacity = capacity;
        data = new T[capacity];
        count = 0;
        start = 0;
    }

    public int Count => count;
    public T[] Data => data;

    public T this[int idx] => data[idx];

    public void Add(T item)
    {
        int idx = (start + count) % capacity;

        if (count == capacity)
        {
            data[start] = item;
            start = (start + 1) % capacity;
        }
        else
        {
            data[idx] = item;
            count++;
        }
    }

    public void AddRange(IEnumerable<T> collection)
    {
        foreach (T item in collection) Add(item);
    }

    public void Clear()
    {
        count = 0;
        start = 0;
    }
}