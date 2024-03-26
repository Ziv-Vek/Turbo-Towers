using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System.Linq;
using Sirenix.Utilities;
using TurboTowers.Turrets.Combat;
using TurboTowers.Turrets.Controls;

public class TrajectorySimulator : MonoBehaviour {
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations = 100;
    [SerializeField] private List<Transform> _obstacles;
    [SerializeField] private LayerMask interacteableLayer;

    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    public Projectile projectile;
    private Vector3 turretExitPosition;
    private Vector3 _velocity;
    
    private bool _isSimulating;

    private Projectile ghostProjectile;
    private InputStyle inputStyle = InputStyle.First;

    IEnumerator Start()
    {
        projectile = GetComponent<PlayerAttacker>().GetProjectile();
        turretExitPosition = GetComponent<PlayerAttacker>().GetTurretExitPosition();
        _velocity = GetComponent<PlayerAttacker>().GetShootingVelocity();
        
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(PopulateObstacles());
        
        yield return StartCoroutine(CreatePhysicsScene());

        ghostProjectile = Instantiate(projectile, turretExitPosition, Quaternion.identity);
        ghostProjectile.GetComponentsInChildren<Renderer>().ForEach(renderer => renderer.enabled = false);
        SceneManager.MoveGameObjectToScene(ghostProjectile.gameObject, _simulationScene);
        _isSimulating = true;
        //SimulateTrajectory(projectile, turretExitPosition, _velocity);
    }

    IEnumerator PopulateObstacles()
    {
        List<int> layers = new List<int>();
        
        for (int i = 0; i < 32; i++)
        {
            // Check if this layer is in the interactableLayer mask
            if (((1 << i) & interacteableLayer.value) != 0)
            {
                Debug.Log("Interactable Layer: " + LayerMask.LayerToName(i) + " (Layer Number: " + i + ")");

                layers.Add(i);
            }
        }
        
        //GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject go in allObjects)
        { 
            if (layers.Contains(go.layer))
            {
                if (go.tag == "Player") continue;
                _obstacles.Add(go.transform);
                Debug.Log("Added: " + go.name + " to obstacles");
            }
        }

        yield return null;
    }

    private IEnumerator CreatePhysicsScene() {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform obj in _obstacles) {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            var renderers = ghostObj.GetComponentsInChildren<Renderer>();
            renderers.ForEach(renderer => renderer.enabled= false);
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }

        yield return null;
    }

    private void Update() {
        foreach (var item in _spawnedObjects) {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (inputStyle != InputStyle.Third) return;
        
        if (_isSimulating)
            SimulateTrajectory(projectile, turretExitPosition, _velocity);
    }

    /// <summary>
    /// Simulates the trajectory of the projectile
    /// </summary>
    /// <param name="ballPrefab">The projectile</param>
    /// <param name="pos">The position in which we start in</param>
    /// <param name="velocity">The shooting velocity</param>
    public void SimulateTrajectory(Projectile ballPrefab, Vector3 pos, Vector3 velocity) {
        //var ghostProjectile = Instantiate(ballPrefab, pos, Quaternion.identity);
        //ghostProjectile.GetComponentsInChildren<Renderer>().ForEach(renderer => renderer.enabled = false);
        //SceneManager.MoveGameObjectToScene(ghostProjectile.gameObject, _simulationScene);

        ghostProjectile.transform.position = pos;
        ghostProjectile.SimulateFire(velocity, true);

        _line.positionCount = _maxPhysicsFrameIterations;

        for (var i = 0; i < _maxPhysicsFrameIterations; i++) {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostProjectile.transform.position);
        }

        //Destroy(ghostProjectile.gameObject);
    }
}