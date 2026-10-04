using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class ARBasePlacement : MonoBehaviour {
    [SerializeField] TextMeshProUGUI planeDetected;
    [Header("AR References")]
    [SerializeField] private ARRaycastManager raycastManager;

    [Header("Game")]
    [SerializeField] private GameObject gameBasePrefab;

    [Header("Targets")]
    [SerializeField] private GameObject targetPrefab;

    private GameObject spawnedBase;
    [SerializeField] private ARAnchorManager anchorManager;
    private static readonly List<ARRaycastHit> hits =
        new List<ARRaycastHit>();

    private void Update() {
        // Base can only be placed once
        if (spawnedBase != null)
            return;

        // Check Input System touchscreen
        if (Touchscreen.current == null) {
            return;
        }

        // Detect a new tap
        if (!Touchscreen.current.primaryTouch.press.wasPressedThisFrame) {
            return;
        }

        Vector2 touchPosition =
            Touchscreen.current.primaryTouch.position.ReadValue();

        Debug.Log("SCREEN TAPPED: " + touchPosition);
        planeDetected.text = "SCREEN TAPPED: " + touchPosition;

        // Check Raycast Manager
        if (raycastManager == null) {
            Debug.LogError("ERROR: ARRaycastManager is NOT assigned!");
            return;
        }

        // Raycast against detected AR surfaces
        if (raycastManager.Raycast(touchPosition, hits)) {
            Pose hitPose = hits[0].pose;

            Debug.Log(
                "AR PLANE HIT! Position: " +
                hitPose.position
            );


            PlaceBase(
                hitPose.position,
                hitPose.up
            );
        } else {
            Debug.Log(
                "AR RAYCAST MISS - No detected plane at tap location."
            );
        }
    }

    private void PlaceBase(
    Vector3 position,
    Vector3 surfaceNormal) {
        if (gameBasePrefab == null) {
            Debug.LogError("ERROR: Game Base Prefab is NOT assigned!");
            return;
        }

        Quaternion rotation = Quaternion.LookRotation(
            Vector3.forward,
            surfaceNormal
        );

        spawnedBase = Instantiate(
            gameBasePrefab,
            position,
            rotation
        );

        // Add AR Anchor to the placed GameBase
        ARAnchor anchor = spawnedBase.GetComponent<ARAnchor>();

        if (anchor == null) {
            anchor = spawnedBase.AddComponent<ARAnchor>();
        }

        Debug.Log("GAME BASE SPAWNED AND ANCHORED!");
        planeDetected.text = "GAME BASE SPAWNED AND ANCHORED";

        SpawnTargets(spawnedBase.transform);
    }

    private void SpawnTargets(Transform baseTransform) {
        if (targetPrefab == null) {
            Debug.LogError(
                "ERROR: Target Prefab is NOT assigned!"
            );
            return;
        }

        Vector3[] localPositions =
        {
            new Vector3(-0.6f, 1.2f, 0.5f),
            new Vector3(0f, 2f, 0.8f),
            new Vector3(0.6f, 1.2f, 0.5f)
        };

        foreach (Vector3 localPosition in localPositions) {
            GameObject target = Instantiate(
                targetPrefab,
                baseTransform.TransformPoint(localPosition),
                baseTransform.rotation
            );

            target.transform.SetParent(baseTransform);
        }

        Debug.Log("3 TARGETS SPAWNED");
        planeDetected.text = "3 TARGETS SPAWNED";

        if (GameManager.Instance != null) {
            GameManager.Instance.SetTargetCount(3);
        }
    }
}