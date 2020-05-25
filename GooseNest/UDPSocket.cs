using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooseNest
{
    public class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int _bufferSize = 8 * 1024;
        private State _state = new State();
        private EndPoint _fromEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback _receiveCallback = null;

        public class State
        {
            public byte[] buffer = new byte[_bufferSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
            }, _state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(_state.buffer, 0, _bufferSize, SocketFlags.None, ref _fromEndPoint, _receiveCallback = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref _fromEndPoint);
                _socket.BeginReceiveFrom(so.buffer, 0, _bufferSize, SocketFlags.None, ref _fromEndPoint, _receiveCallback, so);
            }, _state);
        }
    }
}
