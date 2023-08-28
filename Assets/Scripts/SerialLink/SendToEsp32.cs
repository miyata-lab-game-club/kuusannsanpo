using System;
using System.IO.Ports;
using UnityEngine;
using System.Collections;

public class SendToEsp32 : MonoBehaviour
{
    private SerialPortManager spManager; // SerialPortManager の参照を持つ変数

    // WindManagerの参照
    public WindManager windManager;

    // Port４とネックファンは後で追加
    private int PortIndex_1;
    private int PortIndex_2;
    private int PortIndex_3;

    private int PortIndex_4;

    // WindManagerのup(bool)をa or b　のstringにして格納するようの変数
    private string windBoostedRise;
    //文字送る用の型
    private string port1_DataToSend;
    private string port2_DataToSend;
    private string port3_DataToSend;
    private string port4_DataToSend;
    private string port5_DataToSend;

    private void Start()
    {
        spManager = SerialPortManager.Instance;

        if (spManager == null)
        {
            Debug.LogError("SerialPortManager instance not found!");
            return;
        }

        if (windManager == null)
        {
            Debug.LogError("WindManager reference is not set on SendToEsp32.");
            return;
        }
        {
            StartCoroutine(SendDataCoroutine());
        }
    }
    private IEnumerator SendDataCoroutine()
    {
        while (true)  // 無限ループで送信処理を繰り返す
        {
            try
            {
                // windManager.upが真なら1、偽なら0を格納
                windBoostedRise = windManager.up ? "a" : "b";

                // 力覚装置1~4s用に文字型に変換(引っ張る力と急上昇を送信)
                port1_DataToSend = PortIndex_1.ToString() + windBoostedRise.ToString();
                port2_DataToSend = PortIndex_2.ToString() + windBoostedRise.ToString();
                port3_DataToSend = PortIndex_3.ToString() + windBoostedRise.ToString();
                port4_DataToSend = PortIndex_4.ToString() + windBoostedRise.ToString();

                // NeckFan(方向と急上昇を送信)
                port5_DataToSend = windManager.currentWindIndex.ToString() + windBoostedRise.ToString();
                SetPortIndices();

                // それぞれ送信
                spManager.WriteToPort(0, port1_DataToSend);//s1に送信
                spManager.WriteToPort(1, port2_DataToSend);//s2に送信
                spManager.WriteToPort(2, port3_DataToSend);//s3に送信
                spManager.WriteToPort(3, port4_DataToSend);//s4に送信
                spManager.WriteToPort(4, port5_DataToSend);//Neckfanに送信

            // spManager.Read(5)の結果をデバッグログで表示
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not send to ESP32: " + ex.Message);
            }

            yield return new WaitForSeconds(0.5f);  // 0.5秒待機
        }
    }

    private void FixedUpdate()
    {
       
    }

//caseの中でpullpowerを考慮してそれぞれ(s1~4)の力を決める
    private void SetPortIndices()
    {
        switch (windManager.currentWindIndex)
        {
            case 1:
                PortIndex_1 = 0; PortIndex_2 = 1; PortIndex_3 = 0; PortIndex_4 = 0;
                break;
            case 2:
                PortIndex_1 = 1; PortIndex_2 = 1; PortIndex_3 = 2; PortIndex_4 = 0;
                break;
            case 3:
                PortIndex_1 = 1; PortIndex_2 = 1; PortIndex_3 = 0; PortIndex_4 = 0;
                break;
            case 4:
                PortIndex_1 = 0; PortIndex_2 = 1; PortIndex_3 = 0; PortIndex_4 = 0;
                break;
            case 5:
                PortIndex_1 = 0; PortIndex_2 = 0; PortIndex_3 = 0; PortIndex_4 = 0;
                break;
            case 6:
                PortIndex_1 = 1; PortIndex_2 = 1; PortIndex_3 = 1; PortIndex_4 = 0;
                break;
            case 7:
                PortIndex_1 = 1; PortIndex_2 = 1; PortIndex_3 = 0; PortIndex_4 = 0;
                break;
            case 8:
                PortIndex_1 = 0; PortIndex_2 = 1; PortIndex_3 = 1; PortIndex_4 = 0;
                break;
        }
    }
}
