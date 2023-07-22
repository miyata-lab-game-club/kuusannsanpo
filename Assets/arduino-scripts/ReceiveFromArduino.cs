using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ReceiveFromArduino : MonoBehaviour
{
    public GameObject rotateObject; // The object that will be rotated
    // public GameObject cube; // �X���𔽉f����L���[�u�̎Q��
    // public RotateCube rotateCube; // RotateCube�ւ̎Q�Ƃ�ǉ�
    private Queue outputQueue;
    private SerialPort serialPort;
    private Thread thread;
    private bool isRunning = false;

    private void Start()
    {
        outputQueue = Queue.Synchronized(new Queue());

        SerialPortManager.Instance.OpenSerialPort();
        serialPort = SerialPortManager.Instance.GetSerialPort();

        isRunning = true;
        thread = new Thread(Read);
        thread.Start();
    }

    private void Read()
    {
        while (isRunning && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string message = serialPort.ReadLine();
                if (string.IsNullOrEmpty(message))
                    continue;

                string[] parts = message.Split(',');
                UnityEngine.Debug.Log(message);
                if (parts.Length == 3)
                {
                    float[] rotate = new float[3];
                    for (int i = 0; i < 3; i++)
                    {
                        rotate[i] = float.Parse(parts[i]);
                    }
                    outputQueue.Enqueue(rotate);
                }
            }
            catch (System.Exception e)
            {
                // Handle any exceptions here
            }
        }
    }
    private void Update()
    {
        if (outputQueue.Count != 0)
        {
            float[] rotate = (float[])outputQueue.Dequeue();

            // Set the rotation of the GameObject
            rotateObject.transform.eulerAngles = new Vector3(rotate[0], -rotate[2], -rotate[1]+90);
        }
    }

    private void OnDestroy()
    {
        isRunning = false;
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }
}
