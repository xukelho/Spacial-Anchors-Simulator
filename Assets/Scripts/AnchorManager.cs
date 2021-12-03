using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AnchorManagerSimulator
{
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

        Vector3 _lastFrameCameraPosition;
        Quaternion _lastFrameCameraRotation;

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

            if (_lastFrameCameraPosition == transform.position && _lastFrameCameraRotation == transform.rotation)
                return;

            UpdateAnchors();

            _lastFrameCameraPosition = transform.position;
            _lastFrameCameraRotation = transform.rotation;
        }

        #endregion Unity

        #region Methods

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
                anchor.transform.position = hit.point;
                anchor.transform.LookAt(new Vector3(transform.position.x, anchor.transform.position.y, transform.position.z));

                anchor.GlobalPosition = hit.point;
                anchor.CameraGlobalPosition = transform.position;
                anchor.CameraGlobalRotation = transform.rotation;
                anchor.LocalPosition = anchor.transform.localPosition;
                anchor.LocalRotation = anchor.transform.localRotation;
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
}