using UnityEngine;

public class GridTest : MonoBehaviour
{
    private Grid grid;
    [SerializeField] int value = 0;

    void Start()
    {
        grid = new Grid(100, 100, 10, new Vector3(0, 0));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           grid.SetValue(GetMouseWorldPosition(), value);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(GetMouseWorldPosition()));
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
        mousePosition.z = 10f; // vzdálenost od kamery
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
