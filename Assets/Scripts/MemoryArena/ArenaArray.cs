using System.Collections;
using System.Collections.Generic;

public readonly unsafe struct ArenaArray<T> : IEnumerable<T>, System.IDisposable where T : unmanaged
{
    private readonly T* _array;
    private readonly int _count;

    public int Count => _count;
    public ref T this[int index] => ref _array[index];

    public ArenaArray(MemoryArena arena, int count)
    {
        _array = arena.Allocate<T>(count);
        _count = count;
    }

    public T[] ToArray()
    {
        var array = new T[_count];
        for (var i = 0; i < _count; i++)
        {
            array[i] = _array[i];
        }

        return array;
    }
    
    public List<T> ToList()
    {
        var list = new List<T>(_count);
        for (var i = 0; i < _count; i++)
        {
            list.Add(_array[i]);
        }

        return list;
    }

    public void Dispose()
    {
        // MemoryArena will handle the deallocation
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new ArenaArrayEnumerator(_array, _count);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private struct ArenaArrayEnumerator : IEnumerator<T>
    {
        private readonly T* _array;
        private readonly int _count;
        private int _index;

        public ArenaArrayEnumerator(T* array, int count)
        {
            _array = array;
            _count = count;
            _index = -1;
        }

        public T Current => _array[_index];
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            _index++;
            return _index < _count;
        }

        public void Reset()
        {
            _index = -1;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}