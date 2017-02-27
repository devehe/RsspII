/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-22 15:43:10 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;

namespace BJMT.RsspII4net.ALE
{
    class AleClientTunnel : AleTunnel
    {
        /// <summary>
        /// ������������룩
        /// </summary>
        public const int RetryTimeout = 5000;

        #region "Filed"
        private bool _disposed = false;

        private IAleClientTunnelObserver _observer;
        #endregion

        #region "Constructor"

        /// <summary>
        /// ����һ������������������Ӷ���
        /// </summary>
        public AleClientTunnel(IPAddress clientIP, IPEndPoint serverEndPoint, IAleClientTunnelObserver observer)
            : base(clientIP, serverEndPoint, observer)
        {
            _observer = observer;
        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                base.Dispose(disposing);
            }
        }
        protected override void OnOpen()
        {
            this.BeginConnect();
        }

        protected override void OnReceiveCallbackException(Exception ex)
        {
            try
            {
                if (!_disposed)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(AleClientTunnel.RetryTimeout);
                        this.BeginConnect();
                    });
                }
            }
            catch (Exception ex1)
            {
                LogUtility.Error(ex1.ToString());
            }
        }

        protected override void OnHandshakeTimeout()
        {
            try
            {
                // �ر�Socket������ReceiveCallBack�׳��쳣��
                // ReceiveCallBack�쳣�����OnReceiveCallbackException����������������
                this.CloseTcpClient();
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        public override void Disconnect()
        {
            try
            {
                // �ر�Socket������ReceiveCallBack�׳��쳣��
                // ReceiveCallBack�쳣�����OnReceiveCallbackException����������������
                this.CloseTcpClient();
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }
        #endregion

        #region "Private methods"

        private void ConnectionCallback(IAsyncResult ar)
        {
            try
            {
                // ����첽���ӡ�
                this.Client.EndConnect(ar);

                // ��ȷ�Լ��
                var expectedEndpoint = this.LocalEndPoint;
                var actualEndPoint = (IPEndPoint)this.Client.Client.LocalEndPoint;

                if (expectedEndpoint.Address.ToString() != IPAddress.Any.ToString()
                    && expectedEndpoint.Address.ToString() != actualEndPoint.Address.ToString())
                {
                    string msg = string.Format("�󶨵�IP��ַ({0})�뷵�ص�IP��ַ({1})��һ�£�",
                        expectedEndpoint.Address, actualEndPoint.Address);

                    throw new ApplicationException(msg);
                }

                // ���ӳɹ������Port
                expectedEndpoint.Port = actualEndPoint.Port;

                // �¼�֪ͨ��
                _observer.OnTcpConnected(this);

                // ��ʼ�������ݡ�
                this.BeginReceive();
            }
            catch (Exception ex)
            {
                _observer.OnTcpConnectFailure(this, ex.Message);

                // ��������ӳɹ�����֪ͨ���ӶϿ���
                if (this.LocalEndPoint.Port != 0)
                {
                    this.HandleDisconnected(ex.Message);
                }

                // 5�����������
                Thread.Sleep(AleClientTunnel.RetryTimeout);
                this.BeginConnect();
            }
        }
        #endregion

        #region "Public methods"
        public void BeginConnect()
        {
            try
            {
                if (!_disposed)
                {
                    // �¼�֪ͨ��
                    _observer.OnTcpConnecting(this);

                    // �رվ�Socket��
                    this.CloseTcpClient();

                    // ������Socket��
                    this.LocalEndPoint.Port = 0;
                    this.Client = new TcpClient(this.LocalEndPoint);
                    this.InitializeTcpClient(true);

                    // ��ʼ���ӡ�
                    this.Client.BeginConnect(this.RemoteEndPoint.Address, this.RemoteEndPoint.Port,
                        ConnectionCallback, null);
                }
            }
            catch (Exception)
            {
                if (!_disposed)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(AleClientTunnel.RetryTimeout);
                        this.BeginConnect(); 
                    });
                }
            }
        }
        #endregion

    }
}
