// SOURCE: https://github.com/daniel-ilett/portals-urp/blob/main/Assets/Scripts/PortalCamera.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
{
    public PortalGunController portalGunController;
    
    [SerializeField]
    private Camera portalCamera;
    public Camera mainCamera;
    [SerializeField]
    private int iterations = 7;

    [SerializeField]
    private PortalBehavior[] portals = new PortalBehavior[2];

    [SerializeField] private RenderTexture tempTexture1;
    [SerializeField] private RenderTexture tempTexture2;

    private void Awake()
    {
        tempTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        tempTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        UpdatePortalTexture();
    }

    public void UpdatePortalTexture()
    {
        portals[0].renderPlane.material.mainTexture = tempTexture1;
        portals[1].renderPlane.material.mainTexture = tempTexture2;
    }

    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

    public void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        if (!portals[0].IsPlaced || !portals[1].IsPlaced)
        {
            return;
        }

        if (portals[0].renderPlane.isVisible)
        {
            Debug.Log("render portal 0");
            portalCamera.targetTexture = tempTexture1;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[0], portals[1], i, SRC);
            }
        }

        if(portals[1].renderPlane.isVisible)
        {
            Debug.Log("render portal 1");
            portalCamera.targetTexture = tempTexture2;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[1], portals[0], i, SRC);
            }
        }
    }

    private void RenderCamera(PortalBehavior inPortal, PortalBehavior outPortal, int iterationID, ScriptableRenderContext SRC)
    {
        Transform inTransform = inPortal.renderPlane.transform;
        Transform outTransform = outPortal.renderPlane.transform;

        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;
        
        for (int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
    }


}
