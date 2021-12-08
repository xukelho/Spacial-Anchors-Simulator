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
        [HideInInspector]
        public Vector3 GlobalPosition;

        /// <summary>
        /// Global rotation of the object when was first instantiated.
        /// </summary>
        [HideInInspector]
        public Quaternion GlobalRotation;

        /// <summary>
        /// Anchor Manager / Main Camera reference
        /// </summary>
        public AnchorManager AnchorManager;

        int _frameIgnoreCounter = 0;

        #endregion Fields

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

        /// <summary>
        /// Returns if the object is within the immediate recalculation range.
        /// </summary>
        /// <returns> Yes/No. </returns>
        bool IsWithinImmediateCalculationDistance()
        {
            Vector3 offset = AnchorManager.Camera.transform.position - transform.position;
            float sqrLen = offset.sqrMagnitude;
            
            // square the distance we compare with
            bool isWithinMargin = (sqrLen < AnchorManager.DistanceToCalculateRepositionOfAnchorsImmediatly * AnchorManager.DistanceToCalculateRepositionOfAnchorsImmediatly);

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