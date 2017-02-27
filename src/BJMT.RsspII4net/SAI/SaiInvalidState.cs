/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-16 11:16:37 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using BJMT.RsspII4net.SAI.EC;
using BJMT.RsspII4net.SAI.EC.Frames;
using BJMT.RsspII4net.SAI.EC.State;
using BJMT.RsspII4net.SAI.TTS;
using BJMT.RsspII4net.SAI.TTS.Frames;
using BJMT.RsspII4net.SAI.TTS.State;

namespace BJMT.RsspII4net.SAI
{
    /// <summary>
    /// һ����Ч״̬�����ڱ�����ʹ�á�
    /// ���������յ�EcStart��OffsetStartʱ���л���һ����Ч��״̬��
    /// </summary>
    class SaiInvalidState : SaiState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public SaiInvalidState(ISaiStateContext context)
            : base(context)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"

        public override void HandleMaslConnected()
        {
            // �������ֳ�ʱ��ʱ�����ȴ�EcStart1��TtsOffsetStart��
            this.Context.StartHandshakeTimer();
        }

        protected override void HandleEcFrame(SaiEcFrame ecFrame)
        {
            if (ecFrame.FrameType == SaiFrameType.EC_Start)
            {
                this.Context.RsspEP.DefenseTech = MessageDelayDefenseTech.EC;

                var strategy = new EcDefenseStrategy(this.Context.RsspEP.LocalID, this.Context.RsspEP.EcInterval);
                this.Context.DefenseStrategy = strategy;

                this.Context.CurrentState = new EcWaitingforStart1State(this.Context, strategy);
                this.Context.CurrentState.HandleFrame(ecFrame);
            }
            else
            {
                throw new Exception("SaiInvalideState״̬ʱ���յ��ĵ�һ��֡����ECStart��");
            }
        }

        protected override void HandleTtsFrame(SaiTtsFrame ttsFrame)
        {
            if (ttsFrame.FrameType == SaiFrameType.TTS_OffsetStart)
            {
                this.Context.RsspEP.DefenseTech = MessageDelayDefenseTech.TTS;

                var strategy = new TtsDefenseStrategy(this.Context.FrameTransport, false);
                this.Context.DefenseStrategy = strategy;

                this.Context.CurrentState = new TtsWaitingforStartState(this.Context, strategy);
                this.Context.CurrentState.HandleFrame(ttsFrame);
            }
            else
            {
                throw new Exception("SaiInvalideState״̬ʱ���յ��ĵ�һ��֡����OffsetStart��");
            }
        }

        public override void SendUserData(OutgoingPackage package)
        {
            // Do nothing.
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion
    }
}
