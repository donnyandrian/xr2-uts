using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DroppedSpices : MonoBehaviour
{
    public List<SpiceObject> spices;
    public bool isLock = false;

    private void Start()
    {
        spices = new List<SpiceObject>();
    }
}
