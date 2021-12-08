using UnityEngine;
using UnityEngine.UI;

namespace AnchorManagerSimulator
{
    public class UIManager : MonoBehaviour
    {
        #region Fields

        public AnchorManager AnchorManager;

        public Canvas WorldCanvas;

        public Text DistanceText;

        [Tooltip("The vertical offsetdistance from the anchor and the text.")]
        public float YOffset;

        #endregion Fields

        #region Unity

        void Update()
        {
            if (AnchorManager.ClosestAnchor == null)
                return;


            if (CanRaycastAnchor() && HasRaycastHitClosestAnchor())
            {
                WorldCanvas.gameObject.SetActive(true);
                WorldCanvas.transform.position = new Vector3(AnchorManager.ClosestAnchor.transform.position.x, AnchorManager.ClosestAnchor.transform.position.y + YOffset, AnchorManager.ClosestAnchor.transform.position.z);
                WorldCanvas.transform.LookAt(AnchorManager.Camera.transform);

                DistanceText.text = AnchorManager.ClosestAnchor.DistanceToCamera.ToString();
            }
            else
                WorldCanvas.gameObject.SetActive(false);
        }

        #endregion Unity

        #region Methods

        /// <summary>
        /// Calculates if the closest anchor is within the field of view.
        /// </summary>
        /// <returns></returns>
        bool CanRaycastAnchor()
        {
            Vector3 anchorDirectionToCamera = AnchorManager.ClosestAnchor.transform.position - AnchorManager.Camera.transform.position;

            var dot = Vector3.Dot(AnchorManager.Camera.transform.forward, anchorDirectionToCamera.normalized);

            return (dot > 0.7f);
        }

        /// <summary>
        /// Calculates if the raycast from the screen crossair has hit the closest anchor.
        /// </summary>
        /// <returns></returns>
        bool HasRaycastHitClosestAnchor()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return (hit.collider == AnchorManager.ClosestAnchor.Collider);
            }

            return false;
        }

        #endregion
    }
}