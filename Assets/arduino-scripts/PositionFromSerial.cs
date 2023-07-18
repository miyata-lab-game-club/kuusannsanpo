using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class PositionFromSerial : MonoBehaviour
{
    private SerialPort serialPort;
    private Thread thread;
    private bool isRunning = false;

    void Start()
    {
        serialPort = new SerialPort("COM3", 9600);
        serialPort.Open();
        isRunning = true;
        thread = new Thread(Read);
        thread.Start();
    }

    void Update()
    {
        if (!isRunning) return;

        try
        {
            string message = serialPort.ReadLine();

            string[] parts = message.Split(',');

            if (parts.Length == 3)
            {
                float x = float.Parse(parts[0]);
                float y = float.Parse(parts[1]);
                float z = float.Parse(parts[2]);

                transform.position = new Vector3(x, y, z);
            }
        }
        catch (System.Exception e)
        {
            //�G���[�n���h�����O�B�K�v�ɉ����Ă����ɃR�[�h�������Ă��������B
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
        serialPort.Close();
    }
}
