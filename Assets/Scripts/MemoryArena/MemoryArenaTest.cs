using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class MemoryArenaTest : MonoBehaviour
{
    [SerializeField] private int _points = 512;
    [SerializeField] private float _radius = 5f;
    [SerializeField] private float _durationSeconds = 10;

    private MemoryArena _arena;
    private LineRenderer _arenaRenderer;
    private LineRenderer _heapRenderer;

    private float _elapsedTime;
    private Stopwatch _arenaStopwatch;
    private Stopwatch _heapStopwatch;
    private int _frameCount;
    private double _totalArenaTime;
    private double _totalHeapTime;

    private void Start()
    {
        _arena = new MemoryArena(1024 * 1024); // 1MB arena

        _arenaRenderer = CreateLineRenderer(Color.green, "Arena Renderer");
        _heapRenderer = CreateLineRenderer(Color.red, "Heap Renderer");

        _arenaStopwatch = new Stopwatch();
        _heapStopwatch = new Stopwatch();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _durationSeconds)
        {
            Debug.Log($"[Arena] Avg Frame Time: {_totalArenaTime / _frameCount:F4} ms");
            Debug.Log($"[Heap ] Avg Frame Time: {_totalHeapTime / _frameCount:F4} ms");
            enabled = false;
            return;
        }
        
        _radius += Mathf.Sin(_elapsedTime * 3f) * Time.deltaTime * 0.7f;

        _arena.Reset();

        _arenaStopwatch.Restart();
        var arenaArray = new ArenaArray<Vector2>(_arena, _points);
        FillCircle(ref arenaArray, _radius);
        _arenaStopwatch.Stop();
        _totalArenaTime += _arenaStopwatch.Elapsed.TotalMilliseconds;

        _heapStopwatch.Restart();
        var heapArray = new Vector2[_points];
        FillCircle(ref heapArray, _radius);
        _heapStopwatch.Stop();
        _totalHeapTime += _heapStopwatch.Elapsed.TotalMilliseconds;

        _frameCount++;

        RenderCircle(_arenaRenderer, arenaArray, offset: new Vector3(-_radius * 1.5f, 0));
        RenderCircle(_heapRenderer, heapArray, offset: new Vector3(_radius * 1.5f, 0));
    }

    private void OnDestroy()
    {
        _arena.Dispose();
    }

    private unsafe void FillCircle(ref ArenaArray<Vector2> array, float radius)
    {
        var length = array.Length;
        for (var i = 0; i < length; i++)
        {
            var angle = (Mathf.PI * 2 * i) / length;
            array.Ptr[i].x = Mathf.Cos(angle) * radius;
            array.Ptr[i].y = Mathf.Sin(angle) * radius;
        }
    }

    private void FillCircle(ref Vector2[] array, float radius)
    {
        var length = array.Length;
        for (var i = 0; i < length; i++)
        {
            var angle = (Mathf.PI * 2 * i) / length;
            array[i].x = Mathf.Cos(angle) * radius;
            array[i].y = Mathf.Sin(angle) * radius;
        }
    }

    private void RenderCircle(LineRenderer renderer, ArenaArray<Vector2> array, Vector3 offset)
    {
        renderer.positionCount = array.Length + 1;
        for (var i = 0; i < array.Length; i++)
        {
            renderer.SetPosition(i, (Vector3)array[i] + offset);
        }
        renderer.SetPosition(array.Length, (Vector3)array[0] + offset); // close the circle
    }

    private void RenderCircle(LineRenderer renderer, Vector2[] array, Vector3 offset)
    {
        renderer.positionCount = array.Length + 1;
        for (var i = 0; i < array.Length; i++)
        {
            renderer.SetPosition(i, (Vector3)array[i] + offset);
        }
        renderer.SetPosition(array.Length, (Vector3)array[0] + offset);
    }

    private LineRenderer CreateLineRenderer(Color color, string name)
    {
        var go = new GameObject(name);
        var lr = go.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.widthMultiplier = 0.1f;
        lr.loop = false;
        lr.positionCount = 0;
        lr.useWorldSpace = true;
        lr.startColor = color;
        lr.endColor = color;
        return lr;
    }
}
