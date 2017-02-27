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
    /// һ��״̬����ʾ���������ڵȴ�OffsetAnswer2��Ϣ��
    /// </summary>
    class TtsWaitingforAnswer2State : TtsState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public TtsWaitingforAnswer2State(TtsState preState)
            : base(preState)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override void HandleTtsOffsetAnswer2Frame(Frames.SaiTtsFrameOffsetAnswer2 answer2)
        {
            // �յ�Answer2��ֹͣ��ʱ��
            this.Context.StopHandshakeTimer();
            LogUtility.Info(string.Format("{0}: �յ�OffsetAnswer2���ģ�ֹͣTres_start��ʱ����",
                this.Context.RsspEP.ID));

            // ����ʱ���
            this.Calculator.ResTimestamp3 = this.TTS.LocalLastRecvTimeStamp;
            this.Calculator.InitTimestamp2 = answer2.SenderLastRecvTimestamp;
            this.Calculator.InitTimestamp3 = answer2.SenderTimestamp;

            // Ӧ�𷽼���ʱ��ƫ��
            this.Calculator.EstimateResOffset();
            LogUtility.Info(string.Format("{0}: Ӧ�𷽹����ʱ��ƫ�ƣ�OffsetMin = {1}, OffsetMax = {2}",
                this.Context.RsspEP.ID, this.Calculator.ResMinOffset, this.Calculator.ResMaxOffset));


            // ����OffsetEst
            var seq = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();
            var estimateFrame = new SaiTtsFrameEstimate(seq,
                TripleTimestamp.CurrentTimestamp, TTS.RemoteLastSendTimestamp, TTS.LocalLastRecvTimeStamp, 
                this.Calculator.ResMinOffset,
                this.Calculator.ResMaxOffset);

            this.Context.NextLayer.SendUserData(estimateFrame.GetBytes()); 
            LogUtility.Info(string.Format("{0}: ����OffsetEst��", this.Context.RsspEP.ID));


            // ������ʱ�����ȴ�OffsetEnd���ġ�
            LogUtility.Info(string.Format("{0}: ������ʱ�����ȴ�OffsetEnd���ġ�",
                this.Context.RsspEP.ID));
            this.Context.StartHandshakeTimer();

            // ����״̬��
            this.Context.CurrentState = new TtsWaitingforEndState(this);
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
