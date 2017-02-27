/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-16 13:00:02 
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

namespace BJMT.RsspII4net.SAI.TTS
{
    /// <summary>
    /// TTS�������ԡ�
    /// </summary>
    class TtsDefenseStrategy : DefenseStrategy, ITripleTimestampObserver, ITimeOffsetUpdaterObserver
    {
        #region "TTS��������"

        /// <summary>
        /// ������Ӧ��ʱ��ƫ�����ֵ֮�����������������λ��10ms����Ĭ��ֵ50��
        /// </summary>
        public const UInt32 MaxDifference = 50;

        /// <summary>
        /// ���Ӵ����ӳٵĹ���ֵ����λ��10ms����Ĭ��ֵ3����ʾ30ms
        /// </summary>
        public const UInt16 ExtraDelay = 3;

        /// <summary>
        /// ʱ��ƫ�Ƹ������ڡ�����λ���룩��Ĭ��ֵ300�룬��5���ӡ�
        /// </summary>
        public const UInt16 TimeOffserUpdateInterval = 300;

        #endregion

        #region "Filed"
        private bool _disposed = false;

        private TimeOffsetUpdater _timeOffsetUpdater;
        #endregion

        #region "Constructor"
        public TtsDefenseStrategy(ISaiFrameTransport frameTransport, bool isInitiator)
        {
            this.Calculator = new TimeOffsetCalculator(isInitiator, ExtraDelay, MaxDifference);

            this.LocalTts = new TripleTimestamp(this);

            _timeOffsetUpdater = new TimeOffsetUpdater(frameTransport, this, TimeOffserUpdateInterval);
        }
        #endregion

        #region "Properties"

        /// <summary>
        /// ��������ʱ�������
        /// </summary>
        public TripleTimestamp LocalTts { get; private set; }

        /// <summary>
        /// ʱ��ƫ�Ƽ�������
        /// </summary>
        public TimeOffsetCalculator Calculator { get; private set; }
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"

        protected override long CalcTtsTimeDelay(SaiTtsFrame ttsFrame) 
        {
            return this.Calculator.CalcTimeDelay(TripleTimestamp.CurrentTimestamp, ttsFrame.SenderTimestamp);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    if (_timeOffsetUpdater != null)
                    {
                        _timeOffsetUpdater.Dispose();
                        _timeOffsetUpdater = null;
                    }
                }

                base.Dispose(disposing);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(200);

            sb.AppendFormat("TTS������{0}\r\n", this.LocalTts);

            return sb.ToString();
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"

        #endregion


        #region "TripleTimestamp�ӿ�"
        void ITripleTimestampObserver.OnTimestampZeroPassed(uint latestTimestamp, uint lastTimestamp)
        {
            // �����ʱ����TTSʱ��ƫ��������
            _timeOffsetUpdater.UpdateClockOffset();
        }
        #endregion

        #region "ITimeOffsetUpdater"
        uint ITimeOffsetUpdaterObserver.RemoteLastSendTimestamp { get { return this.LocalTts.RemoteLastSendTimestamp; } }

        uint ITimeOffsetUpdaterObserver.LocalLastRecvTimeStamp { get { return this.LocalTts.LocalLastRecvTimeStamp; } }

        void ITimeOffsetUpdaterObserver.OnTimeOffsetUpdated(long minOffset, long maxOffset)
        {
            this.Calculator.UpdateOffset(minOffset, maxOffset);
        }
        #endregion
    }
}
