using System;
using System.IO.Ports;
using UnityEngine;

public class SendToArduino : MonoBehaviour
{
    private SerialPort serialPort;

    private void Start()
    {
        SerialPortManager.Instance.OpenSerialPort();
        serialPort = SerialPortManager.Instance.GetSerialPort();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            try
            {
                serialPort.WriteLine("1"); // Sends "1" to the Arduino when the Space key is pressed
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not send to Arduino: " + ex.Message);
            }
        }
    }
}
