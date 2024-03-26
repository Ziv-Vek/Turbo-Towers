using System;
using System.Collections;
using DG.Tweening;
using Sirenix.Utilities;
using TurboTowers.Core;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraMover : MonoBehaviour
{
    // Config:
    [SerializeField] float smoothSpeed = 0.125f; // Adjust for smoother movement
    [SerializeField] private float dollyingSpeed = 3f;
    public CameraConfigSO cameraConfig;
    public List<Transform> dollyPoints;

    // Caching:
    public Transform Player { get; private set; }

    // The stationary turret
    public OffsetData offsetData; // Offset from the turret // public for debugging
    private float fixedHeightDifference; // Fixed height difference from the turret

    // State:
    private bool isDollyMovement;

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player").transform.Find("Head")?.transform.Find("Head_Rot");
        isDollyMovement = false;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        offsetData = cameraConfig.GetOffset();
        if (Player)
        {
            fixedHeightDifference = transform.position.y - Player.position.y;
        }
    }

    void LateUpdate()
    {
        if (isDollyMovement == true) return;

        if (Player == null) return;

        // Calculate the desired position with offset
        var desiredPosition = OffsettedPosition();

        // Smoothly interpolate to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera position
        transform.position = smoothedPosition;

        // Make sure the camera always faces the same direction as the turret
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(transform.eulerAngles.x, Player.eulerAngles.y, Player.eulerAngles.z), smoothSpeed);
    }

    private Vector3 OffsettedPosition()
    {
        if (Player == null) return transform.position;
        Vector3 pos = Player.position + Player.TransformDirection(offsetData.posOffset);
        pos.y = Player.position.y + fixedHeightDifference;
        return pos;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.InGameCutscene)
        {
            if (dollyPoints.Count < 1)
            {
                Debug.LogWarning("Not enough dolly points are set up for the camera");
                isDollyMovement = false;
                GameManager.Instance.OnCameraDollyFinished();
                return;
            }

            isDollyMovement = true;
            /*transform.SetPositionAndRotation(dollyPoints[0].position, Quaternion.identity);*/
            StartCoroutine(DollyMovement());
        }
        else if (state == GameState.InGame)
        {
            isDollyMovement = false;
        }
    }

    private IEnumerator DollyMovement()
    {
        foreach (Transform point in dollyPoints)
        {
            yield return transform
                .DOMove(point.position, Vector3.Distance(point.position, transform.position) / dollyingSpeed)
                .SetEase(Ease.Linear).WaitForCompletion();
        }

        var desiredPositionFromTower = OffsettedPosition();
        yield return transform
            .DOMove(desiredPositionFromTower,
                Vector3.Distance(desiredPositionFromTower, transform.position) / dollyingSpeed).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.position = OffsettedPosition();
                GameManager.Instance.OnCameraDollyFinished();
            });


        /*
        for (int i = 0; i < dollyPoints.Count; i++)
        {
            Debug.Log("dollying count : " + i);
            transform.DOMove(dollyPoints[i].position,
                Vector3.Distance(dollyPoints[i].position, transform.position) /
                dollyingSpeed).SetEase(Ease.Linear);
        }

        var desiredPositionFromTower = OffsettedPosition();
        transform.DOMove(desiredPositionFromTower,
            Vector3.Distance(desiredPositionFromTower, transform.position) /
            dollyingSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.position = offsetData.posOffset;
                GameManager.Instance.OnCameraDollyFinished();
            }
        );

        yield return null;      */
    }

#if UNITY_EDITOR
    public void SetOffset()
    {
        var player = GameObject.FindWithTag("Player").transform.Find("Head")?.transform.Find("Head_Rot");
        if (player != null)
        {
            var offsetData = new OffsetData(
                transform.localPosition - player.localPosition,
                player.rotation,
                transform.position.y - player.position.y
            );
            cameraConfig.SetOffset(ref offsetData);
            
            EditorUtility.SetDirty(cameraConfig);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}

public struct OffsetData
{
    public readonly Vector3 posOffset;
    public readonly Quaternion rotation;
    public readonly float heightDifference;

    public OffsetData(Vector3 posOffset, Quaternion rotation, float heightDifference)
    {
        this.posOffset = posOffset;
        this.rotation = rotation;
        this.heightDifference = heightDifference;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraMover))]
public class CameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draws the default inspector

        CameraMover cameraMover = (CameraMover)target;

        GUILayout.Label("Camera Config", EditorStyles.boldLabel);
        GUILayout.Space(3);
        if (GUILayout.Button("Set New Offset"))
        {
            cameraMover.SetOffset();
        }

        GUILayout.Space(10);
        GUILayout.Label("Camera Mover", EditorStyles.boldLabel);
        GUILayout.Space(3);
        if (GUILayout.Button("Set Dolly Points", GUILayout.Width(200f)))
        {
            cameraMover.dollyPoints.Add(cameraMover.transform);
        }

        if (GUILayout.Button("Clear Dolly Points", GUILayout.Width(200f)))
        {
            cameraMover.dollyPoints.Clear();
        }
    }
}
#endif