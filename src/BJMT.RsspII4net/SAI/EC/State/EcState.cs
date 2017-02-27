/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-15 15:30:19 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Diagnostics;
using System.Linq;
using BJMT.RsspII4net.SAI.EC.Frames;

namespace BJMT.RsspII4net.SAI.EC.State
{
    abstract class EcState : SaiState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        protected EcState(EcState preState)
            : base(preState.Context)
        {
            this.DefenseStrategy = preState.DefenseStrategy;
        }

        protected EcState(ISaiStateContext context, EcDefenseStrategy strategy)
            :base(context)
        {
            this.DefenseStrategy = strategy;
        }
        #endregion

        #region "Properties"
        public EcDefenseStrategy DefenseStrategy { get; set; }
        #endregion

        #region "Virtual methods"

        protected virtual void HandleEcStartFrame(SaiEcFrameStart ecStartFrame)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }

        protected virtual void HandleEcAskForAckFrame(SaiEcFrameAskForAck frame)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }

        protected virtual void HandleEcAcknowlegmentFrame(SaiEcFrameAcknowlegment frame)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }

        protected virtual void HandleEcAppFrame(SaiEcFrameApplication ecAppFrame)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }
        #endregion

        #region "Override methods"
        public override void HandleMaslConnected()
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }

        public override void SendUserData(OutgoingPackage package)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }

        protected override void HandleEcFrame(SaiEcFrame ecFrame)
        {
            if (ecFrame.FrameType == SaiFrameType.EC_Start)
            {
                this.HandleEcStartFrame(ecFrame as SaiEcFrameStart);
            }
            else if (ecFrame.FrameType == SaiFrameType.EC_AppDataAskForAck)
            {
                this.HandleEcAskForAckFrame(ecFrame as SaiEcFrameAskForAck);
            }
            else if (ecFrame.FrameType == SaiFrameType.EC_AppDataAcknowlegment)
            {
                this.HandleEcAcknowlegmentFrame(ecFrame as SaiEcFrameAcknowlegment);
            }
            else if (ecFrame.FrameType == SaiFrameType.EC_AppData)
            {
                this.HandleEcAppFrame(ecFrame as SaiEcFrameApplication);
            }
            else
            {
                LogUtility.Error(string.Format("{0}: {1} ���յ���Ec֡��",
                    this.Context.RsspEP.ID, this.GetType().Name));
            }
        }
        #endregion

        #region "Private/Protected methods"

        protected void SendEcStartFrame()
        {
            // ����EC Start Frame.
            var initValue = this.DefenseStrategy.GetLocalEcValue();
            var ecInterval = this.Context.RsspEP.EcInterval;
            var seqNo = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();
            var frame = new SaiEcFrameStart(seqNo, initValue, 1, ecInterval);
            
            var bytes = frame.GetBytes();

            this.Context.NextLayer.SendUserData(bytes);
        }
        #endregion

        #region "Public methods"
        #endregion

    }
}
