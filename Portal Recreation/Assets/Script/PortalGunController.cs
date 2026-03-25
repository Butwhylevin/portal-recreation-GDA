using System;
using System.Collections;
using UnityEngine;

public class PortalGunController : MonoBehaviour
{
    public GameObject portalPrefab, portalPrefab1;
    public Transform portalHolder, shootPoint, cameraTransform;
    
    int _portalPlaneLayer = 9;
    public (bool, bool) activePortals = (false, false);

    [SerializeField] private (PortalBehavior, PortalBehavior) _portals;
    [SerializeField] private float maxPortalDistance;
    [SerializeField] private LayerMask portalLayerMask;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material[] portalMaterials;

    private PortalBehavior GetPortalBehavior(bool index)
    {
        return ((index) ? _portals.Item1 : _portals.Item2);
    }

    private void Start()
    {
        SetupPortals();
    }

    private void SetupPortals()
    {
        _portals.Item1 = Instantiate(portalPrefab, portalHolder).GetComponent<PortalBehavior>();
        _portals.Item2 = Instantiate(portalPrefab1, portalHolder).GetComponent<PortalBehavior>();

        _portals.Item1.otherPortal = _portals.Item2;
        _portals.Item2.otherPortal = _portals.Item1;

        _portals.Item1.myCamera.transform.parent = null;
        _portals.Item1.myCamera.playerCamera = cameraTransform;
        _portals.Item1.myCamera.portal = _portals.Item1.transform;
        _portals.Item1.myCamera.otherPortal = _portals.Item2.transform;

        _portals.Item2.myCamera.transform.parent = null;
        _portals.Item2.myCamera.playerCamera = cameraTransform;
        _portals.Item2.myCamera.portal = _portals.Item2.transform;
        _portals.Item2.myCamera.otherPortal = _portals.Item1.transform;

        _portals.Item1.index = false;
        _portals.Item2.index = true;

        _portals.Item1.gunController = this;
        _portals.Item2.gunController = this;

        _portals.Item1.gameObject.SetActive(false);
        _portals.Item2.gameObject.SetActive(false);
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

    public void TeleportFrom(PortalBehavior newPortal, GameObject obj)
    {
        Transform newPortalTransform = newPortal.transform;
        Transform trans = obj.transform;

        // make it so that the new portal doesn't instantly teleport them back
        newPortal.IgnoreTPList.Add(obj);

        // disable the character controller if necessary
        if (obj.TryGetComponent<CharacterController>(out var cc))
        {
            cc.enabled = false;
        }

        // move player to new one
        trans.position = newPortalTransform.position;

        // align to new normal
        trans.forward = newPortalTransform.forward;

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
            objCollider.includeLayers |= (1 << _portalPlaneLayer);
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
                beh.renderPlane.material = portalMaterials[index ? 1 : 0];
            }
            else
            {
                // just use default material
                beh.renderPlane.material = defaultMaterial;
            }
        }
    }
}
