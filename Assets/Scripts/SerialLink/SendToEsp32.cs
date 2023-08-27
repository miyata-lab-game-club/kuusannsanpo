using System;
using System.IO.Ports;
using UnityEngine;

public class SendToEsp32 : MonoBehaviour
{
    private SerialPortManager spManager; // SerialPortManager の参照を持つ変数
    // WindManagerのup(bool)をintにして格納するようの変数
    private int windBoostedRise;
    // WindManagerの参照
    public WindManager windManager;

    // Port４とネックファンは後で追加
    private int PortIndex_1;
    private int PortIndex_2;
    private int PortIndex_3;

    //文字送る用の型
    private string port1_DataToSend;
    private string port2_DataToSend;
    private string port3_DataToSend;


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
    }

    private void FixedUpdate()
    {
        try
        {
            windBoostedRise = windManager.up ? 1 : 0; // windManager.upが真なら1、偽なら0を格納

            //
            SetPortIndices();
            spManager.WriteToPort(0, port1_DataToSend);
            spManager.WriteToPort(1, port2_DataToSend);
            spManager.WriteToPort(2, port3_DataToSend);
            // spManager.WriteToPort(3, "111");
            // spManager.WriteToPort(4, "111");
            // spManager.WriteToPort(5, "111");

            //送るように1つの文字型にする。
             Debug.Log(port1_DataToSend);
             Debug.Log(port2_DataToSend);
             Debug.Log(port3_DataToSend);

            port1_DataToSend = PortIndex_1.ToString() + windManager.pullPower.ToString() + windBoostedRise.ToString();
            port2_DataToSend = PortIndex_2.ToString() + windManager.pullPower.ToString() + windBoostedRise.ToString();
            port3_DataToSend = PortIndex_3.ToString() + windManager.pullPower.ToString() + windBoostedRise.ToString();

        }
        catch (Exception ex)
        {
            Debug.LogError("Could not send to ESP32: " + ex.Message);
        }
    }

    private void SetPortIndices()
    {
        switch (windManager.currentWindIndex)
        {
            case 1:
                PortIndex_1 = 0; PortIndex_2 = 1; PortIndex_3 = 0;
                break;
            case 2:
                PortIndex_1 = 1; PortIndex_2 = 1; PortIndex_3 = 2;
                break;
            case 3:
                PortIndex_1 = 1; PortIndex_2 = 1; PortIndex_3 = 0;
                break;
            case 4:
                PortIndex_1 = 0; PortIndex_2 = 1; PortIndex_3 = 0;
                break;
            case 5:
                PortIndex_1 = 0; PortIndex_2 = 0; PortIndex_3 = 0;
                break;
            case 6:
                PortIndex_1 = 1; PortIndex_2 = 1; PortIndex_3 = 1;
                break;
            case 7:
                PortIndex_1 = 1; PortIndex_2 = 1; PortIndex_3 = 0;
                break;
            case 8:
                PortIndex_1 = 0; PortIndex_2 = 1; PortIndex_3 = 1;
                break;
        }
    }
}
