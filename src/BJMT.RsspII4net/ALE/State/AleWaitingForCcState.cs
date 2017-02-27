/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 19:35:56 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System.Diagnostics;
using System.Linq;
using BJMT.RsspII4net.ALE.Frames;
using BJMT.RsspII4net.Events;
using System;

namespace BJMT.RsspII4net.ALE.State
{
    /// <summary>
    /// ��ʾALE���������ڵȴ�ConnectionConfirm֡��
    /// </summary>
    class AleWaitingForCcState : AleState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public AleWaitingForCcState(IAleStateContext context)
            :base(context)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"

        public override void HandleTcpConnected(AleClientTunnel theConnection)
        {
            // TCP���ӳɹ�����ALE��������
            this.SendConnectionRequestFrame(theConnection);

            // ֪ͨ�۲���TCP�����ѽ�����
            var args = new TcpConnectedEventArgs(theConnection.ID, 
                this.Context.RsspEP.LocalID, theConnection.LocalEndPoint,
                this.Context.RsspEP.RemoteID, theConnection.RemoteEndPoint);
            this.Context.TunnelEventNotifier.NotifyTcpConnected(args);

            // ������ʱ����ʱ����
            theConnection.StartHandshakeTimer();
        }
        
        public override void HandleConnectionConfirmFrame(AleTunnel theConnection, AleFrame theFrame)
        {
            // ��λ��š�
            this.Context.SeqNoManager.Initlialize();

            // ֹͣ��ʱ����ʱ����
            theConnection.StopHandshakeTimer();

            // ���CC֡��
            var ccData = theFrame.UserData as AleConnectionConfirm;
            this.CheckCcFrame(ccData);

            // ���CC֡�е�Ӧ�𷽱��У��ͨ����������һ����Ч�����ӡ�
            theConnection.IsHandShaken = true;
            this.Context.IncreaseValidConnection();

            // ������Ź������ķ��������ȷ����š�
            this.Context.SeqNoManager.GetAndUpdateSendSeq();
            this.Context.SeqNoManager.UpdateAckSeq(theFrame.SequenceNo);

            // ��CC֡�е�AU2�ύ��MASL��
            this.Context.Observer.OnAleUserDataArrival(ccData.UserData);
        }

        #endregion

        #region "Private methods"

        #endregion

        #region "Public methods"
        #endregion

    }
}
