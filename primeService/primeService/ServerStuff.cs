using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace primeService
{
    public enum Response { UNKNOWN_ERROR, CONNECTION_ERROR, CLIENT_ERROR, SERVER_ERROR, PRIME, NOT_PRIME }
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener
    {
        public const int DEFAULT_PORT = 55555;
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {


        }

        public static void StartListening(bool usePortArg, int portArg)
        {
            int port = DEFAULT_PORT;
            if (usePortArg)
            {
                port = portArg;
            }
            else
            {
                string text = System.IO.File.ReadAllText(@"server.cfg");
                port = Int32.Parse(text);
            }

            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost"/*Dns.GetHostName()*/);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine(@"Waiting for a connection on port {0}", port);
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            Response response = Response.UNKNOWN_ERROR; ;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                String data = state.sb.ToString();

                if (data.Contains("\n"))
                {
                    //if(data.Contains("end"))
                    //{
                    //    try
                    //    {
                    //        state.sb.Clear();
                    //        handler.Shutdown(SocketShutdown.Both);
                    //        handler.Close();

                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Console.WriteLine(e.ToString());
                    //    }
                    //    return;
                    //}

                    Console.WriteLine();
                    Console.WriteLine("received data: " + data.Trim());
                    DateTime start = DateTime.Now;
                    int number = 0;
                    try
                    {
                        number = Int32.Parse(data);
                    }
                    catch (Exception e)
                    {
                        state.sb.Clear();
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        throw new Exception();
                    }

                    bool isPrime = true;
                    if (number < 0)
                    {
                        state.sb.Clear();
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        throw new Exception();
                    }
                    else if (number == 0)
                    {
                        response = Response.PRIME;
                    }
                    else if (number == 1)
                    {
                        response = Response.CLIENT_ERROR;
                    }
                    else if (number > 100)
                    {
                        state.sb.Clear();
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        throw new Exception();
                    }
                    else
                    {

                        for (int i = 2; i < number / 2 && isPrime; i++)
                        {
                            if (number % i == 0)
                                isPrime = false;
                        }
                        Random r = new Random();

                        if (number > 50)
                        {
                            Thread.Sleep(6000 + r.Next(3000));
                            
                        }
                        else if (number > 25)
                        {
                            Thread.Sleep(2000 + r.Next(1000));
                        }

                        if (isPrime)
                            response = Response.PRIME;
                        else
                            response = Response.NOT_PRIME;
                    }

                    Console.WriteLine(@" result: " + response.ToString());
                    Send(handler, response.ToString());
                    state.sb.Clear();

                    //handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    //    new AsyncCallback(ReadCallback), state);

                    TimeSpan timeItTook = DateTime.Now - start;
                    Console.WriteLine(" request processed in " + (timeItTook.TotalMilliseconds < 1D ? 1D : timeItTook.TotalMilliseconds) + " ms");

                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
                else
                {

                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }

            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);


                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}