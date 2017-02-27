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
    /// һ��״̬����ʾ���������ڵȴ�OffsetEnd��Ϣ��
    /// </summary>
    class TtsWaitingforEndState : TtsState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public TtsWaitingforEndState(TtsState preState)
            : base(preState)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override void HandleTtsOffsetEndFrame(SaiTtsFrameOffsetEnd offsetEndFrame)
        {
            // �յ�OffsetEnd��ֹͣTres_start��ʱ��
            LogUtility.Info(string.Format("{0}: �յ�OffsetEnd���ģ�ֹͣTres_start��ʱ����", this.Context.RsspEP.ID));
            this.Context.StopHandshakeTimer();

            if (offsetEndFrame.Valid)
            {
                LogUtility.Info(string.Format("{0}: ʱ��ƫ�ƹ�������Ч��SAI�����ӳɹ���", this.Context.RsspEP.ID));
            }
            else
            {
                LogUtility.Info(string.Format("{0}: ʱ��ƫ�ƹ�������Ч���޷�����SAI���ӡ�", this.Context.RsspEP.ID));
            }

            // 
            this.Context.Connected = offsetEndFrame.Valid;

            if (!offsetEndFrame.Valid)
            {
                throw new Exception(string.Format("{0}: ʱ��ƫ�ƹ�������Ч���޷�����SAI���ӡ�", this.Context.RsspEP.ID));
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
