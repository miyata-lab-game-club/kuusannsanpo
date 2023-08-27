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
        SerialPortManager spManager = SerialPortManager.Instance;

        if (spManager == null)
        {
            Debug.LogError("SerialPortManager instance not found!");
            return;
        }

        // 例として、最初のポートを選択
        serialPort = spManager.serialPorts[0];

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
            if (serialPort != null && serialPort.IsOpen)
            {
                int upValue = windManager.up ? 1 : 0;
                string dataToSend = $"{windManager.currentWindIndex}:{windManager.pullPower}:{upValue}";
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
