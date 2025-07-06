using System;
using System.Collections;
using System.Collections.Generic;

public unsafe struct ArenaArray<T> : IEnumerable<T> where T : unmanaged
{
    public readonly T* Ptr;
    public readonly int Length;
    
    public ref T this[int index] => ref Ptr[index];

    public ArenaArray(MemoryArena arena, int length)
    {
        Ptr = arena.Allocate<T>(length);
        Length = length;
    }

    public T[] ToArray()
    {
        var array = new T[Length];
        for (var i = 0; i < Length; i++)
        {
            array[i] = Ptr[i];
        }

        return array;
    }
    
    public List<T> ToList()
    {
        var list = new List<T>(Length);
        for (var i = 0; i < Length; i++)
        {
            list.Add(Ptr[i]);
        }

        return list;
    }
    
    public Span<T> AsSpan()
    {
        return new Span<T>(Ptr, Length);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new ArenaArrayEnumerator(Ptr, Length);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private struct ArenaArrayEnumerator : IEnumerator<T>
    {
        private readonly T* _array;
        private readonly int _length;
        private int _index;

        public ArenaArrayEnumerator(T* array, int length)
        {
            _array = array;
            _length = length;
            _index = -1;
        }

        public T Current => _array[_index];
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            _index++;
            return _index < _length;
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