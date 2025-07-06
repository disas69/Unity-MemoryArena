# Unity Memory Arena Allocator

A minimalistic implementation of **memory arena allocator** for Unity using `UnsafeUtility`. Designed to reduce GC pressure and improve allocation speed in performance-critical code (like mesh generation, procedural content, gameplay systems, etc).

## 🔧 Features

- Zero GC allocations
- Fixed arena size
- Aligned memory
- Supports only `unmanaged` types
- `ArenaArray<T>` for easy typed access

---

## 🚀 `ArenaArray<T>` vs `Array[]` performance (512 `Vector2`s per frame)

| Access Style   | Arena Frame Time | Heap Frame Time | Allocations |
|----------------|------------------|------------------|-------------|
| **Pointer** | 0.0348 ms        | 0.0349 ms        | Arena: 0 B / Heap: 4 KB |
| **Indexer**    | 0.0379 ms        | 0.0357 ms        | Arena: 0 B / Heap: 4 KB |

## 📦 Usage

```csharp
var arena = new MemoryArena(1024 * 1024); // Allocate 1MB arena
var array = new ArenaArray<Vector2>(arena, 512); // Allocate memory for 512 Vector2 from the arena

var length = array.Length;
for (var i = 0; i < length; i++)
{
    var angle = (Mathf.PI * 2 * i) / length;
    array[i].x = Mathf.Cos(angle) * radius;
    array[i].y = Mathf.Sin(angle) * radius;
}

arena.Reset(); // Reset allocation offset, memory stays
arena.Dispose(); // Free all memory
