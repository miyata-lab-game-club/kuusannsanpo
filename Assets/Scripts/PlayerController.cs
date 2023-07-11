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
    // ��UI
    [SerializeField] private TextMeshProUGUI heightText;

    // �E�̃R���g���[���[��Transform
    //[SerializeField] private Transform rightControllerTransform;

    // Player��Rigidbody
    private Rigidbody playerRigidbody;

    // �����Ă������x
    [SerializeField] private Vector3 gravityDirection;

    // ������`�F�b�N�|�C���g
    private int currentCheckPointIndex = 0;

    [SerializeField] private WindController windController;
    [SerializeField] private float pullTime;
    [SerializeField] private float pullPower;
    [SerializeField] private Vector3 pullDirection;
    private bool isPulling = false;

    // Start is called before the first frame update
    private void Start()
    {
        heightText.text = this.transform.position.y.ToString();
        playerRigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        heightText.text = this.transform.position.y.ToString();
        // �L�[����������O�����ɂӂ��ƕ���
        if (Input.GetKeyDown(KeyCode.Space) && isPulling == false)
        {
            StartCoroutine(PullUmbrella());
        }
        if (isPulling == false)
        {
            playerRigidbody.velocity = gravityDirection;
        }
    }

    private IEnumerator PullUmbrella()
    {
        playerRigidbody.velocity = pullDirection * pullPower;
        isPulling = true;
        yield return new WaitForSeconds(pullTime);
        isPulling = false;
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