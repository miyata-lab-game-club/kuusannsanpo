using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialComm : MonoBehaviour
{
    private SerialPort serialPort;
    private Thread thread;
    private Queue outputQueue; // Thread間通信用
    private bool isRunning = false;

    void Start()
    {
        serialPort = new SerialPort("COM3", 9600); // COMポートとボーレートを設定
        serialPort.ReadTimeout = 1000; // Read時のTimeout
        serialPort.Open();

        outputQueue = Queue.Synchronized(new Queue());

        isRunning = true;
        thread = new Thread(Read);
        thread.Start();
    }

    void Update()
    {
        // 何かメッセージがあれば表示
        while (outputQueue.Count > 0)
        {
            string message = (string)outputQueue.Dequeue();
            Debug.Log(message);
        }

        // スペースキーが押されたら、Arduinoにメッセージを送信
        if (Input.GetKey(KeyCode.Space))
        {
            WriteToArduino("1");
        }
        else
        {
            WriteToArduino("0");
        }
    }

    void OnDestroy()
    {
        isRunning = false;

        if (thread != null && thread.IsAlive)
        {
            thread.Join();
        }

        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort = null;
        }
    }

    private void Read()
    {
        while (isRunning && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string message = serialPort.ReadLine();
                outputQueue.Enqueue(message);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void WriteToArduino(string message)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine(message);
        }
    }
}
