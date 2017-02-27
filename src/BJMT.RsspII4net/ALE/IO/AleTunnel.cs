/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-22 15:42:55 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Net;
using System.Net.Sockets;
using BJMT.RsspII4net.ALE.Frames;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.ALE
{
    abstract class AleTunnel : IHandshakeTimeoutObserver, IDisposable
    {
        /// <summary>
        /// ��ֵ�����˶೤ʱ���������ݰ�����Alive��Ϣ������λ�����룩
        /// </summary>
        private const int AliveInterval = 1200;

        /// <summary>
        /// ��ֵ�����˵�����KeepAlive��೤ʱ���ղ�����Ӧ������һ��KeepAlive������λ�����룩
        /// </summary>
        private const int AliveTimeout = 1000;

        /// <summary>
        /// ���ֳ�ʱ��
        /// </summary>
        private const int HandshakeTimeout = 3000;

        #region "Filed"

        private bool _disposed = false;
        
        /// <summary>
        /// ��������ʱ����ʱ���档
        /// ����ȡֵ�ο�MTU��
        /// </summary>
        private byte[] _recvBufCache = new byte[1500];

        /// <summary>
        /// ALE����������
        /// </summary>
        private AleStreamParser _streamPaser = new AleStreamParser();
        
        /// <summary>
        /// ���ֳ�ʱ��������
        /// </summary>
        private HandshakeTimeoutManager _handshakeTimeoutMgr;
        #endregion

        #region "Constructor"
        /// <summary>
        /// ˽�й��캯����
        /// </summary>
        private AleTunnel(IAleTunnelObserver observer)
        {
            this.Observer = observer;

            this.IsNormal = true;
            this.IsActive = true;

            _handshakeTimeoutMgr = new HandshakeTimeoutManager(AleTunnel.HandshakeTimeout, this);
        }

        /// <summary>
        /// ����һ���ͻ���ʹ�õ�TCP���ӡ�
        /// </summary>
        protected AleTunnel(IPAddress clientIP,
            IPEndPoint serverEndPoint,
            IAleTunnelObserver observer)
            :this(observer)
        {
            if (clientIP==null || serverEndPoint == null || observer == null)
            {
                throw new ArgumentNullException();
            }

            this.LocalEndPoint = new IPEndPoint(clientIP, 0);
            this.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(serverEndPoint.Address.ToString()), serverEndPoint.Port);
            
            this.ID = string.Format("Client_{0}_{1}_{2}", this.LocalEndPoint, this.RemoteEndPoint, Guid.NewGuid());
        }

        /// <summary>
        /// ����һ����������ʹ�õ�TCP���ӡ�
        /// </summary>
        protected AleTunnel(TcpClient client,
            IAleTunnelObserver observer)
            : this(observer)
        {
            if (client == null || observer == null)
            {
                throw new ArgumentNullException();
            }

            this.Client = client;
            this.InitializeTcpClient(true);

            this.LocalEndPoint = this.Client.Client.LocalEndPoint as IPEndPoint;
            this.RemoteEndPoint = this.Client.Client.RemoteEndPoint as IPEndPoint;

            this.ID = string.Format("Server_{0}_{1}_{2}", this.LocalEndPoint, this.RemoteEndPoint, Guid.NewGuid());
        }

        /// <summary>
        /// �սắ��
        /// </summary>
        ~AleTunnel()
        {
            this.Dispose(false);
        }
        #endregion

        #region "Properties"

        /// <summary>
        /// ��ȡ�����ӵ�Ψһ��ʶ��
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// ��ȡһ��ֵ�����ڱ�ʾTcp�����Ƿ��ѽ�����
        /// </summary>
        public bool Connected
        {
            get
            {
                if (this.Client != null)
                {
                    return this.Client.Connected;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// ��ȡһ��ֵ�����ڱ�ʾ��TCP�����Ƿ���ͨ��CR/CC���֡�
        /// true��ʾ�����֣�fasle��ʾû�����֡�
        /// </summary>
        public bool IsHandShaken { get; set; }

        /// <summary>
        /// ��ȡһ��ֵ�����ڱ�ʾ��ǰ��·�Ƿ�Ϊ������·��true��ʾ������·��false��ʾ������·����A�����ʱ��Ч��
        /// TODO: IsNormalӦ�ø������û�A��D�������ȷ����
        /// </summary>
        public bool IsNormal { get; private set; }

        /// <summary>
        /// ��ȡһ��ֵ �����ڱ�ʾ��ǰ��·�Ƿ��ڻ״̬����A�����ʱ��Ч��
        /// true��ʾ���false��ʾ�ǻ��
        /// </summary>
        public bool IsActive { get; private set; }

        public IAleTunnelObserver Observer { get; set; }

        protected TcpClient Client { get; set; }

        public IPEndPoint LocalEndPoint { get; private set; }

        public IPEndPoint RemoteEndPoint { get; private set; }
        #endregion

        #region "abstract/virtual methods"

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    _handshakeTimeoutMgr.Dispose();

                    this.CloseTcpClient();
                }
            }
        }

        protected abstract void OnOpen();
        protected abstract void OnReceiveCallbackException(Exception ex);
        protected abstract void OnHandshakeTimeout();

        public abstract void Disconnect();
        #endregion

        #region "Override methods"
        #endregion

        #region "Private/Protected methods"
        protected void InitializeTcpClient(bool keepAliveEnabled)
        {
            this.Client.SendBufferSize = AleStreamParser.AleStreamMaxLength * 5;
            this.Client.ReceiveBufferSize = AleStreamParser.AleStreamMaxLength * 5;

            if (keepAliveEnabled)
            {
                HelperTools.SetKeepAlive(this.Client.Client, AleTunnel.AliveInterval, AleTunnel.AliveTimeout);
            }
        }

        protected void CloseTcpClient()
        {
            try
            {
                if (this.Client != null)
                {
                    this.Client.Close();
                    this.Client = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }
        
        protected void HandleDisconnected(string reason)
        {
            this.Observer.OnTcpDisconnected(this, reason);
        }
        
        protected void BeginReceive()
        {
            this.Client.GetStream().BeginRead(_recvBufCache, 0, _recvBufCache.Length,
                ReceiveCallback, _recvBufCache);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Read data from the remote device.
                int count = this.Client.GetStream().EndRead(ar);
                var bytes = ar.AsyncState as byte[];

                if (count > 0)
                {
                    this.HandleDataReceived(bytes, count);

                    // Get the rest of the data.
                    this.BeginReceive();
                }
                else
                {
                    throw new Exception(string.Format("Զ�����������ر�TCP���ӣ������ս����{0}��Զ���ս����{1}��", 
                        this.LocalEndPoint, this.RemoteEndPoint));
                }
            }
            catch (System.Exception ex)
            {
                this.HandleDisconnected(ex.Message);

                // ����ģ�巽�������ദ��
                this.OnReceiveCallbackException(ex);
            }
        }

        private void HandleDataReceived(byte[] buffer, int bytesRead)
        {
            try
            {
                var aleFrameBytes = _streamPaser.ParseTcpStream(buffer, bytesRead);

                aleFrameBytes.ForEach(p =>
                {
                    var aleFrame = AleFrame.Parse(p);
                    this.Observer.OnAleFrameArrival(this, aleFrame);
                });
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
                // ��ʽ1���쳣ȫ�����ԣ������첽���ա�
                // ��ʽ2���Ͽ����ӡ�
                //this.Disconnect();
            }
        }

        #endregion

        #region "Public methods"

        public void Open()
        {
            this.OnOpen();
        }

        public void Close()
        {
            this.Dispose();
        }

        public void Send(byte[] data)
        {
            if (this.Connected && !_disposed)
            {
                this.Client.GetStream().Write(data, 0, data.Length);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void StartHandshakeTimer()
        {
            _handshakeTimeoutMgr.Start();
        }

        public void StopHandshakeTimer()
        {
            _handshakeTimeoutMgr.Stop();
        }

        /// <summary>
        /// �������ֳ�ʱ��
        /// </summary>
        public void HandleHandshakeTimeout()
        {
            this.OnHandshakeTimeout();
        }
        #endregion

        #region "IHandshakeTimeoutObserver�ӿ�ʵ��"
        void IHandshakeTimeoutObserver.OnHandshakeTimeout()
        {
            try
            {
                this.HandleHandshakeTimeout();
            }
            catch (System.Exception /*ex*/)
            {
            }
        }
        #endregion
    }
}
