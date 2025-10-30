using UnityEngine;
using TMPro;

public class InteractManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private float interactRadius = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("UI")]
    [SerializeField] private TMP_Text descriptionText;

    private IInteractable currentTarget;

    private void Update()
    {
        GameObject playerShip = ShipController.GetControllingObject();
        if (playerShip == null) return;

        FindClosestInteractable(playerShip.transform.position);

        UpdateDescriptionText();

        CheckShowDescriptionText();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact(playerShip);
        }

    }

    private void FindClosestInteractable(Vector3 playerPos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(playerPos, interactRadius, interactableLayer);
        float bestDist = float.MaxValue;
        IInteractable best = null;

        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector2.Distance(playerPos, hit.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = interactable;
            }
        }

        currentTarget = best;
    }

    private void UpdateDescriptionText()
    {
        if (currentTarget != null)
        {
            descriptionText.text = "Press <color=#FF0000><b>" + interactKey + "</b></color> to <b>" + currentTarget.DescriptionText + "</b>";
        }
    }

    private void CheckShowDescriptionText()
    {
        if (currentTarget != null)
        {
            descriptionText.alpha = 1f;
        }
        else
        {
            descriptionText.alpha = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        GameObject playerShip = ShipController.GetControllingObject();
        if (playerShip == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(playerShip.transform.position, interactRadius);
    }
}
