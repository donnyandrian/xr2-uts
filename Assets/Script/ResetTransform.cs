using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ResetTransform : MonoBehaviour
{
    // Variables to store the initial state
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _startScale;

    private XRGrabInteractable _interactable;

    void Start()
    {
        // Record the starting transform values
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _startScale = transform.localScale;

        _interactable = GetComponent<XRGrabInteractable>();
    }

    // Call this method whenever you need to reset
    public void ResetToInitialState()
    {
        if (_interactable != null && _interactable.isSelected)
        {
            var manager = _interactable.interactionManager;
            manager.CancelInteractableSelection((IXRSelectInteractable)_interactable);
        }

        transform.position = _startPosition;
        transform.rotation = _startRotation;
        transform.localScale = _startScale;

        // Physics Guard: If using Rigidbody, stop the momentum too!
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}