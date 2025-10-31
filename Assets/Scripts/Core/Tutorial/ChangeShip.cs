using Cinemachine;
using UnityEngine;

public class ChangeShip : MonoBehaviour, IInteractable
{
    [Header("Interactable Ship Reference")]
    [SerializeField] private GameObject ship;

    public string DescriptionText => "Change ship";

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private PlayerUpgradeDisplayUI playerUpgradeDisplayUI;

    [Header("Observer")]
    public System.Action OnUpgradesChanged;

    private void Awake()
    {
        if (playerUpgradeDisplayUI == null)
            playerUpgradeDisplayUI = FindObjectOfType<PlayerUpgradeDisplayUI>();
    }

    public void Interact(GameObject currentControllingShip)
    {
        DisableCurrentShip(currentControllingShip);
        EnableNewShip();
    }

    private void DisableCurrentShip(GameObject currentControllingShip)
    {
        currentControllingShip.GetComponent<PlayerShip>().enabled = false;
        currentControllingShip.layer = 11;
        gameObject.GetComponent<ChangeShip>().enabled = true;
    }

    private void EnableNewShip()
    {
        ShipController.SetControllingObject(gameObject);
        gameObject.layer = 9;
        gameObject.GetComponent<PlayerShip>().enabled = true;

        cinemachineVirtualCamera.LookAt = gameObject.transform;
        cinemachineVirtualCamera.Follow = gameObject.transform;

        playerUpgradeDisplayUI.SetProgression(gameObject.GetComponent<PlayerProgression>());

        gameObject.GetComponent<ChangeShip>().enabled = false;
    }
}
