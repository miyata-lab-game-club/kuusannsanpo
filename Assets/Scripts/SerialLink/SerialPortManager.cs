using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System; 

public class SerialPortManager : MonoBehaviour
{
    public static SerialPortManager Instance { get; private set; }

    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    [Header("Port Names")]
    [SerializeField]
    private string portName_s1 = "COM1";
    [SerializeField]
    private string portName_s2 = "COM2";
    [SerializeField]
    private string portName_s3 = "COM3";
    [SerializeField]
    private string portName_s4 = "COM4";

    public SerialPort[] serialPorts = new SerialPort[4];
    public int baudRate = 115200;

    private Thread[] threads = new Thread[4];
    private bool isRunning_ = false;

    private string[] messages = new string[4];
    private bool[] isNewMessageReceived_ = new bool[4];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        OpenAllPorts();
    }

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (isNewMessageReceived_[i])
            {
                OnDataReceived(messages[i]);
                isNewMessageReceived_[i] = false;
            }
        }
    }

    private void OnApplicationQuit()
    {
        CloseAllPorts();
    }

    // private void OpenAllPorts()
    // {
    //     string[] portNames = { portName_s1, portName_s2, portName_s3, portName_s4 };
    //     for (int i = 0; i < 4; i++)
    //     {
    //         serialPorts[i] = new SerialPort(portNames[i], baudRate, Parity.None, 8, StopBits.One);
    //         serialPorts[i].Open();
    //         threads[i] = new Thread(() => Read(i));
    //         threads[i].Start();
    //     }
    //     isRunning_ = true;
    // }
private void OpenAllPorts()
{
    string[] portNames = { portName_s1, portName_s2, portName_s3, portName_s4 };
    for (int i = 0; i < 4; i++)
    {
        try
        {
            serialPorts[i] = new SerialPort(portNames[i], baudRate, Parity.None, 8, StopBits.One);
            serialPorts[i].Open();

            int threadIndex = i; // ラムダキャプチャの問題を回避するためのローカル変数
            threads[i] = new Thread(() => Read(threadIndex));
            threads[i].Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error opening port or starting thread for index {i}: {e.Message}");
        }
    }
    isRunning_ = true;
}
    private void CloseAllPorts()
    {
        isRunning_ = false;
        for (int i = 0; i < 4; i++)
        {
            if (threads[i] != null && threads[i].IsAlive)
            {
                threads[i].Join();
            }
            if (serialPorts[i] != null && serialPorts[i].IsOpen)
            {
                serialPorts[i].Close();
                serialPorts[i].Dispose();
            }
        }
    }

    private void Read(int index)
    {
        while (isRunning_ && serialPorts[index] != null && serialPorts[index].IsOpen)
        {
            try
            {
                messages[index] = serialPorts[index].ReadLine();
                isNewMessageReceived_[index] = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void WriteToPort(int index, string message)
    {
        try
        {
            serialPorts[index].Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
