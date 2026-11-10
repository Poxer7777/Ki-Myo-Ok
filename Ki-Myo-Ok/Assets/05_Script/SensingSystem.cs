using System.Collections.Generic;
using UnityEngine;

public class SensingSystem : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask obstacleMask;

    // TEST
    [SerializeField] private GameObject[] targetObjects;
    private List<GameObject> visibleObjects = new List<GameObject>();

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        foreach (GameObject target in targetObjects)
        {
            if (target == null)
                continue;

            if (CanSeeObject(target))
            {
                visibleObjects.Add(target);
            }
        }
    }

    public bool CanSeeObject(GameObject target)
    {
        Renderer targetRenderer = target.GetComponentInChildren<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogWarning("Object의 Renderer가 감지되지 않았습니다.");
            return false;
        }

        Bounds bounds = targetRenderer.bounds; // Object의 범위
        Vector3 cameraPosition = playerCamera.transform.position; // 카메라의 위치

        // 1. 이상현상이 거리 내에 있는지 Check
        Vector3 closestPoint = bounds.ClosestPoint(cameraPosition); // Object 범위 중 가장 가까운 점(Point)
        float distance = Vector3.Distance(cameraPosition, closestPoint);

        if (distance > maxDistance)
        {
            return false;
        }

        // 2. 이상현상이 플레이어 화면 범위 내에 있는지 Check
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, bounds))
        {
            return false;
        }

        // 3. 이상현상이 다른 오브젝트에 의해 가려지는지 Check
        Vector3 targetPoint = bounds.center;
        Vector3 direction = targetPoint - cameraPosition;
        float targetDistance = direction.magnitude;

        if (!HasLineOfSight(bounds))
        {
            return false;
        }

        return true;
    }

    private bool HasLineOfSight(Bounds bounds)
    {
        Vector3 origin = playerCamera.transform.position;

        // Check : 정육면체 모든 면의 중심 및 정육면체의 중심 7개 점
        Vector3[] points =
        {
            bounds.center
        };

        foreach (Vector3 point in points)
        {
            Vector3 direction = point - origin;
            float distance = direction.magnitude;

            if (!Physics.Raycast(origin, direction.normalized, distance, obstacleMask)) // 4번째 인수로 obstacleMask (시야를 가릴 수 있는 레이어)
                return true;
        }

        return false;
    }
}