using Unity.Collections;
using UnityEngine;

public class MemoryArenaTest : MonoBehaviour
{
    private MemoryArena _memoryArena;
    
    private void Start()
    {
        _memoryArena = new MemoryArena(1024 * 1024, Allocator.Persistent, true);
    }

    private void Update()
    {
        RunArenaTest();
        // RunDefaultTest();
    }

    private void RunArenaTest()
    {
        _memoryArena.Reset();

        var points1 = new ArenaArray<Vector2>(_memoryArena, 20100);
        var count = points1.Count;
        for (var i = 0; i < count; i++)
        {
            points1[i].x = i * 0.1f;
            points1[i].y = i * 0.2f;
            
            // Debug.Log(points1[i]);
        }

        var points2 = new ArenaArray<Vector2>(_memoryArena, 17000);
        // foreach (var pos in points2)
        // {
        //     // Debug.Log(pos);
        // }
        
        var ints = new ArenaArray<int>(_memoryArena, 1010);
        var countInts = ints.Count;
        for (var i = 0; i < countInts; i++)
        {
            ints[i] = i * 10;
            // Debug.Log(ints[i]);
        }
    }

    private void RunDefaultTest()
    {
        var points1 = new Vector2[20100];
        for (var i = 0; i < points1.Length; i++)
        {
            points1[i].x = i * 0.1f;
            points1[i].y = i * 0.2f;
            
            // Debug.Log(points1[i]);
        }

        var points2 = new Vector2[17000];
        foreach (var pos in points2)
        {
            // Debug.Log(pos);
        }

        var ints = new int[1010];
        for (var i = 0; i < ints.Length; i++)
        {
            ints[i] = i * 10;
            // Debug.Log(ints[i]);
        }
    }

    private void OnDestroy()
    {
        if (_memoryArena != null)
        {
            _memoryArena.Dispose();
            _memoryArena = null;
        }
    }
}