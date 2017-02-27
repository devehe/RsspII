/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-15 15:21:08 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.SAI.TTS.Frames;

namespace BJMT.RsspII4net.SAI.TTS.State
{
    /// <summary>
    /// һ��״̬����ʾ���������ڵȴ�OffsetStart��Ϣ��
    /// </summary>
    class TtsWaitingforStartState : TtsState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public TtsWaitingforStartState(ISaiStateContext context, TtsDefenseStrategy strategy)
            : base(context, strategy)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override void HandleTtsOffsetStartFrame(SaiTtsFrameOffsetStart startFrame)
        {
            // �յ�OffsetStart��ֹͣ���ֳ�ʱ��ʱ����
            LogUtility.Info(string.Format("{0}: �յ���OffsetStart����. ֹͣTres_start��ʱ����",
                this.Context.RsspEP.ID));
            this.Context.StopHandshakeTimer();

            // ����OffsetAnswer1
            var seqNo = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();

            var offsetAnswer1 = new SaiTtsFrameOffsetAnswer1(seqNo,
                TripleTimestamp.CurrentTimestamp, 
                this.TTS.RemoteLastSendTimestamp,
                this.TTS.LocalLastRecvTimeStamp, 0);

            this.Context.NextLayer.SendUserData(offsetAnswer1.GetBytes());            
            LogUtility.Info(string.Format("{0}: ����OffsetAnswer1.", this.Context.RsspEP.ID));

            // ����ʱ���
            this.Calculator.ResTimestamp2 = offsetAnswer1.SenderTimestamp;

            // ����Tres_start��ʱ�����ȴ�OffsetAnswer2���ġ�
            LogUtility.Info(string.Format("{0}: ����Tres_start��ʱ�����ȴ�OffsetAnswer2���ġ�", this.Context.RsspEP.ID));
            this.Context.StartHandshakeTimer();

            // ����״̬��
            this.Context.CurrentState = new TtsWaitingforAnswer2State(this);
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
