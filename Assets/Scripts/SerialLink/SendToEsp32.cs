using System;
using System.IO.Ports;
using UnityEngine;
using System.Collections;


public class SendToEsp32 : MonoBehaviour
{
    //private SerialPortManager spManager; // SerialPortManager の参照を持つ変数

    // WindManagerの参照
    //public WindManager windManager;

    // Port４とネックファンは後で追加
    private int LF_power;
    private int RF_power;
    private int RB_power;
    private int LB_power;

    // 全開
    const int FULL＿OPEN = 3;
    // 中くらい
    const int HALF＿OPEN = 2;
    // 閉じる
    const int CLOSE = 1;
    // WindManagerのup(bool)をa or b　のstringにして格納するようの変数
    private string windBoostedRise;
    //文字送る用の型
    private string LF_Data;
    private string RF_Data;
    private string RB_Data;
    private string LB_Data;
    private string NF_Data;


    private void Start()
    {
        // spManager = SerialPortManager.Instance;

        // if (spManager == null)
        // {
        //     Debug.LogError("SerialPortManager instance not found!");
        //     return;
        // }

        // if (windManager == null)
        // {
        //     Debug.LogError("WindManager reference is not set on SendToEsp32.");
        //     return;
        // }
        
        // {
        //     StartCoroutine(SendDataCoroutine());
        // }
    }

    public void StartSendData(SerialPortManager spManager,WindManager windManager){
        StartCoroutine(SendDataCoroutine(spManager,windManager));
    }
    private IEnumerator SendDataCoroutine(SerialPortManager spManager,WindManager windManager)
    {
        while (true)  // 無限ループで送信処理を繰り返す
        {
            try
            {
                // windManager.upが真なら1、偽なら0を格納
                windBoostedRise = windManager.up ? "a" : "b";

                // 力覚装置1~4s用に文字型に変換(引っ張る力と急上昇を送信)
                LF_Data = LF_power.ToString() + windBoostedRise.ToString();
                RF_Data = RF_power.ToString() + windBoostedRise.ToString();
                RB_Data = RB_power.ToString() + windBoostedRise.ToString();
                LB_Data = LB_power.ToString() + windBoostedRise.ToString();

                // NeckFan(方向と急上昇を送信)
                NF_Data = windManager.currentWindIndex.ToString() + windBoostedRise.ToString();
                SetPortIndices(windManager);

                // それぞれ送信
                spManager.WriteToPort(0, LF_Data);//LFに送信
                spManager.WriteToPort(1, RF_Data);//RFに送信
                spManager.WriteToPort(2, RB_Data);//RBに送信
                spManager.WriteToPort(3, LB_Data);//LBに送信
                spManager.WriteToPort(4, NF_Data);//Neckfanに送信


            // spManager.Read(5)の結果をデバッグログで表示
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not send to ESP32: " + ex.Message);
            }
            //Debug.Log("RB_Data: " + RB_Data);
            yield return new WaitForSeconds(0.1f);  // 0.5秒待機
        }
    }

    private void FixedUpdate()
    {
       
    }

//caseの中でpullpowerを考慮してそれぞれ(LF_port~4)の力を決める
    private void SetPortIndices(WindManager windManager)
    {
        switch (windManager.currentWindIndex)
        {
            /*
            // 1が下のモータ閉める、2が半分、3が下のモータ開いて上がく。
            */
            case 1://北(前)
                LF_power = FULL＿OPEN; RF_power = FULL＿OPEN; RB_power = CLOSE; LB_power = CLOSE;
                break;
            case 2://北東(右前)
                LF_power = CLOSE; RF_power = FULL＿OPEN; RB_power = CLOSE; LB_power = CLOSE;
                break;
            case 3://東(右)
                LF_power = FULL＿OPEN; RF_power = CLOSE; RB_power = CLOSE; LB_power = CLOSE;
                break;
            case 4://南東(右後)
                LF_power = CLOSE; RF_power = CLOSE; RB_power = FULL＿OPEN; LB_power = CLOSE;
                break;
            case 5://南(後)
                LF_power = CLOSE; RF_power = CLOSE; RB_power = FULL＿OPEN; LB_power = FULL＿OPEN;
                break;
            case 6://南西(左後)
                LF_power = CLOSE; RF_power = CLOSE; RB_power = CLOSE; LB_power = FULL＿OPEN;
                break;
            case 7://西(左)
                LF_power = FULL＿OPEN; RF_power = CLOSE; RB_power = CLOSE; LB_power = FULL＿OPEN;
                break;
            case 8://北西(左前)
                LF_power = FULL＿OPEN; RF_power = CLOSE; RB_power = CLOSE; LB_power = CLOSE;
                break;
        }
    }
}
