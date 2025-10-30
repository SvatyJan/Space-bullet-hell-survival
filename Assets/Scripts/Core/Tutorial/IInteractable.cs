using UnityEngine;

public interface IInteractable
{
   public string DescriptionText { get; }
   public void Interact(GameObject interactor);
}
