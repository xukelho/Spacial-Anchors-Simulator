using System.Threading.Tasks;
using UnityEngine;

namespace AnchorManagerSimulator
{
    public class Anchor : MonoBehaviour
    {
        #region Fields

        public float DistanceToCamera = Mathf.Infinity;

        /// <summary>
        /// Global position of the object when was first instantiated. To be removed. Avoid usage.
        /// </summary>
        [HideInInspector]
        public Vector3 GlobalPosition;

        /// <summary>
        /// Global rotation of the object when was first instantiated.
        /// </summary>
        [HideInInspector]
        public Quaternion GlobalRotation;

        [HideInInspector]
        public Collider Collider;

        /// <summary>
        /// Anchor Manager / Main Camera reference
        /// </summary>
        public AnchorManager AnchorManager;

        int _frameIgnoreCounter = 0;

        MeshRenderer _meshRenderer;

        #endregion Fields

        #region Unity

        void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            Collider = GetComponent<Collider>();
        }

        void Start()
        {
            UpdateDistanceToCamera();
        }

        #endregion Unity

        #region Methods

        /// <summary>
        /// Recalculates the Anchor transform, relative to the camera, to keep it in place in the World.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAnchor()
        {
            if (!CanCalculateReposition())
                return;

            transform.position = GlobalPosition;
            transform.rotation = GlobalRotation;

            await Task.Yield();
        }

        public void ChangeMaterial(Material mat)
        {
            _meshRenderer.material = mat;
        }

        public void UpdateDistanceToCamera()
        {
            //Usually, the square product is more effient to find the closest distance
            //but in this case, since the distance is needed to be stored to be accessed later
            //Vector3.Distance is the only choice
            DistanceToCamera = Vector3.Distance(AnchorManager.Camera.transform.position, transform.position);
        }

        /// <summary>
        /// Returns if the object is within the immediate recalculation range.
        /// </summary>
        /// <returns> Yes/No. </returns>
        bool IsWithinImmediateCalculationDistance()
        {
            bool isWithinMargin = (DistanceToCamera <= AnchorManager.DistanceToCalculateRepositionOfAnchorsImmediatly);

            return isWithinMargin;
        }

        /// <summary>
        /// Calculates if the object can recalculate its position.
        /// </summary>
        /// <returns> Yes/No. </returns>
        bool CanCalculateReposition()
        {
            if (!IsWithinImmediateCalculationDistance())
            {
                if (_frameIgnoreCounter != AnchorManager.FrameIgnoreValue)
                {
                    _frameIgnoreCounter++;
                    return false;
                }
                else
                    _frameIgnoreCounter = 0;
            }

            return true;
        }

        #endregion Methods
    }
}