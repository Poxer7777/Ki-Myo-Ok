using JetBrains.Annotations;
using System;
using UnityEngine;

public class SensingSystem : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask obstacleMask;

    // TEST
    public GameObject targetObject;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
    }

    public bool CanSeeObject(GameObject target)
    {
        Renderer targetRenderer = target.GetComponent<Renderer>();

        if (targetRenderer == null)
        {
            Debug.LogWarning("Object의 Renderer가 감지되지 않았습니다.");
            return false;
        }

        Bounds bounds = targetRenderer.bounds; // Object의 범위
        Vector3 cameraPosition = playerCamera.transform.position; // 카메라의 위치

        // 1. Out of Sight
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, bounds))
        {
            Debug.Log("Object가 화면 범위 밖에 있습니다.");
            return false;
        }

        // 2. Distance Check
        Vector3 closestPoint = bounds.ClosestPoint(cameraPosition); // Object 범위 중 가장 가까운 점(Point)
        float distance = Vector3.Distance(cameraPosition, closestPoint);

        if (distance > maxDistance)
        {
            Debug.Log("Object가 감지 거리 밖에 있습니다.");
            return false;
        }

        // 3. 'Object is hidden' Check
        Vector3 targetPoint = bounds.center;
        Vector3 direction = targetPoint - cameraPosition;
        float targetDistance = direction.magnitude;

        if (!HasLineOfSight(bounds))
        {
            Debug.Log("Object가 가려져있습니다.");
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

    private void Update()
    {
        CanSeeObject(targetObject);
    }
}