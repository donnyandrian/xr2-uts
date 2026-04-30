using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ResetTransform : MonoBehaviour
{
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Vector3 _startScale;

    private XRGrabInteractable _interactable;
    private Rigidbody _rb;

    void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _startScale = transform.localScale;

        _interactable = GetComponent<XRGrabInteractable>();
        _rb = GetComponent<Rigidbody>();
    }

    public void ResetToInitialState()
    {
        // Force the Interaction Manager to drop the object
        if (_interactable != null && _interactable.isSelected)
        {
            var manager = _interactable.interactionManager;
            // This tells the manager to force all interactors (hands or sockets) to let go
            manager.CancelInteractableSelection((IXRSelectInteractable)_interactable);
        }

        // Physics Guard: IMPORTANT for Sockets
        // If the socket made the RB kinematic, we reset that state
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = false; // Ensure physics can take over again
        }

        // Apply the Transform
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        transform.localScale = _startScale;

        // The "Socket Break" (The Secret Sauce)
        // Briefly toggle the interactable to clear the "Hover" state of the socket
        StartCoroutine(TemporaryDisableInteraction());
    }

    private System.Collections.IEnumerator TemporaryDisableInteraction()
    {
        _interactable.enabled = false;
        yield return new WaitForFixedUpdate(); // Wait for physics/interaction loop to cycle
        _interactable.enabled = true;
    }
}