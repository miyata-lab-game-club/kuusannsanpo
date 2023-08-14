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

    private string ConvertVector3ToString(Vector3 vec, float pullPower)
    {
        return string.Format("{0},{1},{2},{3}", vec.x, vec.y, vec.z, pullPower);
    }

    private void Update()
    {
        try
        {
            Vector3 windData = windManager.windXZDirection[windManager.currentWindIndex];
            string dataToSend = ConvertVector3ToString(windData, windManager.pullPower);

            // Arduinoにデータ送信
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.WriteLine(dataToSend);
            }

            // For debugging purposes:
            // Debug.Log(windManager.pullPower);
            Debug.Log(dataToSend);
        }
        catch (Exception ex)
        {
            Debug.LogError("Could not send to Arduino: " + ex.Message);
        }
    }
}