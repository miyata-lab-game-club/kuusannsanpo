using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

/* ���̒����U������v���C���[�̃R���g���[���[
* ���̃x�N�g���ɎP���X����ƕ��̕����Ƀv���C���[���i��
* �X���Ȃ��ƐÂ��ɏ����������Ă���
* ���݂̍��x��\������
*/

public class PlayerController : MonoBehaviour
{
    // �E�̃R���g���[���[��Transform
    [SerializeField] private Transform rightControllerTransform;

    // �����Ă������x
    [SerializeField] private Vector3 gravityDirection;

    // ������`�F�b�N�|�C���g
    private int currentCheckPointIndex = 0;

    [SerializeField] private WindManager windController;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        /*
        // �R���g���[���[�̊p�x���擾
        Quaternion rightControllerRotation = rightControllerTransform.rotation;

        Vector3 rightControllerTilt = (rightControllerRotation * Vector3.forward).normalized;
        //Debug.Log("��]" + rightControllerRotation.eulerAngles);
        Debug.Log("�X��" + rightControllerTilt);
        Debug.DrawLine(new Vector3(0, 15, 2), new Vector3(0, 15, 2) + rightControllerTilt * 3, Color.red);
        bool existNextCheckPoint = windController.currentWindDirection(currentCheckPointIndex, this.transform);
        float similarity;
        if (existNextCheckPoint)
        {
            similarity = Vector3.Dot(rightControllerTilt, windController.windDirection);
        }
        else
        {
            similarity = -1;
        }
        // �ގ��x��0.7��肨�������Ƃ�
        Debug.Log(similarity);
        if (similarity >= 0.7)
        {
            //�`�F�b�N�|�C���g�Ɍ������ĕ�������
            Vector3 currentWind = windController.windDirection;
            playerRigidbody.velocity = currentWind;
        }
        // �ގ����Ă��Ȃ����
        else
        {
            // �����Ă���
            playerRigidbody.velocity = gravityDirection;
        }
        //Debug.Log(rightControllerTransform.position);
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CheckPoint")
        {
            currentCheckPointIndex = int.Parse(other.gameObject.name.Substring(10));
            Debug.Log(currentCheckPointIndex);
        }
    }
}