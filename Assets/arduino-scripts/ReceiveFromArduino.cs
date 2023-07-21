using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ReceiveFromArduino : MonoBehaviour
{
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
                string[] parts = message.Split(',');
                UnityEngine.Debug.Log(message);
                if (parts.Length == 4)
                {
                    outputQueue.Enqueue(parts);
                }
            }
            catch (System.Exception e)
            {
                //�G���[�n���h�����O�B�K�v�ɉ����Ă����ɃR�[�h�������Ă��������B
            }
        }
    }
    private void Update()
    {
        while (outputQueue.Count != 0)
        {
            string[] parts = (string[])outputQueue.Dequeue();

            int mode = int.Parse(parts[3]);

            if (mode == 0)
            {
                UnityEngine.Debug.Log("3");
                // Gyro mode
                                               // �����K�؂ȏ���

            }
            else if (mode == 1)
            {
                UnityEngine.Debug.Log("2");// Accelerometer mode
                                               // �����K�؂ȏ���
            }
            else if (mode == 2)
            {
                UnityEngine.Debug.Log("1");
                // AHRS mode
                float x = float.Parse(parts[0]);
                float y = float.Parse(parts[1]);
                float z = float.Parse(parts[2]);

                // �X����RotateCube�ɔ��f���܂��B
                // rotateCube.SetRotation(x, y, z);
            }
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
