using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace primeTester
{
    public enum Response { UNKNOWN_ERROR, CONNECTION_ERROR, CLIENT_ERROR, SERVER_ERROR, PRIME, NOT_PRIME }

    class ServerConnection
    {        
        public static Response Send(string data)
        {
            Response response = Response.UNKNOWN_ERROR;

            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            string receivedData = "";

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, TestCases.SERVER_PORT);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    // Encode the data string into a byte array.  
                    Console.WriteLine("sending " + data);
                    byte[] msg = Encoding.ASCII.GetBytes(data + "\r\n");

                    response = Response.UNKNOWN_ERROR;
                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
                    receivedData = Encoding.ASCII.GetString(bytes, 0, bytesRec);              

                    if (receivedData.Contains("UNKNOWN_ERROR"))
                    {
                        response = Response.UNKNOWN_ERROR;
                    }
                    else if(receivedData.Contains("CONNECTION_ERROR"))
                    {
                        response = Response.CONNECTION_ERROR;
                    }
                    else if (receivedData.Contains("CLIENT_ERROR"))
                    {
                        response = Response.CLIENT_ERROR;
                    }
                    else if (receivedData.Contains("SERVER_ERROR"))
                    {
                        response = Response.SERVER_ERROR;
                    }
                    else if (receivedData.Contains("NOT_PRIME"))
                    {
                        response = Response.NOT_PRIME;
                    }
                    else if (receivedData.Contains("PRIME"))
                    {
                        response = Response.PRIME;
                    }

                    Console.WriteLine("response: " + response.ToString());

                    //msg = Encoding.ASCII.GetBytes("end\r\n");
                    //sender.Send(msg);

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException)
                {
                    response = Response.CONNECTION_ERROR;
                }
                catch (SocketException)
                {
                    response = Response.CONNECTION_ERROR;
                }
                catch (Exception)
                {
                    response = Response.UNKNOWN_ERROR;
                }

                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return response;
        }
    }
}
