/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-15 15:20:51 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.SAI.EC.Frames;

namespace BJMT.RsspII4net.SAI.EC.State
{
    /// <summary>
    /// һ��״̬����ʾ�������򱻶��������ӣ�ʹ��EC������������
    /// </summary>
    class EcConnectedState : EcState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public EcConnectedState(EcState preState)
            : base(preState)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        public override void SendUserData(OutgoingPackage package)
        {
            var ecValue = this.DefenseStrategy.GetLocalEcValue();
            var seqNo = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();
            var frame = new SaiEcFrameApplication(seqNo, ecValue, package.UserData);

            var bytes = frame.GetBytes();

            this.Context.NextLayer.SendUserData(bytes);
        }

        protected override void HandleEcAskForAckFrame(SaiEcFrameAskForAck askForAckFrame)
        {
            // ��ȡ�û����ݲ�֪ͨ�۲�����
            if (askForAckFrame.UserData != null)
            {
                var timeDelay = this.DefenseStrategy.CalcTimeDelay(askForAckFrame);
                var remoteID = this.Context.RsspEP.RemoteID;

                this.Context.Observer.OnSaiUserDataArrival(remoteID, askForAckFrame.UserData, timeDelay, MessageDelayDefenseTech.EC);
            }

            // �ظ�Acknowlegment��
            var ecValue = this.DefenseStrategy.GetLocalEcValue();
            var seqNo = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();
            var frame = new SaiEcFrameAcknowlegment(seqNo, ecValue, null);

            var bytes = frame.GetBytes();

            this.Context.NextLayer.SendUserData(bytes);
        }

        protected override void HandleEcAcknowlegmentFrame(SaiEcFrameAcknowlegment ackFrame)
        {
            // ��ȡ�û����ݲ�֪ͨ�۲�����
            if (ackFrame.UserData != null)
            {
                var timeDelay = this.DefenseStrategy.CalcTimeDelay(ackFrame);
                var remoteID = this.Context.RsspEP.RemoteID;

                this.Context.Observer.OnSaiUserDataArrival(remoteID, ackFrame.UserData, timeDelay, MessageDelayDefenseTech.EC);
            }
        }

        protected override void HandleEcAppFrame(SaiEcFrameApplication frame)
        {
            var timeDelay = this.DefenseStrategy.CalcTimeDelay(frame);
            var remoteID = this.Context.RsspEP.RemoteID;

            this.Context.Observer.OnSaiUserDataArrival(remoteID, frame.UserData, timeDelay, MessageDelayDefenseTech.EC);
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
