using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR;

public class WindManager : MonoBehaviour
{
    [SerializeField] SendToEsp32 sendToesp32;
    private SerialPortManager spManager; 
    [SerializeField] WindMovement windMovement;
    [SerializeField] private GameObject player;
    [SerializeField] private float speed;

    // PlayerのRigidbody
    private Rigidbody playerRigidbody;

    // 傘を引っ張るパワー
    public int pullPower = 0;

    public bool up;
    private bool boost;
    private bool twiceBoost;// 速度二倍

    // 落ちていく速度
    [SerializeField] private Vector3 gravityDirection;

    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private Transform rightControllerTransform;
    [SerializeField] private GameObject[] directionUIs;
    [SerializeField] private float upHeight = 20;
    [SerializeField] private float startUpHeight = 5;
    [SerializeField] private float windCicleTime = 5;
    private float timer;
    private Vector3 currentWind;


    [SerializeField] private ReceiveFromEsp32 ReceiveFromEsp32;// SerialPortManager の参照を持つ変数
    public int currentWindIndex = 0;
    // ひとつ前の風の方向
    private int previousWindIndex = 1;


    //　上、北、北東、東、南東、南、南西、西、北西
    public Vector3[] windDirection = Define.windDirection;

    public Vector3[] windXZDirection = new Vector3[]
{new Vector3(0,0,0), new Vector3(0, 0, 1),new Vector3(1, 0, 1),new Vector3(1, 0, 0),
     new Vector3(1,0,-1), new Vector3(0, 0, -1),new Vector3(-1, 0, -1),new Vector3(-1, 0, 0),
     new Vector3(-1, 0, 1)
};

    // 仮UI
    [SerializeField] private TextMeshProUGUI heightText;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI similarityText;

    public GameObject kasa_Port;
    Quaternion kasaVector;

    private void Start()
    {

        /*
        Vector3 oculusForward = InputTracking.GetLocalRotation(XRNode.CenterEye) * Vector3.forward;
        player.transform.rotation = Quaternion.LookRotation(oculusForward, Vector3.up);*/
        player.transform.forward = centerEyeAnchor.forward;
        heightText.text = this.transform.position.y.ToString();
        playerRigidbody = player.GetComponent<Rigidbody>();
        timer = 0;
        timerText.text = timer.ToString();
        currentWind = currentWindDirection();

        spManager = SerialPortManager.Instance;

        if (spManager == null)
        {
            Debug.LogError("SerialPortManager instance not found!");
            return;
        }
        
        {
            sendToesp32.StartSendData(spManager,this);
        }

    }

    private Vector3 rightControllerTilt;

    private void Update()
    {
        kasaVector = kasa_Port.transform.rotation;

        // 引っ張る強さによってスピードが変わる
        if (pullPower == 3)
        {
            speed = 3;
        }
        else if (pullPower == 2)
        {
            speed = 2;
        }
        else
        {
            speed = 1;
        }
        //player.transform.forward = centerEyeAnchor.forward;
        // 右コントローラーの傾き
        // Quaternion rightControllerRotation = rightControllerTransform.rotation;
        Quaternion rightControllerRotation = kasaVector;

        // 右コントローラーの傾きをベクトルにする
        //rightControllerTilt = (rightControllerRotation * Vector3.forward).normalized;
        rightControllerTilt = kasa_Port.transform.up;
        //Debug.Log(rightControllerTilt);
        // xz平面に戻す
        rightControllerTilt.y = 0;

        heightText.text = this.transform.position.y.ToString();

        if (OVRInput.GetDown(OVRInput.RawButton.A) && !boost)
        {
            boost = true;
            timer = 0;
            twiceBoost = currentWindFromController();
            SetActiveWindDirection(currentWindIndex);
        }

        //Debug.Log(ReceiveFromEsp32.buttonState);
        //スイッチ押しているなら加速
        if (ReceiveFromEsp32.buttonState == 'b' && !boost)
        {
            Debug.Log("加速");
            boost = true;
            timer = 0;
            twiceBoost = currentWindFromController();
            SetActiveWindDirection(currentWindIndex);
        }

        Debug.DrawLine(new Vector3(-28, 9, -60), new Vector3(-28, 9, -60) + rightControllerTilt * 5, Color.red);

        timerText.text = timer.ToString();
    }

