using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using System.IO;
using System;
using System.Threading;
using System.Text;
using CymaticLabs.Unity3D.Amqp;

public class WebClient : MonoBehaviour
{
    //public string host;
    //public int port;

    //public GameData gameData;
    public GamePlay gamePlay;

/*    TcpClient tcpClient;

    Thread clientThread;

    volatile bool connected = false;*/

    [Header("AMQP")]
    public string queueName;
    
    public string exchangeName;
    //public AmqpExchangeTypes exchangeType = AmqpExchangeTypes.Topic;
    //public string subRoutingKey;
    public string pubRoutingKey;

    // Start is called before the first frame update
    void Start()
    {
        //SetupSocket();
        SetupAMQP();
    }

    // Update is called once per frame
    void Update()
    {
/*        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SendMessage();
        }*/
    }

    private void SetupAMQP()
    {
        var subscription = new AmqpQueueSubscription(queueName, false, HandleExchangeMessageReceived);
        //var subscription = new AmqpExchangeSubscription(exchangeName, exchangeType, subRoutingKey, HandleExchangeMessageReceived);
        AmqpClient.Subscribe(subscription);
    }

    private void HandleExchangeMessageReceived(AmqpQueueReceivedMessage received)
    {
        string message = System.Text.Encoding.UTF8.GetString(received.Message.Body);
        Debug.LogFormat("AMQP message received => {0}", message);
        gamePlay.HandleMessageFromServer(message);
    }

/*    private void SetupSocket()
    {
        try
        {
            connected = true;
            clientThread = new Thread(new ThreadStart(ListenForData));
            clientThread.IsBackground = true;
            clientThread.Start();
        }
        catch (Exception ex)
        {
            Debug.Log("On client connect exception " + ex);
        }
    }

    private void ListenForData()
    {
        try
        {
            Debug.Log("Trying to connect to " + Config.HOST + ":" + Config.PORT.ToString());
            tcpClient = new TcpClient(Config.HOST, Config.PORT);
            //tcpClient = new TcpClient(host, port);
            Byte[] bytes = new Byte[1024];
            while (connected)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.UTF8.GetString(incommingData);
                        Debug.Log("server message received as: " + serverMessage);
                        gamePlay.HandleMessageFromServer(serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }*/

    public void SendClientMessage(string clientMessage)
    {
        /*        if (tcpClient == null)
                {
                    return;
                }
                try
                {
                    // Get a stream object for writing. 			
                    NetworkStream stream = tcpClient.GetStream();
                    if (stream.CanWrite)
                    {
                        //string clientMessage = "This is a message from one of your clients.";
                        // Convert string message to byte array.                 
                        byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                        // Write byte array to socketConnection stream.                 
                        stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                        Debug.Log("Client sent his message - should be received by server");
                    }
                }
                catch (SocketException socketException)
                {
                    Debug.Log("Socket exception: " + socketException);
                }*/

        AmqpClient.Publish(exchangeName, pubRoutingKey, clientMessage);
    }

    private void OnApplicationQuit()
    {
/*        try
        {
            tcpClient.GetStream().Close();
            tcpClient.Close();
            connected = false;
        }
        catch
        {
            Debug.Log("Whatever");
        }*/

        AmqpClient.Disconnect();
    }
}