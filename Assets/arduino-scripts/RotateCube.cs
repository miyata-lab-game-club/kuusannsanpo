public class ReceiveFromArduino : MonoBehaviour
{
    public RotateCube rotateCube;  // RotateCube�ւ̎Q��

    private void SomeMethod()
    {
        rotateCube.SetRotation(x, y, z);  // SetRotation���\�b�h���Ăяo��
    }
}