
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
using System.Threading.Tasks;

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

    // Hindernis
    public float timer = 2;
    public int zaeler = 0;
    public GameObject hindernisPrefab;

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

    bool TimerFinished()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 2;

            return true;
        }
        else
        {
            return false;
        }
    }

    // Movement
    void Update()
    {
        Vector3 spawnPosition;
        GameObject hindernis;
        int wheelSpeed = speed * 20;

        if (lastReceivedUDPPacket == "Hindernis")
        {
            /*switch (zaeler)
            {
                case 0:
                    spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 20);
                    Instantiate(hindernisPrefab, spawnPosition, hindernisPrefab.transform.rotation);
                    break;

                case 1:
                    spawnPosition = new Vector3(transform.position.x - 20, transform.position.y, transform.position.z);
                    Instantiate(hindernisPrefab, spawnPosition, hindernisPrefab.transform.rotation);
                    break;

                case 2:
                    spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 20);
                    Instantiate(hindernisPrefab, spawnPosition, hindernisPrefab.transform.rotation);
                    break;

                case 3:
                    spawnPosition = new Vector3(transform.position.x + 20, transform.position.y, transform.position.z);
                    Instantiate(hindernisPrefab, spawnPosition, hindernisPrefab.transform.rotation);
                    zaeler = 0;
                    break;

                default:
                    break;
            }*/
            //zaeler++;

            spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            hindernis = Instantiate(hindernisPrefab, spawnPosition, transform.rotation);
            hindernis.transform.Translate(new Vector3(0,0,20));


        }
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
                transform.Rotate(Vector3.Lerp(transform.eulerAngles, new Vector3(0, -3, 0), smoothSpeed), Space.World);
            }
            else
            {
                transform.Rotate(Vector3.Lerp(transform.eulerAngles, new Vector3(0, 3, 0), smoothSpeed), Space.World);
            }

            WheelLeft.transform.Rotate(Vector3.left * (-1) * wheelSpeed * Time.deltaTime);
            WheelRight.transform.Rotate(Vector3.right * (-1) * wheelSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.X))
        {
            lastReceivedUDPPacket = "STOP";
            allReceivedUDPPackets = " ";
        }


        //lastReceivedUDPPacket = "";
    }


    // OnGUI
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"
                    + "shell> nc -u 127.0.0.1 : " + port + " \n"
                    + "Zähler : " + zaeler + " \n"
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
        port = 5505;

        // status
        print("Sending to 127.0.0.1 : " + port);
        print("Test-Sending to this Port: nc -u 127.0.0.1  " + port + "");


        // ----------------------------
        // Abhören
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread für den Empfang eingehender Nachrichten erstellen.
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