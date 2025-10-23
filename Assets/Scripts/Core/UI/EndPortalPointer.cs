using UnityEngine;

public class EndPortalPointer : MonoBehaviour
{
    [Header("Pointer")]
    [SerializeField] private GameObject pointerPrefab;
    [SerializeField] private float screenEdgePadding = 64f;
    [SerializeField] private float depthFromCamera = 5f;
    [SerializeField] private string cameraTag = "MainCamera";

    private Camera cam;
    private Transform pointer;

    private void Start()
    {
        FindCamera();
        if (cam != null && pointerPrefab != null)
        {
            var go = Instantiate(pointerPrefab, cam.transform);
            pointer = go.transform;
            pointer.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (cam == null) FindCamera();
        if (cam == null || pointer == null) return;

        Vector3 vp = cam.WorldToViewportPoint(transform.position);

        if (vp.z < 0f)
        {
            vp.x = 1f - vp.x;
            vp.y = 1f - vp.y;
            vp.z = 0.001f;
        }

        bool onScreen = vp.x > 0f && vp.x < 1f && vp.y > 0f && vp.y < 1f;
        pointer.gameObject.SetActive(!onScreen);
        if (onScreen) return;

        float padX = screenEdgePadding / Screen.width;
        float padY = screenEdgePadding / Screen.height;

        float clampedX = Mathf.Clamp(vp.x, padX, 1f - padX);
        float clampedY = Mathf.Clamp(vp.y, padY, 1f - padY);

        Vector3 screenPos = new Vector3(clampedX * Screen.width, clampedY * Screen.height, depthFromCamera);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        pointer.position = worldPos;

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 dir = new Vector2(screenPos.x, screenPos.y) - screenCenter;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        pointer.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FindCamera()
    {
        var camObj = GameObject.FindGameObjectWithTag(cameraTag);
        if (camObj != null) cam = camObj.GetComponent<Camera>();
    }
}
