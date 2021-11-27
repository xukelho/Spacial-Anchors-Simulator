using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnchorManager : MonoBehaviour
{
    #region Fields

    [Tooltip("How often, in seconds, to recalculate all anchors positions.")]
    public float AnchorUpdateInterval = .1f;

    [Header("Prefab References")]
    public Anchor PrefabAnchor;

    public Vector3 StartingWorldPosition { get; private set; }

    Camera _camera;

    List<Anchor> _anchors = new List<Anchor>();

    #endregion Fields

    #region Unity

    void Awake()
    {
        _camera = GetComponent<Camera>();
        StartingWorldPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            SpawnAnchor();

        UpdateAnchors();
    }

    #endregion Unity

    #region Methods

    //TODO
    public Vector3 ConvertWorldToLocalPoint(Vector3 point)
    {
        //Quaternion rotation = Quaternion.FromToRotation(_camera.transform.forward, Vector3.forward);
        //float distance = Vector3.Distance(_startingWorldPosition, point);
        //Vector3 rotatedPoint = rotation * point;
        //Vector3 newLocalPoint = rotatedPoint - _startingWorldPosition;
        //print(newLocalPoint);

        Vector3 localPoint = _camera.transform.InverseTransformPoint(point);
        return localPoint;
    }

    /// <summary>
    /// Spawns an Anchor in the intersection of a ray from the center of the screen, to the first collider hit.
    /// </summary>
    void SpawnAnchor()
    {
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Anchor anchor = Instantiate(PrefabAnchor);
            _anchors.Add(anchor);

            anchor.transform.parent = _camera.transform;
            anchor.transform.localPosition = ConvertWorldToLocalPoint(hit.point);
            anchor.Position = hit.point;
            anchor.AnchorManager = this;

            anchor.name = "Anchor " + _anchors.Count;
        }
    }

    /// <summary>
    /// Controls the update of all anchors.
    /// </summary>
    async void UpdateAnchors()
    {
        await Task.Yield();

        var tasks = new List<Task>();

        for (int i = 0; i < _anchors.Count; i++)
            tasks.Add(_anchors[i].UpdateAnchor());

        await Task.WhenAll(tasks);
    }

    #endregion Methods
}