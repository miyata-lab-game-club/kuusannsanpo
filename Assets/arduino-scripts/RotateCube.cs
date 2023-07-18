public class ReceiveFromArduino : MonoBehaviour
{
    public RotateCube rotateCube;  // RotateCubeへの参照

    private void SomeMethod()
    {
        rotateCube.SetRotation(x, y, z);  // SetRotationメソッドを呼び出し
    }
}