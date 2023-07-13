using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoController : MonoBehaviour
{
    SerialPort serialPort;

    void Start()
    {
        serialPort = new SerialPort("COM3", 9600); // あなたのPCのArduinoが接続されているCOMポートを指定
        serialPort.Open();
        StartCoroutine(ReadFromArduino());
    }

    IEnumerator ReadFromArduino()
    {
        while (true)
        {
            try
            {
                string result = serialPort.ReadLine();
                if (result == "1")
                {
                    Debug.Log("Button was pressed!");
                }
                else
                {
                    Debug.Log("Button was not pressed.");
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
            yield return null;
        }
    }

    void OnDestroy()
    {
        serialPort.Close();
    }
}
