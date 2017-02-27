/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-26 15:47:34 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BJMT.RsspII4net.ALE;
using BJMT.RsspII4net.ALE.Frames;
using BJMT.RsspII4net.Config;
using BJMT.RsspII4net.Events;
using BJMT.RsspII4net.Infrastructure.Services;
using BJMT.RsspII4net.SAI;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net
{
    /// <summary>
    /// ����RSSP-IIͨѶЭ��ķ������˽ڵ㶨�塣
    /// </summary>
    class RsspNodeServer : RsspNode,
        IRsspNode,
        INodeListenerObserver,
        IAleServerTunnelObserver
    {
        #region "Filed"
        private bool _disposed = false;

        private RsspServerConfig _rsspConfig;

        /// <summary>
        /// ���ڹ�������TCP�����㡣
        /// </summary>
        private NodeListener _nodeListener;

        /// <summary>
        /// ��ʱ��AleServerTunnel�����յ���������󽫱�ί�е�SaiConnection����
        /// </summary>
        private ThreadSafetyList<AleServerTunnel> _serverTunnels = new ThreadSafetyList<AleServerTunnel>();

        /// <summary>
        /// Key = SaiConnection ID.
        /// </summary>
        private Dictionary<string, SaiConnectionServer> _saiConnections = new Dictionary<string, SaiConnectionServer>();
        private object _saiConnectionsLock = new object();

        /// <summary>
        /// TCP�����¼�ͬ����������
        /// </summary>
        private object _acceptEventSyncLock = new object();
        #endregion

        #region "Constructor"
        /// <summary>
        /// ����һ��������ʹ�õ�ALE��������
        /// </summary>
        public RsspNodeServer(RsspServerConfig config)
            : base(config)
        {
            _rsspConfig = config;

            _nodeListener = new NodeListener(config.ListenEndPoints, this);
        }
        #endregion

        #region "Properties"

        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        public override void OnSaiDisconnected(uint localID, uint remoteID)
        {
            try
            {
                lock (_saiConnectionsLock)
                {
                    var id = this.BuildSaiConnectionID(localID, remoteID);
                    var sai = _saiConnections[id];
                    _saiConnections.Remove(id);
                    sai.Dispose();
                }

                base.OnSaiDisconnected(localID, remoteID);
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    if (_nodeListener != null)
                    {
                        _nodeListener.Dispose();
                        _nodeListener = null;
                    }

                    lock (_saiConnectionsLock)
                    {
                        _saiConnections.ToList().ForEach(p => p.Value.Dispose());
                        _saiConnections.Clear();
                    }

                    _serverTunnels.ToList().ForEach(p => p.Close());
                    _serverTunnels.Clear();
                }

                base.Dispose(disposing);
            }
        }

        protected override void OnOpen()
        {
            if (_nodeListener != null)
            {
                _nodeListener.Start();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(200);

            // TCP��ʱ���ӡ�
            sb.AppendFormat("��ʱAleTunnel����={0}��\r\n", _serverTunnels.Count);
            _serverTunnels.AsReadOnly().ToList().ForEach(p =>
            {
                sb.AppendFormat("{0}������EP={1}��Զ��EP={2}���Ƿ�����={3}���Ƿ�����={4}��\r\n",
                    p.GetType().Name, p.LocalEndPoint, p.RemoteEndPoint, p.Connected, p.IsHandShaken);
            });

            // ��ȫ���ӡ�
            sb.AppendFormat("Saiͨ������={0}��\r\n", _saiConnections.Count);
            var index = 1;
            _saiConnections.Values.ToList().ForEach(p =>
            {
                sb.AppendFormat("\r\n\r\n��{0}����{1}", index++, p);
            });

            sb.AppendFormat("\r\n");

            return sb.ToString();
        }
        #endregion

        #region "Private methods"
        protected override SaiConnection GetSaiConnection(string key)
        {
            lock (_saiConnectionsLock)
            {
                SaiConnectionServer theConnection;

                _saiConnections.TryGetValue(key, out theConnection);

                return theConnection;
            }
        }

        private void AddSaiConnection(string key, SaiConnectionServer value)
        {
            lock (_saiConnectionsLock)
            {
                _saiConnections.Add(key, value);
            }
        }

        private bool GetClientID(IPEndPoint clientEP, out uint clientID)
        {
            clientID = 0;

            var result = false;

            if (_rsspConfig != null && _rsspConfig.AcceptableClients != null)
            {
                foreach (var item in _rsspConfig.AcceptableClients)
                {
                    result = HelperTools.ContainsEndPoint(item.Value, clientEP);
                    if (result)
                    {
                        clientID = item.Key;
                    }
                }
            }

            return result;
        }

        private SaiConnectionServer CreateSaiConnectionServer(uint remoteID)
        {
            var rsspEP = new RsspEndPoint(_rsspConfig.LocalID, remoteID,
                _rsspConfig.ApplicationType, _rsspConfig.LocalEquipType,
                false, _rsspConfig.SeqNoThreshold,
                _rsspConfig.EcInterval, _rsspConfig.AuthenticationKeys,
                _rsspConfig.AcceptableClients);

            var result = new SaiConnectionServer(rsspEP, this, this);

            return result;
        }
        #endregion

        #region "Public methods"

        public List<uint> GetConnectedNodeID()
        {
            lock (_saiConnectionsLock)
            {
                return _saiConnections.Where(p => p.Value.Connected).
                    Select(p => p.Value.RemoteID).ToList();
            }
        }
        #endregion


        #region "INodeListenerObserver�ӿ�"

        void INodeListenerObserver.OnEndPointListening(TcpListener listener)
        {
            try
            {
                var args = new TcpEndPointListeningEventArgs(_rsspConfig.LocalID, listener.LocalEndpoint as IPEndPoint);
                this.NotifyTcpEndPointListeningEvent(args);
            }
            catch (System.Exception)
            {
            }
        }

        void INodeListenerObserver.OnEndPointListenFailed(TcpListener listener, string message)
        {
            try
            {
                var args = new TcpEndPointListenFailedEventArgs(_rsspConfig.LocalID, listener.LocalEndpoint as IPEndPoint);
                this.NotifyTcpEndPointListenFailedEvent(args);
            }
            catch (System.Exception)
            {
            }
        }

        void INodeListenerObserver.OnAcceptTcpClient(TcpClient tcpClient)
        {
            try
            {
                lock (_acceptEventSyncLock)
                {
                    LogUtility.Info(string.Format("Accept a new tcp client, LEP = {0}, REP={1}.",
                        tcpClient.Client.LocalEndPoint, tcpClient.Client.RemoteEndPoint));

                    SaiConnectionServer saiConnection = null;

                    // ���ҿͻ��˵�ID
                    //uint clientID;
                    //var clientIdFound = this.GetClientID(tcpClient.Client.RemoteEndPoint as IPEndPoint, out clientID);

                    //if (clientIdFound)
                    //{
                    //    // ���Ҷ�Ӧ��SaiConnection��
                    //    var saiID = this.BuildSaiConnectionID(_rsspConfig.LocalID, clientID);
                    //    saiConnection = this.GetSaiConnection(saiID) as SaiConnectionServer;

                    //    if (saiConnection == null)
                    //    {
                    //        saiConnection = this.CreateSaiConnectionServer(clientID);
                    //        this.AddSaiConnection(saiID, saiConnection);
                    //        saiConnection.Open();
                    //    }
                    //}

                    // SaiConnection�Ƿ���Ч
                    var saiConnectionValid = saiConnection != null;

                    // ����һ��Aleͨ����
                    var newTunnel = new AleServerTunnel(tcpClient, this, !saiConnectionValid);
                    if (saiConnectionValid)
                    {
                        saiConnection.AddAleServerTunnel(newTunnel);
                    }
                    else
                    {
                        // ������ʱ����
                        _serverTunnels.Add(newTunnel);

                        // ����ʱ���Ӹ��������涨ֵʱ����¼��־��
                        var count = _serverTunnels.Count;
                        if (count > 20)
                        {
                            LogUtility.Warn(string.Format("��ʱ���ӵ�TCP�����Ѵﵽ{0}����", count));
                        }
                    }

                    // �򿪡�
                    newTunnel.Open();
                }
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(string.Format("{0}", ex));
            }
        }
        #endregion




        #region "IAleTunnelObserver�ӿ�ʵ��"

        void IAleTunnelObserver.OnTcpDisconnected(AleTunnel theConnection, string reason)
        {
            try
            {
                // ����˺�����ʾ�ͻ������Ӻ�û�з����κ����ݾ͹رգ������ǿͻ��������رգ�Ҳ�������ڹ涨ʱ����û�з���CR֡�����������رգ���

                _serverTunnels.Remove(theConnection as AleServerTunnel);
                theConnection.Close();
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());  	
            }
        }

        void IAleTunnelObserver.OnAleFrameArrival(AleTunnel theConnection, AleFrame theFrame)
        {
            try
            {
                // ֻ����ConnectionRequest֡��
                if (theFrame.FrameType != AleFrameType.ConnectionRequest)
                {
                    throw new Exception(string.Format("����ʱAleTunnel���յ��˷�CR֡���رմ����ӣ������ս��={0}��Զ���ս��={1}��",
                        theConnection.LocalEndPoint, theConnection.RemoteEndPoint));
                }

                // ȷ�ϴ������ǡ�AleServerTunnel���󡱡�
                var tunnel = theConnection as AleServerTunnel;
                if (tunnel == null)
                {
                    throw new Exception(string.Format("�ڷ�AleServerTunnel���յ���CR֡�������ս��={0}��Զ���ս��={1}��",
                        theConnection.LocalEndPoint, theConnection.RemoteEndPoint));
                }

                // ����ALE֡��
                var aleCrData = theFrame.UserData as AleConnectionRequest;
                var key = this.BuildSaiConnectionID(aleCrData.ResponderID, aleCrData.InitiatorID);

                SaiConnectionServer saiConnection;
                lock (_saiConnectionsLock)
                {
                    saiConnection = this.GetSaiConnection(key) as SaiConnectionServer;
                    if (saiConnection == null)
                    {
                        saiConnection = this.CreateSaiConnectionServer(aleCrData.InitiatorID);

                        this.AddSaiConnection(key, saiConnection);

                        saiConnection.Open();
                    }
                }

                // ����SaiConnection����
                saiConnection.HandleAleConnectionRequestFrame(tunnel, theFrame);

                // ����ʱ�������Ƴ���
                _serverTunnels.Remove(tunnel);
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(string.Format("{0}", ex));
                _serverTunnels.Remove(theConnection as AleServerTunnel);
                theConnection.Close();     	
            }
        }
        #endregion
    }
}
