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
    private string s4 = "COM4";
    [SerializeField]
    private string nf = "COM5"; // s5をnfに変更
    [SerializeField]
    private string kasa = "COM6"; // s6をkasaに変更

    public SerialPort[] serialPorts = new SerialPort[6];
    public int baudRate = 115200;

    private Thread[] threads = new Thread[6];
    private bool isRunning_ = false;

    private string[] messages = new string[6];
    private bool[] isNewMessageReceived_ = new bool[6];

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
        for (int i = 0; i < 6; i++)
        {
            if (isNewMessageReceived_[i] && OnDataReceived != null)
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

    private void OpenAllPorts()
    {
        string[] portNames = { s1, s2, s3, s4, nf, kasa }; // s5とs6をnfとkasaに変更
        for (int i = 0; i < 6; i++)
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
        for (int i = 0; i < 6; i++)
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

    public void Read(int index)
    {
        Debug.Log("動いて居r");
        while (isRunning_ && serialPorts[index] != null && serialPorts[index].IsOpen)
        {        Debug.Log("動いて居r2");
            try
            {   Debug.Log(messages[5]);
                        Debug.Log(messages);
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
