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
    private int LF_power;
    private int RF_power;
    private int RB_power;

    private int LB_power;

    // WindManagerのup(bool)をa or b　のstringにして格納するようの変数
    private string windBoostedRise;
    //文字送る用の型
    private string LF_Data;
    private string RF_Data;
    private string RB_Data;
    private string LB_Data;
    private string NF_power;

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
                LF_Data = LF_power.ToString() + windBoostedRise.ToString();
                RF_Data = RF_power.ToString() + windBoostedRise.ToString();
                RB_Data = RB_power.ToString() + windBoostedRise.ToString();
                LB_Data = LB_power.ToString() + windBoostedRise.ToString();

                // NeckFan(方向と急上昇を送信)
                NF_power = windManager.currentWindIndex.ToString() + windBoostedRise.ToString();
                SetPortIndices();

                // それぞれ送信
                spManager.WriteToPort(0, LF_Data);//s1に送信
                spManager.WriteToPort(1, RF_Data);//s2に送信
                spManager.WriteToPort(2, RB_Data);//s3に送信
                spManager.WriteToPort(3, LB_Data);//s4に送信
                spManager.WriteToPort(4, NF_power);//Neckfanに送信

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

//caseの中でpullpowerを考慮してそれぞれ(lf_port~4)の力を決める
    private void SetPortIndices()
    {
        switch (windManager.currentWindIndex)
        {
            case 1:
                LF_power = 0; RF_power = 1; RB_power = 0; LB_power = 0;
                break;
            case 2:
                LF_power = 1; RF_power = 1; RB_power = 2; LB_power = 0;
                break;
            case 3:
                LF_power = 1; RF_power = 1; RB_power = 0; LB_power = 0;
                break;
            case 4:
                LF_power = 0; RF_power = 1; RB_power = 0; LB_power = 0;
                break;
            case 5:
                LF_power = 0; RF_power = 0; RB_power = 0; LB_power = 0;
                break;
            case 6:
                LF_power = 1; RF_power = 1; RB_power = 1; LB_power = 0;
                break;
            case 7:
                LF_power = 1; RF_power = 1; RB_power = 0; LB_power = 0;
                break;
            case 8:
                LF_power = 0; RF_power = 1; RB_power = 1; LB_power = 0;
                break;
        }
    }
}
