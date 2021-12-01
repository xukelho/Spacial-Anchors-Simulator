using System.Threading.Tasks;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    #region Fields

    public Vector3 GlobalPosition;
    public Vector3 LocalPosition;
    public Vector3 CameraPosition;
    public Quaternion CameraRotation;
    public Quaternion Rotation;

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
        CalculateNewLocalPosition();
        transform.rotation = Quaternion.identity;

        await Task.Yield();
    }

    void CalculateNewLocalPosition()
    {
        Matrix4x4 matrixExample = Matrix4x4.identity;

        matrixExample.SetTRS(LocalPosition, CameraRotation, Vector3.one);

        Vector3 localSpacePosition = matrixExample.inverse.MultiplyPoint3x4(GlobalPosition);
        transform.localPosition = localSpacePosition;

        //-----------
        //Quaternion newRotation = CameraRotation * Quaternion.Inverse(AnchorManager.transform.rotation);
        //Vector3 rotatedPoint = newRotation * LocalPosition;

        //transform.localPosition = rotatedPoint;
    }

    #endregion Methods
}