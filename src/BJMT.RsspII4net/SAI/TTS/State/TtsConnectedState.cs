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
    /// һ��״̬����ʾ�������򱻶��������ӣ�ʹ��TTS������������
    /// </summary>
    class TtsConnectedState : TtsState
    {
        #region "Filed"
        public TtsConnectedState(TtsState preState)
            : base(preState)
        {

        }
        #endregion

        #region "Constructor"
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override void HandleTtsAppFrame(SaiTtsFrameAppData frame)
        {
            if (frame.UserData != null)
            {
                // ��Ϣʱ��
                var timeDelay = this.DefenseStrategy.CalcTimeDelay(frame);

                // ֪ͨ���������¼� 
                this.Context.Observer.OnSaiUserDataArrival(this.Context.RsspEP.RemoteID,
                    frame.UserData, timeDelay, MessageDelayDefenseTech.TTS);
            }
        }

        public override void SendUserData(OutgoingPackage package)
        {
            // ���·������
            var seq = (ushort)this.Context.SeqNoManager.GetAndUpdateSendSeq();

            // �����û������ڷ��Ͷ����е��Ŷ�ʱ�ӡ�
            package.QueuingDelay = (UInt32)((DateTime.Now - package.CreationTime).TotalMilliseconds / 10);

            // ���㷢�ͷ�ʱ���
            var currentTime = TripleTimestamp.CurrentTimestamp;
            var senderTimeStamp = currentTime;

            if (currentTime > (package.ExtraDelay + package.QueuingDelay))
            {
                senderTimeStamp = currentTime - package.ExtraDelay - package.QueuingDelay;
            }

            var dtFrame = new SaiTtsFrameAppData(seq,
                senderTimeStamp,
                this.TTS.RemoteLastSendTimestamp,
                this.TTS.LocalLastRecvTimeStamp,
                package.UserData);

            this.Context.NextLayer.SendUserData(dtFrame.GetBytes());
        }

        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
