using System;
using System.IO.Ports;
using UnityEngine;

public class SerialPortManager : MonoBehaviour
{
    public static SerialPortManager Instance { get; private set; }
    private SerialPort serialPort;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenSerialPort(string portName = "COM3", int baudRate = 9600)
    {
        try
        {
            if (serialPort == null)
            {
                serialPort = new SerialPort(portName, baudRate);
                serialPort.ReadTimeout = 1000;
            }
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Could not open serial port: " + ex.Message);
        }
    }

    public SerialPort GetSerialPort()
    {
        return serialPort;
    }

    void OnDestroy()
    {
        CloseSerialPort();
    }

    public void CloseSerialPort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort = null;
        }
    }

}
