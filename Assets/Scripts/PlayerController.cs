using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

/* 風の中を散歩するプレイヤーのコントローラー
* 風のベクトルに傘を傾けると風の方向にプレイヤーが進む
* 傾けないと静かに少しずつ落ちていく
* 現在の高度を表示する
*/

public class PlayerController : MonoBehaviour
{
    // 仮UI
    [SerializeField] private TextMeshProUGUI heightText;

    // 右のコントローラーのTransform
    //[SerializeField] private Transform rightControllerTransform;

    // PlayerのRigidbody
    private Rigidbody playerRigidbody;

    // 落ちていく速度
    [SerializeField] private Vector3 gravityDirection;

    // 今いるチェックポイント
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
        // キーを押したら前方向にふわりと浮く
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