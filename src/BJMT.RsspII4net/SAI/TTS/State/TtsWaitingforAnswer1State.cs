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
    /// һ��״̬����ʾ���������ڵȴ�OffsetAnswer1��Ϣ��
    /// </summary>
    class TtsWaitingforAnswer1State : TtsState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public TtsWaitingforAnswer1State(TtsState preState)
            : base(preState)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override void  HandleTtsOffsetAnswer1Frame(SaiTtsFrameOffsetAnswer1 answer1)
        {
            // �յ�Answer1��ֹͣ��ʱ��
            LogUtility.Info(string.Format("{0}: �յ���OffsetAnswer1��ֹͣTint_start��ʱ����", this.Context.RsspEP.ID));
            this.Context.StopHandshakeTimer();

            // ����ʱ���
            this.Calculator.InitTimestamp2 = this.TTS.LocalLastRecvTimeStamp;
            this.Calculator.ResTimestamp1 = answer1.SenderLastRecvTimestamp;
            this.Calculator.ResTimestamp2 = answer1.SenderTimestamp;

            // ���𷽼���ƫ��
            this.Calculator.EstimateInitOffset();

            // ��¼��־
            LogUtility.Info(string.Format("{0}: ���𷽹���ʱ��ƫ�ƣ�OffsetMin = {1}, OffsetMax = {2}",
                this.Context.RsspEP.ID, this.Calculator.InitiatorMinOffset, this.Calculator.InitiatorMaxOffset));

            // ����OffsetAnswer2����
            var seq = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();
            var answer2 = new SaiTtsFrameOffsetAnswer2(seq,
                TripleTimestamp.CurrentTimestamp, 
                this.TTS.RemoteLastSendTimestamp, 
                this.TTS.LocalLastRecvTimeStamp);

            this.Context.NextLayer.SendUserData(answer2.GetBytes());
            
            LogUtility.Info(string.Format("{0}: ����OffsetAnswer2��", this.Context.RsspEP.ID));


            // ������ʱ��,�ȴ�OffsetEst����
            LogUtility.Info(string.Format("{0}: ����Tint_start��ʱ�����ȴ�OffsetEst����",  this.Context.RsspEP.ID));
            this.Context.StartHandshakeTimer(); 

            // ����״̬��
            this.Context.CurrentState = new TtsWaitingforEstimateState(this);
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
