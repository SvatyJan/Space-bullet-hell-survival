using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }

    public Pathfinding pathfinding { get; private set; }

    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private int maxPathCalculationsPerFrame = 5;

    private Queue<PathRequest> requestQueue = new Queue<PathRequest>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pathfinding = new Pathfinding(width, height, cellSize);
    }

    private void LateUpdate()
    {
        int count = 0;
        while (requestQueue.Count > 0 && count < maxPathCalculationsPerFrame)
        {
            var request = requestQueue.Dequeue();
            List<PathNode> path = pathfinding.FindPath(request.startX, request.startY, request.endX, request.endY);
            request.callback?.Invoke(path);
            count++;
        }
    }

    public void RequestPath(int startX, int startY, int endX, int endY, Action<List<PathNode>> callback)
    {
        requestQueue.Enqueue(new PathRequest(startX, startY, endX, endY, callback));
    }

    private struct PathRequest
    {
        public int startX, startY, endX, endY;
        public Action<List<PathNode>> callback;

        public PathRequest(int startX, int startY, int endX, int endY, Action<List<PathNode>> callback)
        {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
            this.callback = callback;
        }
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vector = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vector.z = 0f;
        return vector;
    }

    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPoint, Camera worldCamera)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private void OnDrawGizmos()
    {
        if (pathfinding != null)
        {
            pathfinding.GetGrid().DrawGizmos();
        }
    }
}
