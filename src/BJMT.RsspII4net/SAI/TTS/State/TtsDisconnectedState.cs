/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-16 11:25:09 
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
    /// ��ʾ����������TTS�Ͽ�״̬��
    /// </summary>
    class TtsDisconnectedState : TtsState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public TtsDisconnectedState(ISaiStateContext context, TtsDefenseStrategy strategy)
            : base(context, strategy)
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
            // ����OffsetStart
            var seqNo = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();
            var offsetStart = new SaiTtsFrameOffsetStart(seqNo, TripleTimestamp.CurrentTimestamp, 0);
            this.Context.NextLayer.SendUserData(offsetStart.GetBytes());

            // ��¼��־
            LogUtility.Info(string.Format("{0}: ����OffsetStart��", this.Context.RsspEP.ID));

            // ����ʱ���
            this.Calculator.InitTimestamp1 = offsetStart.SenderTimestamp;

            // ������ʱ��
            LogUtility.Info(string.Format("{0}: ����Tint_start��ʱ�����ȴ�OffsetAnswer1����",
                this.Context.RsspEP.ID));
            this.Context.StartHandshakeTimer();

            // ����״̬��
            this.Context.CurrentState = new TtsWaitingforAnswer1State(this);
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
