using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARBasePlacement : MonoBehaviour {
    [Header("AR References")]
    [SerializeField] private ARRaycastManager raycastManager;

    [Header("Game")]
    [SerializeField] private GameObject gameBasePrefab;

    [Header("Targets")]
    [SerializeField] private GameObject targetPrefab;

    private GameObject spawnedBase;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update() {
        // Don't allow another placement
        if (spawnedBase != null)
            return;

#if UNITY_EDITOR

        // -----------------------------
        // UNITY EDITOR TEST
        // -----------------------------

        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                PlaceBase(hit.point, Vector3.up);
            }
        }

#else

        // -----------------------------
        // REAL AR DEVICE
        // -----------------------------

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (raycastManager.Raycast(
            touch.position,
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            PlaceBase(
                hitPose.position,
                hitPose.up
            );
        }

#endif
    }

    private void PlaceBase(Vector3 position, Vector3 surfaceNormal) {
        spawnedBase = Instantiate(
            gameBasePrefab,
            position,
            Quaternion.LookRotation(
                Vector3.forward,
                surfaceNormal
            )
        );
        SpawnTargets(position);

        Debug.Log("Game Base Placed!");
    }

    private void SpawnTargets(Vector3 basePosition) {
        Vector3[] targetPositions =
        {
        basePosition + new Vector3(-0.6f, 1.2f, 0.5f),
        basePosition + new Vector3(0f, 1.6f, 0.8f),
        basePosition + new Vector3(0.6f, 1.2f, 0.5f)
    };

        foreach (Vector3 targetPosition in targetPositions) {
            Instantiate(
                targetPrefab,
                targetPosition,
                Quaternion.identity
            );
        }
    }
}