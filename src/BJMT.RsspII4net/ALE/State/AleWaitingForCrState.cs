/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 19:36:10 
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

namespace BJMT.RsspII4net.ALE.State
{
    /// <summary>
    /// ��ʾALE���������ڵȴ�ConnectionRequest֡��
    /// </summary>
    class AleWaitingForCrState : AleState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public AleWaitingForCrState(IAleStateContext context)
            :base(context)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"

        public override void HandleConnectionRequestFrame(AleTunnel theConnection, AleFrame theFrame)
        {
            var crData = theFrame.UserData as AleConnectionRequest;

            // ֹͣ���ּ�ʱ����
            theConnection.StopHandshakeTimer();

            // ��λ��š�
            this.Context.SeqNoManager.Initlialize();

            // ���CRЭ��֡��
            this.CheckCrFrame(crData, theConnection);

            // ���·������͡�
            this.Context.RsspEP.ServiceType = crData.ServiceType;

            // ��CR֡�е�AU1�ύ��MASL����Ҫ����RandomB����                    
            this.Context.Observer.OnAleUserDataArrival(crData.UserData);

            // ����CC֡��
            this.SendConnectionConfirmFrame(theConnection);

            // ������Ź������ķ��������ȷ����š�
            this.Context.SeqNoManager.GetAndUpdateSendSeq();
            this.Context.SeqNoManager.UpdateAckSeq(theFrame.SequenceNo);

            if (!this.Context.ContainsTunnel(theConnection))
            {
                // ���յ�һ���µ�TCP���ӡ�
                var args = new TcpConnectedEventArgs(theConnection.ID,
                    this.Context.RsspEP.LocalID, theConnection.LocalEndPoint,
                    this.Context.RsspEP.RemoteID, theConnection.RemoteEndPoint);
                this.Context.TunnelEventNotifier.NotifyTcpConnected(args);

                // ������Ч�����Ӹ�����
                theConnection.IsHandShaken = true;
                this.Context.IncreaseValidConnection();

                // ����TCP����
                this.Context.AddConnection(theConnection);
            }
        }
        #endregion

        #region "Private methods"

        #endregion

        #region "Public methods"
        #endregion

    }
}
