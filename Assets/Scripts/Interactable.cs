using UnityEngine;

/// <summary>
/// Base class for anything the player can look at and press E on.
/// Make new interactables (doors, chests, pickups) by inheriting from this.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private string prompt = "Interact";

    /// <summary>Text shown on screen, e.g. "Pick up Orb".</summary>
    public virtual string Prompt => prompt;

    /// <summary>Called when the player presses E while looking at this object.</summary>
    public abstract void Interact(PlayerInteractor interactor);
}