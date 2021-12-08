using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AnchorManagerSimulator
{
    public class AnchorManager : MonoBehaviour
    {
        #region Fields

        [Space(20)]
        public Anchor ClosestAnchor;

        [Space(20)]
        [Tooltip("Set the distance to when an anchor position is calculated immediatly, or after a set interval.")]
        public float DistanceToCalculateRepositionOfAnchorsImmediatly = 3;

        [Tooltip("Set the number of frames that will be skipped until the anchors that are far away can be calculated again.")]
        public int FrameIgnoreValue = 10;

        [Header("Scene References")]
        [Space(20)]
        public Camera Camera;

        [Header("Material References")]
        [Space(20)]
        public Material MaterialAnchorDefault;

        public Material MaterialAnchorClosest;

        [Header("Prefab References")]
        [Space(20)]
        public Anchor PrefabAnchor;

        List<Anchor> _anchors = new List<Anchor>();

        Vector3 _lastFrameCameraPosition;
        Quaternion _lastFrameCameraRotation;

        #endregion Fields

        #region Unity

        void Update()
        {
            HandleSpawnOfAnchors();
            HandleUpdateOfAnchors();
        }

        #endregion Unity

        #region Methods

        /// <summary>
        /// Manages when an anchor can be spawned.
        /// </summary>
        void HandleSpawnOfAnchors()
        {
            if (Input.GetMouseButtonDown(0))
                SpawnAnchor();
        }

        /// <summary>
        /// Manages when anchors can be updated.
        /// </summary>
        void HandleUpdateOfAnchors()
        {
            var lastFrameCameraPosition = _lastFrameCameraPosition;
            var lastFrameCameraRotation = _lastFrameCameraRotation;

            _lastFrameCameraPosition = Camera.transform.position;
            _lastFrameCameraRotation = Camera.transform.rotation;

            //Verify if the camera has moved/rotated
            if (lastFrameCameraPosition == Camera.transform.position &&
                lastFrameCameraRotation.x == Camera.transform.rotation.x && //Comparing lastFrameCameraRotation == Camera.transform.rotation
                lastFrameCameraRotation.y == Camera.transform.rotation.y && //isn't providing the correct results so each value must be
                lastFrameCameraRotation.z == Camera.transform.rotation.z && //compared separatly
                lastFrameCameraRotation.w == Camera.transform.rotation.w)
                return;

            if (_anchors.Count != 0)
                UpdateAnchors();
        }

        /// <summary>
        /// Spawns an Anchor in the intersection of a ray from the center of the screen, to the first collider hit.
        /// </summary>
        void SpawnAnchor()
        {
            Ray ray = Camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Anchor anchor = Instantiate(PrefabAnchor);
                _anchors.Add(anchor);

                anchor.transform.parent = Camera.transform;
                anchor.transform.position = hit.point;
                anchor.transform.LookAt(new Vector3(transform.position.x, anchor.transform.position.y, transform.position.z));

                anchor.GlobalPosition = hit.point;
                anchor.GlobalRotation = anchor.transform.rotation;
                anchor.AnchorManager = this;

                anchor.name = "Anchor " + _anchors.Count;

                if (ClosestAnchor == null)
                {
                    ClosestAnchor = anchor;
                    ClosestAnchor.ChangeMaterial(MaterialAnchorClosest);
                }
            }
        }

        /// <summary>
        /// Controls the update of all anchors.
        /// </summary>
        async void UpdateAnchors()
        {
            await Task.Yield();

            var tasks = new List<Task>();

            ClosestAnchor.UpdateDistanceToCamera();
            tasks.Add(ClosestAnchor.UpdateAnchor());

            for (int i = 0; i < _anchors.Count; i++)
                if (_anchors[i] != ClosestAnchor)
                {
                    Anchor anchor = _anchors[i];
                    anchor.UpdateDistanceToCamera();
                    tasks.Add(anchor.UpdateAnchor());

                    if (anchor.DistanceToCamera < ClosestAnchor.DistanceToCamera)
                    {
                        anchor.ChangeMaterial(MaterialAnchorClosest);
                        ClosestAnchor.ChangeMaterial(MaterialAnchorDefault);
                        ClosestAnchor = anchor;
                    }
                }

            await Task.WhenAll(tasks);
        }

        #endregion Methods
    }
}