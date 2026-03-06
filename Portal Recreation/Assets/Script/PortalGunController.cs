using System;
using UnityEngine;

public class PortalGunController : MonoBehaviour
{
    public GameObject portalPrefab;
    public Transform portalHolder, shootPoint, cameraTransform;

    [SerializeField] private (GameObject, GameObject) _portals;
    [SerializeField] private float maxPortalDistance;
    [SerializeField] private LayerMask portalLayerMask;

    private void Start()
    {
        _portals.Item1 = Instantiate(portalPrefab, portalHolder);
        _portals.Item2 = Instantiate(portalPrefab, portalHolder);

        _portals.Item1.SetActive(false);
        _portals.Item2.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootPortal(0);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ShootPortal(1);
        }
    }

    private void ShootPortal(int index)
    {
        GameObject thePortal = index == 0 ? _portals.Item1 : _portals.Item2;
        // raycast out to a surface
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, cameraTransform.forward, out hit, maxPortalDistance, portalLayerMask))
        {
            // TODO: check if there's enough space

            // move portal to the new location
            thePortal.SetActive(true);
            thePortal.transform.position = hit.point;

            // aligned to new normal
            thePortal.transform.forward = hit.normal;
        }
    }
}
