using System.Threading.Tasks;
using UnityEngine;

namespace AnchorManagerSimulator
{
    public class Anchor : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Global position of the object when was first instantiated. To be removed. Avoid usage.
        /// </summary>
        public Vector3 GlobalPosition;

        /// <summary>
        /// Local position of the object when was first instantiated.
        /// </summary>
        public Vector3 LocalPosition;

        /// <summary>
        /// Local rotation of the object when was first instantiated.
        /// </summary>
        public Quaternion LocalRotation;

        /// <summary>
        /// Global position of the camera when the object was first instantiated.
        /// </summary>
        public Vector3 CameraGlobalPosition;

        /// <summary>
        /// Global rotation of the camera when the object was first instantiated.
        /// </summary>
        public Quaternion CameraGlobalRotation;

        /// <summary>
        /// Anchor Manager / Main Camera reference
        /// </summary>
        public AnchorManager AnchorManager;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Recalculates the Anchor transform, relative to the camera, to keep it in place in the World.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAnchor()
        {
            //transform.localPosition = AnchorManager.ConvertWorldToLocalPoint(GlobalPosition);
            //transform.rotation = Quaternion.identity;

            CalculateNewLocalPosition();

            await Task.Yield();
        }

        void CalculateNewLocalPosition()
        {
            Vector3 cameraMovedOffset = AnchorManager.transform.position - CameraGlobalPosition;
            Vector3 newLocalPosition = cameraMovedOffset + LocalPosition;
            transform.localPosition = newLocalPosition;
        }

        #endregion Methods
    }
}