
/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{

    // receiving Thread
    Thread receiveThread;

    // udpclient object
    UdpClient client;

    // public
    public string IP = "10.250.10.14"; //default local
    public int port; // define > init

    // infos
    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = ""; // clean up this from time to time!


    //Movement
    public int speed = 30;
    public float smoothSpeed = 5f;
    public GameObject WheelLeft;
    public GameObject WheelRight;

    // start from shell
    private static void Main()
    {
        UDPReceive receiveObj = new UDPReceive();
        receiveObj.init();

        string text = "";
        do
        {
            text = Console.ReadLine();
        }
        while (!text.Equals("exit"));
    }
    // start from unity3d
    public void Start()
    {

        init();
    }

    // Movement
    void Update()
    {
        int wheelSpeed = speed * 20;
        if (lastReceivedUDPPacket == "Forward")
        {
            transform.Translate(Vector3.forward * 1 * speed * Time.deltaTime);
            WheelLeft.transform.Rotate(Vector3.right * 1 * wheelSpeed * Time.deltaTime);
            WheelRight.transform.Rotate(Vector3.right * 1 * wheelSpeed * Time.deltaTime);
        }
        else if (lastReceivedUDPPacket == "Backward")
        {
            transform.Translate(Vector3.forward * (-1) * speed * Time.deltaTime);
            WheelLeft.transform.Rotate(Vector3.right * (-1) * wheelSpeed * Time.deltaTime);
            WheelRight.transform.Rotate(Vector3.right * (-1) * wheelSpeed * Time.deltaTime);
        }


        if (lastReceivedUDPPacket == "Left" || lastReceivedUDPPacket == "Right")
        {
            if (lastReceivedUDPPacket == "Left")
            {
                transform.Rotate(Vector3.Lerp(transform.eulerAngles, new Vector3(0, -1 , 0), smoothSpeed), Space.World);
            }
            else
            {
                transform.Rotate(Vector3.Lerp(transform.eulerAngles, new Vector3(0, 1, 0), smoothSpeed), Space.World);
            }

            WheelLeft.transform.Rotate(Vector3.left * (-1) * wheelSpeed * Time.deltaTime);
            WheelRight.transform.Rotate(Vector3.right * (-1) * wheelSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.X))
        {
            lastReceivedUDPPacket = "STOP";
            allReceivedUDPPackets = " ";
        }



    }


    // OnGUI
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"
                    + "shell> nc -u 127.0.0.1 : " + port + " \n"
                    + "\nLast Packet: \n" + lastReceivedUDPPacket
                    + "\n\nAll Messages: " + allReceivedUDPPackets // + "\n\nAll Messages: " + allReceivedUDPPackets
                , style);
    }

    // init
    private void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define port
        port = 5501;

        // status
        print("Sending to 127.0.0.1 : " + port);
        print("Test-Sending to this Port: nc -u 127.0.0.1  " + port + "");


        // ----------------------------
        // Abh�ren
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread f�r den Empfang eingehender Nachrichten erstellen.
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(port);
        while (true)
        {

            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);

                // Den abgerufenen Text anzeigen.
                print(">> " + text);

                // latest UDPpacket
                lastReceivedUDPPacket = text;

                // ....
                allReceivedUDPPackets = allReceivedUDPPackets + text;

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
}