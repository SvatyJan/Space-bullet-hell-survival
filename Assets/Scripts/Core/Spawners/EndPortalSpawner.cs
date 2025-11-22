using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPortalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private float spawnDelaySeconds = 300f;

    [SerializeField] private List<GameObject> HideObjectsOnShowList = new List<GameObject>();


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(spawnDelaySeconds);

        foreach(GameObject obj in HideObjectsOnShowList)
        {
            obj.SetActive(false);
        }

        if (portalPrefab != null)
        {
            Instantiate(portalPrefab, transform.position, transform.rotation);
        }
        enabled = false;
    }
}