    bool upOnce = false;
    private void FixedUpdate()
    {
        if (boost != true)
        {
            if (player.transform.position.y > startUpHeight && !up)
            {
                timer += Time.deltaTime;
                if (timer > windCicleTime)
                {
                    currentWind = currentWindDirection();
                    timer = 0;
                }

                float similarity;
                similarity = Vector3.Dot(rightControllerTilt.normalized, windXZDirection[currentWindIndex].normalized);
                // 類似度が0.7よりおおきいとき
                //Debug.Log(similarity);
                similarityText.text = similarity.ToString();
                if (similarity >= 0.8)
                {
                    playerRigidbody.velocity = currentWind * speed;
                }
                // 類似していなければ
                else
                {
                    // おちていく
                    playerRigidbody.velocity = gravityDirection;
                }
            }
            // 5mから20mまで上昇
            if (player.transform.position.y < startUpHeight)
            {
                up = true;
            }
            // 20mまで上昇したらとまる
            if (player.transform.position.y > upHeight && up)
            {
                up = false;
                upOnce = false;
            }
            // 上昇中
            if (up)
            {
                playerRigidbody.velocity = windDirection[0]*5;
                SetActiveWindDirection(0);
                if(upOnce == false){
                    currentWindIndex = 0;
                   // Debug.Log("力を加える");
                    //playerRigidbody.AddForce(windDirection[0], ForceMode.Impulse);
                    //playerRigidbody.velocity = windDirection[0];
                            // 風向きを音とエフェクトで提示
                    windMovement.WindMove(currentWindIndex);
                    upOnce = true;
                }
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer < windCicleTime && twiceBoost)
            {
                currentWind = windDirection[currentWindIndex];
                // 傾きに一番近い方向に風が吹く
                playerRigidbody.velocity = currentWind * 3 * speed;
            }
            else if (timer < windCicleTime && !twiceBoost)
            {
                currentWind = windDirection[currentWindIndex];
                // 傾きに一番近い方向に風が吹く
                playerRigidbody.velocity = currentWind * speed;
            }
            else
            {
                timer = 0;
                boost = false;
            }
        }
    }

    // コントローラーの向いている方向に風を吹かせる 
    private bool currentWindFromController()
    {
        float similarity;
        float maxSimilarity = -1;
        int tmpCurrentWindIndex = currentWindIndex;
        // 傾きと一番近い方向を割り出す
        for (int i = 1; i < windDirection.Length; i++)
        {
            rightControllerTilt.y = 0;
            similarity = Vector3.Dot(rightControllerTilt.normalized, windXZDirection[i].normalized);
            /*Debug.Log("右コントローラーの傾き" + rightControllerTilt.normalized + "×" + windXZDirection[i].normalized);
            Debug.Log("類似度" + similarity);*/
            if (similarity > maxSimilarity)
            {
                maxSimilarity = similarity;
                currentWindIndex = i;
            }
        }
    
        // 風向きを音とエフェクトで提示
        windMovement.WindMove(currentWindIndex);
        similarityText.text = maxSimilarity.ToString();
        if (tmpCurrentWindIndex == currentWindIndex)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 現在吹いている風を決める
    public Vector3 currentWindDirection()
    {
        // currentWindIndex = Random.Range(1, 9);

        //各斜めの風の方向
        int[] choices = { 2, 4, 6, 8 };
        int rand = choices[Random.Range(0,choices.Length)];
        // 前吹いた風の向きと同じなら
        while(rand == previousWindIndex){
            // もう一度振りなおす
            rand = choices[Random.Range(0,choices.Length)];
        }
        Debug.Log(rand);
        // 現在の風を更新
        currentWindIndex = rand;
        // ひとつ前の風を更新
        previousWindIndex = currentWindIndex;
        // UIの表示
        SetActiveWindDirection(currentWindIndex);
        // 風向きを音とエフェクトで提示
        windMovement.WindMove(currentWindIndex);
        return windDirection[currentWindIndex].normalized;
    }

    private void SetActiveWindDirection(int currentWindIndex)
    {
        for (int i = 0; i < windDirection.Length; i++)
        {
            directionUIs[i].SetActive(false);
        }
        directionUIs[currentWindIndex].SetActive(true);
    }
}