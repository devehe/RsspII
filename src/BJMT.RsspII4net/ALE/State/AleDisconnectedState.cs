/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 19:34:11 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.ALE.Frames;
using BJMT.RsspII4net.Events;
using System.Diagnostics;

namespace BJMT.RsspII4net.ALE.State
{
    /// <summary>
    /// ��ʾALE�������ĶϿ�״̬��
    /// </summary>
    class AleDisconnectedState : AleState
    {
        public AleDisconnectedState(IAleStateContext context)
            :base(context)
        {

        }

        public override void HandleTcpConnected(AleClientTunnel theConnection)
        {
            // ��λ��š�
            this.Context.SeqNoManager.Initlialize();

            // TCP���ӳɹ�����ALE��������
            this.SendConnectionRequestFrame(theConnection);

            // ֪ͨ�۲���TCP�����ѽ�����
            var args = new TcpConnectedEventArgs(theConnection.ID, 
                this.Context.RsspEP.LocalID, theConnection.LocalEndPoint,
                this.Context.RsspEP.RemoteID, theConnection.RemoteEndPoint);
            this.Context.TunnelEventNotifier.NotifyTcpConnected(args);

            // ������һ��״̬��
            this.Context.CurrentState = new AleWaitingForCcState(this.Context);

            // �������ֳ�ʱ��ʱ����
            theConnection.StartHandshakeTimer();
        }


        #region "private methods"
        #endregion
    }
}
