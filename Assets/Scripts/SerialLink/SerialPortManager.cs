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
    private string s1 = "COM1";
    [SerializeField]
    private string s2 = "COM2";
    [SerializeField]
    private string s3 = "COM3";
        [SerializeField]

    //ESP側の不調により以下は一旦省略
    // private string s4 = "COM4";

    //     [SerializeField]
    // private string neckfun = "COM5";

    //     [SerializeField]
    // private string umbrella = "COM6";

    public SerialPort[] serialPorts = new SerialPort[3];
    public int baudRate = 115200;

    private Thread[] threads = new Thread[3];
    private bool isRunning_ = false;

    private string[] messages = new string[3];
    private bool[] isNewMessageReceived_ = new bool[3];

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
        // for (int i = 0; i < 3; i++)
        // {
        //     if (isNewMessageReceived_[i])
        //     {
        //         OnDataReceived(messages[i]);
        //         isNewMessageReceived_[i] = false;
        //     }
        // }
    }

    private void OnApplicationQuit()
    {
        CloseAllPorts();
    }

    private void OpenAllPorts()
    {
        string[] portNames = { s1, s2,s3 };
        for (int i = 0; i < 3; i++)
        {
            try
            {
                serialPorts[i] = new SerialPort(portNames[i], baudRate, Parity.None, 8, StopBits.One);
                serialPorts[i].Open();

                int threadIndex = i; // Avoid lambda capture problem
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
        for (int i = 0; i < 3; i++)
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
