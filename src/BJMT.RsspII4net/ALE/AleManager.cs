/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-23 14:04:46 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using BJMT.RsspII4net.ALE.Config;
using BJMT.RsspII4net.ALE.Frames;
using BJMT.RsspII4net.Services;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.ALE
{
    /// <summary>
    /// ALE��������
    /// </summary>
    class AleManager : IAleListenerObserver, ITcpConnectionSlaveObserver, IDisposable
    {
        #region "Filed"
        private bool _disposed = false;

        /// <summary>
        /// ���ڹ�������TCP�����㡣
        /// </summary>
        private AleListener _aleListener;

        /// <summary>
        /// Key = ALE Connection ID.
        /// </summary>
        private ConcurrentDictionary<string, AleConnection> _aleConnections = new ConcurrentDictionary<string, AleConnection>();

        /// <summary>
        /// ��ʱ��SlaveTcpConnection�����յ���������󽫱��Ƶ�AleConnection�����С�
        /// </summary>
        private ThreadSafetyList<TcpConnection> _tcpConnections = new ThreadSafetyList<TcpConnection>();
        #endregion

        #region "Constructor"
        /// <summary>
        /// ����һ��������ʹ�õ�ALE��������
        /// </summary>
        public AleManager(AleClientConfig config)
        {
            this.LocalID = config.LocalID;

            config.LinkInfo.ToList().ForEach(p =>
            {
                var key = this.BuildAleConnectionID(config.LocalID, p.Key);
                var value = new AleConnection(config.LocalID, p.Key, p.Value);
                _aleConnections.GetOrAdd(key, value);
            });
        }

        /// <summary>
        /// ����һ��������ʹ�õ�ALE��������
        /// </summary>
        public AleManager(AleServerConfig config)
        {
            this.LocalID = config.LocalID;

            _aleListener = new AleListener(config.ListenEndPoints, this);
        }

        ~AleManager()
        {
            this.Dispose(false);
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// ��ȡ���ر�š�
        /// </summary>
        public UInt32 LocalID { get; private set; }

        /// <summary>
        /// һ���¼�����ALE�����ڽ�������ʱ������
        /// </summary>
        event EventHandler<AleConnectingEventArgs> Connecting;

        /// <summary>
        /// һ���¼�����ALE�����ӽ���ʱ������
        /// </summary>
        event EventHandler<AleConnectedEventArgs> Connected;

        /// <summary>
        /// һ���¼�����ALE�����ӶϿ�ʱ������
        /// </summary>
        event EventHandler<AleDisconnectedEventArgs> Disconnected;

        /// <summary>
        /// һ���¼������û����ݵ���ʱ������
        /// </summary>
        event EventHandler<AleUserDataReceivedEventArgs> DataReceived;

        #endregion

        #region "Virtual methods"
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    if (_aleListener != null)
                    {
                        _aleListener.Dispose();
                        _aleListener = null;
                    }
                    
                    _aleConnections.ToList().ForEach(p => p.Value.Dispose());
                    _aleConnections.Clear();

                    _tcpConnections.ToList().ForEach(p => p.Close());
                    _tcpConnections.Clear();
                }
            }
        }
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        private string BuildAleConnectionID(UInt32 clientID, UInt32 serverID)
        {
            return string.Format("ALE Connection: ClientID={0}, ServerID={1}.", clientID, serverID);
        }
        #endregion

        #region "Public methods"
        public void Open()
        {
            _aleConnections.ToList().ForEach(p => 
            {
                p.Value.Open(); 
            });

            if (_aleListener != null)
            {
                _aleListener.Start();
            }
        }

        public void Send()
        {

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region "IAleListenerObserver�ӿ�"

        void IAleListenerObserver.OnEndPointListening(TcpListener endPoint)
        {
            Console.WriteLine(string.Format("{0}. ���ڼ��� {1}", DateTime.Now, endPoint));
            // TODO: ֪ͨ�۲������ڼ���
            try
            {
            }
            catch (System.Exception /*ex*/)
            {

            }
        }

        void IAleListenerObserver.OnEndPointListenFailed(TcpListener endPoint, string message)
        {
            // TODO: ֪ͨ�۲�������ʧ�ܡ�
            try
            {
            }
            catch (System.Exception /*ex*/)
            {

            }
        }

        void IAleListenerObserver.OnAcceptTcpClient(TcpClient tcpClient)
        {
            try
            {
                // TODO: ֪ͨ�۲������յ�һ��TCP���ӡ�
                Console.WriteLine(string.Format("{0}. ���յ�һ��TCP�ͻ��� {1}<->{2}", DateTime.Now,
                    tcpClient.Client.LocalEndPoint, tcpClient.Client.RemoteEndPoint));

                var newConnection = new TcpConnectionSlave(tcpClient, this);
                newConnection.Open();

                // ������ʱ����
                _tcpConnections.Add(newConnection);
            }
            catch (System.Exception /*ex*/)
            {
            }
        }
        #endregion

        #region "ITcpConnectionObserver�ӿ�ʵ��"

        void ITcpConnectionObserver.OnConnectionClosed(TcpConnection theConnection, string reason)
        {
            try
            {
                // Client���Ӻ�û�з����κ����ݾ͹ر�ʱ�����˺�����

                _tcpConnections.Remove(theConnection);
                theConnection.Close();
            }
            catch (System.Exception /*ex*/)
            {            	
            }
        }

        void ITcpConnectionObserver.OnAleFrameArrival(TcpConnection theConnection, AleFrame theFrame)
        {
            try
            {
                // ֻ����ConnectionRequest֡��
                if (theFrame.FrameType == AleFrameType.ConnectionRequest)
                {
                    var aleData = theFrame.UserData as AleDataConnectionRequest;
                    var key = this.BuildAleConnectionID(aleData.MasterID, aleData.SlaveID);

                    AleConnection aleConnection;
                    if (!_aleConnections.ContainsKey(key))
                    {
                        aleConnection = new AleConnection(aleData.SlaveID, aleData.MasterID);
                        _aleConnections.GetOrAdd(key, aleConnection);
                    }
                    else
                    {
                        aleConnection = _aleConnections[key];
                    }

                    // ����AleConnection����
                    ((ITcpConnectionObserver)aleConnection).OnAleFrameArrival(theConnection, theFrame);

                    // ����ʱ�������Ƴ���
                    _tcpConnections.Remove(theConnection);
                }
            }
            catch (System.Exception /*ex*/)
            {
                _tcpConnections.Remove(theConnection);
                theConnection.Close();     	
            }
        }
        #endregion
    }
}
