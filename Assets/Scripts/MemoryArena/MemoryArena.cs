using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public unsafe class MemoryArena : System.IDisposable
{
    private long _capacity;
    private Allocator _allocator;
    private bool _isExpandable;

    private long _offset;
    private byte* _buffer;

    public MemoryArena(long sizeInBytes, Allocator allocator = Allocator.Persistent, bool isExpandable = false)
    {
        _capacity = sizeInBytes;
        _allocator = allocator;
        _isExpandable = isExpandable;
        
        _offset = 0;
        _buffer = (byte*)UnsafeUtility.Malloc(_capacity, 16, allocator);
        
        // Debug.Log($"MemoryArena created with capacity: {_capacity} bytes");
    }

    public T* Allocate<T>(int count = 1) where T : unmanaged
    {
        long size = UnsafeUtility.SizeOf<T>() * count;
        long alignment = UnsafeUtility.AlignOf<T>();
        var alignedOffset = (_offset + (alignment - 1)) & ~(alignment - 1);

        if (alignedOffset + size > _capacity)
        {
            if (_isExpandable)
            {
                var newCapacity = _capacity * 2;
                var newBuffer = (byte*)UnsafeUtility.Malloc(newCapacity, 16, _allocator);
                
                if (newBuffer == null)
                {
                    throw new System.Exception("Failed to allocate more memory for the arena!");
                }

                UnsafeUtility.MemCpy(newBuffer, _buffer, _offset);
                UnsafeUtility.Free(_buffer, _allocator);
                
                _buffer = newBuffer;
                _capacity = newCapacity;
                
                // Debug.Log($"MemoryArena expanded to {newCapacity} bytes");
                
                return Allocate<T>(count);
            }
            else
            {
                throw new System.Exception("Arena out of memory!");
            }
        }

        var result = (T*)(_buffer + alignedOffset);
        _offset = alignedOffset + size;

        // Debug.Log($"Allocated {size} bytes for {typeof(T).Name} at offset {alignedOffset}. New offset is {_offset}");
        
        return result;
    }

    public void Reset()
    {
        _offset = 0;
        
        // Debug.Log("MemoryArena reset. Offset set to 0");
    }

    public void Dispose()
    {
        if (_buffer != null)
        {
            UnsafeUtility.Free(_buffer, _allocator);
            _buffer = null;
            
            // Debug.Log("MemoryArena disposed");
        }
    }
}