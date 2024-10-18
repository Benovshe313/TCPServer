using System.Net;
using System.Net.Sockets;
using System.Text;

var ip = IPAddress.Parse("192.168.100.115");
var port = 27001;
var endPoint = new IPEndPoint(ip, port);

var listener = new TcpListener(endPoint);
listener.Start();

Console.WriteLine("Server is running");


while (true)
{
    using var client = listener.AcceptTcpClient();
    using var stream = client.GetStream();

    var fileNameBytes = new byte[1024];
    int fileNameLength = stream.Read(fileNameBytes, 0, fileNameBytes.Length);
    var fileName = Encoding.UTF8.GetString(fileNameBytes, 0, fileNameLength);

    var fileLengthBytes = new byte[1024];
    int fileLengthLength = stream.Read(fileLengthBytes, 0, fileLengthBytes.Length);
    var fileLength = int.Parse(Encoding.UTF8.GetString(fileLengthBytes, 0, fileLengthLength));

    var path = Path.Combine(Environment.CurrentDirectory, Path.GetFileName(fileName));

    using var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

    var buffer = new byte[5000];
    int bytesRead, bytesReceived = 0;

    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
    {
        fileStream.Write(buffer, 0, bytesRead);
        bytesReceived += bytesRead;

        if (bytesReceived == fileLength)
            break;
    }
    Console.WriteLine("File downloaded");
}