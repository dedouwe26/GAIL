using System.Net;
using System.Net.Sockets;

TcpListener tcpListener = new(IPAddress.Parse("127.0.0.1"), 3003);
tcpListener.Start();
using TcpClient client = await tcpListener.AcceptTcpClientAsync();
while (true) {
    byte[] buffer = new byte[256];
    if (client.GetStream().Read(buffer)!=0) {
        Console.WriteLine(BitConverter.ToString(buffer));
    }
}