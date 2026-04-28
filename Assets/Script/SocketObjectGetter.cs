using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketObjectGetter : MonoBehaviour
{
    public XRSocketInteractor socketInteractor;
    public DroppedSpices droppedSpices;

    public List<Transform> interacted;

    private void Start()
    {
        interacted = new List<Transform>();
    }

    private void OnEnable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnObjectPlaced);
            socketInteractor.selectExited.AddListener(OnObjectRemoved);
        }
    }

    private void OnDisable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnObjectPlaced);
            socketInteractor.selectExited.RemoveListener(OnObjectRemoved);
        }
    }

    private void OnObjectPlaced(SelectEnterEventArgs args)
    {
        var obj = args.interactableObject.transform;
        interacted.Add(obj);
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<BoxCollider>().enabled = false;

        var spice = obj.GetComponent<SpiceObject>();
        Debug.Log("Object placed: " + spice.spiceName);
        droppedSpices.spices.Add(spice);
    }

    private void OnObjectRemoved(SelectExitEventArgs args)
    {
        var obj = args.interactableObject.transform;
        interacted.Remove(obj);
        obj.GetComponent<Rigidbody>().isKinematic = false;
        obj.GetComponent<BoxCollider>().enabled = true;

        var spice = obj.GetComponent<SpiceObject>();
        Debug.Log("Object removed: " + spice.spiceName);
        droppedSpices.spices.Remove(spice);
    }
}
