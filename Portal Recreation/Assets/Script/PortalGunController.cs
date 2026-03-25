using System;
using System.Collections;
using UnityEngine;

public class PortalGunController : MonoBehaviour
{
    public Transform portalHolder, shootPoint, cameraTransform;
    public PortalCamera portalCamera;
    
    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private static readonly int portalPlaneLayer = 9;
    public (bool, bool) activePortals = (false, false);

    public PortalBehavior[] portals;
    [SerializeField] private float maxPortalDistance;
    [SerializeField] private LayerMask portalLayerMask;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material portalMaterial;

    private PortalBehavior GetPortalBehavior(bool index)
    {
        return ((index) ? portals[0] : portals[1]);
    }

    private void Awake()
    {
        SetupPortals();
    }

    private void SetupPortals()
    {
        portals[0].otherPortal = portals[1];
        portals[1].otherPortal = portals[0];

        portals[0].index = false;
        portals[1].index = true;

        portals[0].gunController = this;
        portals[1].gunController = this;

        portals[0].gameObject.SetActive(false);
        portals[1].gameObject.SetActive(false);

        portalCamera.portalGunController = this;
        portalCamera.mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootPortal(false);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ShootPortal(true);
        }
    }

    public bool DoesOtherPortalExist(bool index)
    {
        if (index)
            return activePortals.Item1;
        else
            return activePortals.Item2;
    }

    public void TeleportFrom(PortalBehavior fromPortal, PortalBehavior newPortal, GameObject obj)
    {
        Transform inTransform = fromPortal.renderPlane.transform;
        Transform outTransform = newPortal.renderPlane.transform;
        
        // make it so that the new portal doesn't instantly teleport them back
        newPortal.IgnoreTPList.Add(obj.gameObject);

        // disable the character controller if necessary
        if (obj.TryGetComponent<CharacterController>(out var cc))
        {
            cc.enabled = false;
        }

        // Update position of object.
        //Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        //relativePos = halfTurn * relativePos;
        //obj.transform.position = outTransform.TransformPoint(relativePos);
        obj.transform.position = outTransform.position;

        // Update rotation of object.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        obj.transform.rotation = outTransform.rotation * relativeRot;

        // Update velocity of rigidbody.
        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Vector3 relativeVel = inTransform.InverseTransformDirection(rb.linearVelocity);
            relativeVel = halfTurn * relativeVel;
            rb.linearVelocity = outTransform.TransformDirection(relativeVel);
        }


        if (cc != null)
        {
            cc.enabled = true;
        }

        // Make the teleported object only collide with the portalPlane until it leaves the portal trigger
        if (obj.TryGetComponent<Collider>(out var objCollider))
        {
            // stop colliding with walls (default layer)
            objCollider.excludeLayers |= (1 << 0);
            // collide with the portal plane
            objCollider.includeLayers |= (1 << portalPlaneLayer);
        }
    }

    private void ShootPortal(bool index)
    {
        PortalBehavior beh = GetPortalBehavior(index);
        GameObject thePortal = GetPortalBehavior(index).gameObject;
        // raycast out to a surface
        if (Physics.Raycast(shootPoint.position, cameraTransform.forward, out RaycastHit hit, maxPortalDistance, portalLayerMask))
        {
            // move portal to the new location
            thePortal.SetActive(true);
            beh.IsPlaced = true;

            thePortal.transform.position = hit.point;

            // aligned to new normal
            thePortal.transform.forward = hit.normal;

            // set active portal to true
            if (index)
                activePortals.Item1 = true;
            else
                activePortals.Item2 = true;
            
            if (DoesOtherPortalExist(index))
            {
                // set material to camera material
                beh.renderPlane.material = portalMaterial;
                portalCamera.UpdatePortalTexture();
            }
            else
            {
                // just use default material
                beh.renderPlane.material = defaultMaterial;
            }
        }
    }
}
