using System.Threading.Tasks;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Spawned position relative to Camera.
    /// </summary>
    public Vector3 Position;

    public AnchorManager AnchorManager;

    #endregion Fields

    #region Methods

    /// <summary>
    /// Recalculates the Anchor transform, relative to the camera, to keep it in place in the World.
    /// </summary>
    /// <returns></returns>
    public async Task UpdateAnchor()
    {
        transform.localPosition = AnchorManager.ConvertWorldToLocalPoint(Position);
        transform.rotation = Quaternion.identity;

        await Task.Yield();
    }

    #endregion Methods
}