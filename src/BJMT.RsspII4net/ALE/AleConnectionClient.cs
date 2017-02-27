/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-16 8:18:55 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.ALE.State;
using BJMT.RsspII4net.Config;
using BJMT.RsspII4net.Events;
using BJMT.RsspII4net.Infrastructure.Services;

namespace BJMT.RsspII4net.ALE
{
    class AleConnectionClient : AleConnection, IAleClientTunnelObserver
    {
        #region "Filed"
        #endregion

        #region "Constructor"

        /// <summary>
        /// ����һ����������������ALE Connection��
        /// </summary>
        public AleConnectionClient(RsspEndPoint rsspEP, IEnumerable<RsspTcpLinkConfig> linkConfig,
            IAuMessageBuilder auMsgProvider,
            IAleConnectionObserver observer,
            IAleTunnelEventNotifier tcpEventNotifier)
            : base(rsspEP, auMsgProvider, observer, tcpEventNotifier)
        {
            linkConfig.ToList().ForEach(p =>
            {
                var item = new AleClientTunnel(p.ClientIpAddress, p.ServerEndPoint, this);
                this.AddConnection(item);
            });
        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"

        protected override AleState GetInitialState()
        {
            return new AleDisconnectedState(this);
        }

        protected override void HandleTunnelDisconnected(AleTunnel theConnection, string reason)
        {
            try
            {
                // �ͻ��ˣ�������Ч�����Ӹ�����
                if (theConnection.IsHandShaken)
                {
                    this.DescreaseValidConnection();
                    theConnection.IsHandShaken = false;
                }
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

        #region "IAleTunnelClient �ӿ�ʵ��"
        void IAleClientTunnelObserver.OnTcpConnecting(AleClientTunnel theConnection)
        {
            try
            {
                var args = new TcpConnectingEventArgs(theConnection.ID,
                    this.RsspEP.LocalID, theConnection.LocalEndPoint,
                    this.RsspEP.RemoteID, theConnection.RemoteEndPoint);

                this.TunnelEventNotifier.NotifyTcpConnecting(args);
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        void IAleClientTunnelObserver.OnTcpConnected(AleClientTunnel theConnection)
        {
            try
            {
                LogUtility.Info(string.Format("{0}: A TCP link Connected. LEP = {1}, REP = {2}",
                    this.RsspEP.ID, theConnection.LocalEndPoint, theConnection.RemoteEndPoint));

                lock (this.StateEventLock)
                {
                    this.CurrentState.HandleTcpConnected(theConnection);
                }
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        void IAleClientTunnelObserver.OnTcpConnectFailure(AleClientTunnel theConnection, string reason)
        {
            try
            {
                var args = new TcpConnectFailedEventArgs(theConnection.ID,
                    this.RsspEP.LocalID, theConnection.LocalEndPoint,
                    this.RsspEP.RemoteID, theConnection.RemoteEndPoint);

                this.TunnelEventNotifier.NotifyTcpConnectFailure(args);
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        #endregion
    }
}
