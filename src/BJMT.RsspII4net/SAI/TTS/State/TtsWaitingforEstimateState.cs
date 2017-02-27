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
    /// һ��״̬����ʾ���������ڵȴ�OffsetEstimate��Ϣ��
    /// </summary>
    class TtsWaitingforEstimateState : TtsState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public TtsWaitingforEstimateState(TtsState preState)
            : base(preState)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override void HandleTtsEstimateFrame(SaiTtsFrameEstimate estimateFrame)
        {
            // �յ�OffsetEst��ֹͣ��ʱ��
            LogUtility.Info(string.Format("{0}: �յ�OffsetEst���ģ�ֹͣTint_start��ʱ����", this.Context.RsspEP.ID));
            this.Context.StopHandshakeTimer();
            
            // ����ʱ���
            this.Calculator.ResMinOffset = estimateFrame.OffsetMin;
            this.Calculator.ResMaxOffset = estimateFrame.OffsetMax;

            // ������ֵ
            var valid = this.Calculator.IsEstimationValid();

            // ��������״̬��
            this.Context.Connected = valid;

            // ����OffsetEnd��
            var seq = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();
            var offsetEndFrame = new SaiTtsFrameOffsetEnd(seq,
                TripleTimestamp.CurrentTimestamp, TTS.RemoteLastSendTimestamp, TTS.LocalLastRecvTimeStamp, valid);
            this.Context.NextLayer.SendUserData(offsetEndFrame.GetBytes());

            // 
            if (valid)
            {
                LogUtility.Info(string.Format("{0}: ����OffsetEnd��ʱ��ƫ����֤ͨ����SAI�����ӳɹ���", this.Context.RsspEP.ID));
            }
            else
            {
                throw new Exception(string.Format("{0}: ʱ��ƫ�ƹ�����Ч��SAI������ʧ�ܡ�", this.Context.RsspEP.ID));
            }

            // ����״̬��
            this.Context.CurrentState = new TtsConnectedState(this);
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
