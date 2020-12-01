using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal otherPortal;

    public Transform normalVisible;
    public Transform normalInvisible;

    public Camera portalCamera;
    public Renderer viewThroughRenderer;
    private RenderTexture viewThroughRenderTexture;
    private Material viewThroughMaterial;

    private Camera mainCamera;

    private Vector4 vectorPlane;

    // Start is called before the first frame update
    void Start()
    {
        viewThroughRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        viewThroughRenderTexture.Create();

        viewThroughMaterial = viewThroughRenderer.material;
        viewThroughMaterial.mainTexture = viewThroughRenderTexture;

        portalCamera.targetTexture = viewThroughRenderTexture;

        mainCamera = Camera.main;

        // Generate bounding plane

        var plane = new Plane(normalVisible.forward, transform.position);
        vectorPlane = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        // calculate portal camera position and rotation

        var virtualPosition = TransformPositionBetweenPortals(this, otherPortal, mainCamera.transform.position);
        var virtualRotation = TransformRotationBetweenPortals(this, otherPortal, mainCamera.transform.rotation);

        portalCamera.transform.SetPositionAndRotation(virtualPosition, virtualRotation);

        // Calculate projection matrix

        var clipThroughSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * otherPortal.vectorPlane;

        // Set portal camera projection matrix to clip walls between target portal and portal camera
        // Inherits main camera near/far clip plane and FOV settings

        var obliqueProjectionMatrix = mainCamera.CalculateObliqueMatrix(clipThroughSpace);
        portalCamera.projectionMatrix = obliqueProjectionMatrix;

        portalCamera.Render();
    }

    private void OnDestroy()
    {
        viewThroughRenderTexture.Release();

        Destroy(viewThroughMaterial);
        Destroy(viewThroughRenderTexture);
    }

    public static Vector3 TransformPositionBetweenPortals(Portal sender, Portal target, Vector3 position)
    {
        return target.normalInvisible.TransformPoint(sender.normalVisible.InverseTransformPoint(position));
    }

    public static Quaternion TransformRotationBetweenPortals(Portal sender, Portal target, Quaternion rotation)
    {
        return target.normalInvisible.rotation * Quaternion.Inverse(sender.normalVisible.rotation) * rotation;
    }
}
