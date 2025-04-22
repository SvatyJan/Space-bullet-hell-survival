using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance { get; private set; }

    public Pathfinding pathfinding { get; private set; }

    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Vector3 vector = new Vector3(-(width/2), -(height/2), 0);
        pathfinding = new Pathfinding(width, height, 1, vector);
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
        mousePosition.z = 10f; // vzdálenost od kamery
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
