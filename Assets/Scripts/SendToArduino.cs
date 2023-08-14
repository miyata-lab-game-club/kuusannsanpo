using System;
using System.IO.Ports;
using UnityEngine;

public class SendToArduino : MonoBehaviour
{
    private SerialPort serialPort;

    // WindManagerの参照
    public WindManager windManager;

    private void Start()
    {
        SerialPortManager.Instance.OpenSerialPort();
        serialPort = SerialPortManager.Instance.GetSerialPort();
        if (windManager == null)
        {
            Debug.LogError("WindManager reference is not set on SendToArduino.");
            return;
        }
    }

    private void Update()
    {
        try
        {
            // Arduinoにデータ送信
            if (serialPort != null && serialPort.IsOpen)
            {
                // データを":"で区切って送信
                string dataToSend = $"{windManager.currentWindIndex}:{windManager.pullPower}";
                serialPort.WriteLine(dataToSend);
                Debug.Log(dataToSend);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Could not send to Arduino: " + ex.Message);
        }
    }
}