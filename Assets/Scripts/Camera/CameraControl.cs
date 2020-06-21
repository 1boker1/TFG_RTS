using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;


public class CameraControl : MonoBehaviour
{
    [SerializeField] private int scrollSpeed = 75;
    [SerializeField] private int maxZoom = 30;
    [SerializeField] private int minZoom = 100;
    [SerializeField] private int levelArea = 10000;

    [SerializeField] private LineRenderer[] lines;

    private int ScrollArea = 25;
    private int layerMask;
    private int residualNormal = 1;

    private float residualSpeed = 0;

    private Camera mainCamera;

    Vector3[] worldPoints = new Vector3[4];

    private void Start()
    {
        layerMask = LayerMask.GetMask("MiniMap");
        mainCamera = Camera.main;
    }

    private void Update()
    {
        MouseMovement();
        Zoom();
        CameraRectangle();

        if (Input.GetKey("space") && SelectionManager.SelectedEntities.Count!=0)
            CenterCamera();
    }

    private void MouseMovement()
    {
        Vector3 translation = Vector3.zero;

        float mult = ((mainCamera.transform.localPosition.y - maxZoom) / (minZoom - maxZoom));

        if (mult < 0.35f) mult = 0.35f;

        float desiredSpeed = scrollSpeed * mult * Time.deltaTime;

        if (Input.mousePosition.x < ScrollArea) translation += Vector3.right * -desiredSpeed;
        if (Input.mousePosition.y < ScrollArea) translation += Vector3.forward * -desiredSpeed;
        if (Input.mousePosition.x >= Screen.width - ScrollArea) translation += Vector3.right * desiredSpeed;
        if (Input.mousePosition.y >= Screen.height - ScrollArea) translation += Vector3.forward * desiredSpeed;

        Vector3 desiredPosition = mainCamera.transform.localPosition + translation;

        translation = Quaternion.Euler(0, mainCamera.transform.rotation.eulerAngles.y, 0) * translation;

        mainCamera.transform.localPosition += translation;
    }

    private void Zoom()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (scrollWheel != 0f)
        {
            residualSpeed = 0.30f;
            residualNormal = scrollWheel < 0 ? 1 : -1;
        }

        residualSpeed = residualSpeed > 0 ? residualSpeed - Time.deltaTime : 0;

        float speed = (residualSpeed * residualNormal) - (15 * scrollWheel);

        Vector3 newPosition = mainCamera.transform.localPosition + mainCamera.transform.forward * -speed;

        if (newPosition.y < minZoom && newPosition.y > maxZoom)
            mainCamera.transform.localPosition = newPosition;
    }

    private void CameraRectangle()
    {
        if (lines.Length == 0) return;

        Ray[] rays = new Ray[4];

        rays[0] = mainCamera.ScreenPointToRay(new Vector3(0, 0, 0));
        rays[1] = mainCamera.ScreenPointToRay(new Vector3(0, Screen.height, 0));
        rays[2] = mainCamera.ScreenPointToRay(new Vector3(Screen.width, 0, 0));
        rays[3] = mainCamera.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0));

        for (int i = 0; i < rays.Length; i++)
        {
            RaycastHit hit;

            if (Physics.Raycast(rays[i], out hit, 9000, layerMask))
            {
                if (hit.transform != null)
                    worldPoints[i] = hit.point + new Vector3(0, 60, 0);
            }
        }

        float distance = mainCamera.transform.position.y / Mathf.Sin(90 - mainCamera.transform.rotation.eulerAngles.x);

        Vector3[] frustumCorners = new Vector3[4]; // will hold the result
        mainCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), distance, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        Vector3[] worldCorners = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            worldCorners[i] = mainCamera.transform.TransformVector(frustumCorners[i]);
        }

        lines[0].SetPosition(0, worldPoints[0].With(y: 50));
        lines[0].SetPosition(1, worldPoints[1].With(y: 50));
        lines[1].SetPosition(0, worldPoints[1].With(y: 50));
        lines[1].SetPosition(1, worldPoints[3].With(y: 50));
        lines[2].SetPosition(0, worldPoints[3].With(y: 50));
        lines[2].SetPosition(1, worldPoints[2].With(y: 50));
        lines[3].SetPosition(0, worldPoints[2].With(y: 50));
        lines[3].SetPosition(1, worldPoints[0].With(y: 50));
    }

    public void CenterCamera()
    {
        Vector3 centerOfMass = Utils.CenterOfMass(SelectionManager.SelectedEntities);

        float distance = transform.position.y / Mathf.Sin(90 - transform.rotation.eulerAngles.x);

        transform.position = (centerOfMass + (-transform.forward * distance)).With(y: transform.position.y);
    }

    public static void CenterCamera(Vector3 point)
    {
        Transform _Camera = Camera.main.transform;

        float distance = _Camera.position.y / Mathf.Sin(90 - _Camera.rotation.eulerAngles.x);

        _Camera.position = (point + (-_Camera.forward * distance)).With(y: _Camera.position.y);
    }
}
