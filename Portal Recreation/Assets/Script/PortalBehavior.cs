using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortalBehavior : MonoBehaviour
{
    public PortalGunController gunController;
    public PortalBehavior otherPortal;
    public PortalCamera myCamera;

    public MeshRenderer renderPlane;
    public bool index;
    public Vector3 portalExtents;
    private Vector3 _colSize;

    int _portalPlaneLayer = 9;

    public List<GameObject> IgnoreTPList = new();

    public LayerMask layerMask;

    private void Start()
    {
        _colSize = portalExtents / 2f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gunController.DoesOtherPortalExist(index))
            return;

        if (IgnoreTPList.Contains(other.gameObject))
            return;

        gunController.TeleportFrom(otherPortal, other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        // if they walked forwards, then remove them from the ignore TPList and make them collide with other objects again

        // check if the object is in front of the portal
        bool walkedForwards = Vector3.Dot(other.transform.position - transform.position, transform.forward) > 0f;
        if (walkedForwards)
        {
            IgnoreTPList.Remove(other.gameObject);
            
            // Also make it so that the exiting object can collide with other objects again
            other.excludeLayers &= ~(1 << 0);
            // stop colliding with the portal plane
            other.includeLayers &= ~(1 << _portalPlaneLayer);
        }
        else
        {
            // otherwise, teleport them right back to the other portal
            if (!gunController.DoesOtherPortalExist(index))
                return;
            
            IgnoreTPList.Remove(other.gameObject);

            gunController.TeleportFrom(otherPortal, other.gameObject);
        }
    }
}
