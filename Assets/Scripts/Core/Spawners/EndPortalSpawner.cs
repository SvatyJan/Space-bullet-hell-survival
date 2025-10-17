using System.Collections;
using UnityEngine;

public class EndPortalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private float spawnDelaySeconds = 300f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(spawnDelaySeconds);
        if (portalPrefab != null)
        {
            Instantiate(portalPrefab, transform.position, transform.rotation);
        }
        enabled = false;
    }
}
