/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-22 15:43:23 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Net.Sockets;

namespace BJMT.RsspII4net.ALE
{
    class AleServerTunnel : AleTunnel
    {
        #region "Filed"
        private IAleServerTunnelObserver _observer;
        private bool _waitingforConnectionRequest = false;
        #endregion

        #region "Constructor"
        /// <summary>
        /// ����һ��������������Ӷ���
        /// </summary>
        public AleServerTunnel(TcpClient client, IAleServerTunnelObserver observer, bool waitforCR)
            : base(client, observer)
        {
            _observer = observer;
            _waitingforConnectionRequest = waitforCR;
        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override void OnOpen()
        {
            this.BeginReceive();

            if (_waitingforConnectionRequest)
            {
                this.StartHandshakeTimer();
            }
        }

        protected override void OnReceiveCallbackException(Exception ex)
        {
            try
            {
                this.Close();
            }
            catch (System.Exception ex1)
            {
                LogUtility.Error(ex1.ToString());
            }
        }

        protected override void OnHandshakeTimeout()
        {
            try
            {
                LogUtility.Error(string.Format("�涨ʱ����û���յ�CR֡���ر�Socket��LEP = {0}, REP={1}.",
                    this.LocalEndPoint, this.RemoteEndPoint));

                this.Close();
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
                this.Close();
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

    }
}
