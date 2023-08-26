using System;
using System.IO.Ports;
using UnityEngine;

public class SendToEsp32 : MonoBehaviour
{
    private SerialPort serialPort;

    // WindManagerの参照
    public WindManager windManager;

   private void Start()
{
    // 以下の行を追加して、シーン内の SerialPortManager コンポーネントを取得します
    SerialPortManager spManager = FindObjectOfType<SerialPortManager>();

    if (spManager == null)
    {
        Debug.LogError("SerialPortManager instance not found!");
        return;
    }

    // 以下の行で SerialPortManager の serialPort_ を直接取得します
    serialPort = spManager.serialPort_;

    if (serialPort == null || !serialPort.IsOpen)
    {
        Debug.LogError("Serial port is not available or open!");
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
            // ESP32にデータ送信
            if (serialPort != null && serialPort.IsOpen)
            {
                int upValue = windManager.up ? 1 : 0;

                // データを":"で区切って送信
                string dataToSend = $"{windManager.currentWindIndex}:{windManager.pullPower}:{upValue}:{"a"}";
                serialPort.WriteLine(dataToSend);
                Debug.Log(dataToSend);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Could not send to ESP32: " + ex.Message);
        }
    }
}
