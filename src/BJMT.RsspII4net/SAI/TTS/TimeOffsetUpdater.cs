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
using BJMT.RsspII4net.SAI.Events;
using BJMT.RsspII4net.SAI.TTS.Frames;

namespace BJMT.RsspII4net.SAI.TTS
{
    /// <summary>
    /// ʱ��ƫ�Ƹ������Ĺ۲����ӿڡ�
    /// </summary>
    interface ITimeOffsetUpdaterObserver
    {
        uint RemoteLastSendTimestamp { get; }

        uint LocalLastRecvTimeStamp { get; }

        void OnTimeOffsetUpdated(Int64 minOffset, Int64 maxOffset);
    }

    /// <summary>
    /// ʱ��ƫ�Ƹ�������
    /// </summary>
    class TimeOffsetUpdater : IDisposable
    {
        /// <summary>
        /// ʱ��ƫ����������С���������λ���룩
        /// </summary>
        private const byte MinInterval = 10;

        #region "Filed"

        private bool _disposed = false;

        /// <summary>
        /// ʱ��ƫ�Ƹ��¼�ʱ��
        /// </summary>
        private System.Timers.Timer _timer = null;

        private ITimeOffsetUpdaterObserver _observer = null;

        private ISaiFrameTransport _ttsFrameTransport = null;

        /// <summary>
        /// ��һ�η�������������ʱ��ʱ�����
        /// </summary>
        private uint _lastRequestTimestamp;

        /// <summary>
        /// ��һ�λ�Ӧʱ���������ĵ�ʱ�䡣
        /// </summary>
        private DateTime _lastResponseTime;

        #endregion

        #region "Constructor"
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="frameTransport"></param>
        /// <param name="observer"></param>
        /// <param name="interval">TTS�������ڣ���λ���룩</param>
        public TimeOffsetUpdater(ISaiFrameTransport frameTransport, ITimeOffsetUpdaterObserver observer, int interval)
        {
            if (interval < MinInterval)
            {
                throw new ArgumentException(string.Format("TTS�������ڲ���С��{0}�롣", MinInterval));
            }

            _ttsFrameTransport = frameTransport;
            _ttsFrameTransport.SaiFrameReceived += OnSaiFrameReceived;

            _observer = observer;

            this.CreateTimer(interval);

            this.StartTimer();
        }

        ~TimeOffsetUpdater()
        {
            Dispose(false);
        }
        #endregion

        #region "Properties"
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        private void CreateTimer(int interval)
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = interval * 1000;
            _timer.AutoReset = true;
            _timer.Elapsed += OnTimerElapsed;
        }

        private void CloseTimer()
        {
            if (_timer != null)
            {
                _timer.Close();
                _timer = null;
            }
        }

        private void StartTimer()
        {
            if (_timer != null)
            {
                _timer.Start();
            }
        }

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// ���¼�ʱ����ʱ
        /// </summary>
        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // ������������֡�������Է���
                this.SendRequestFrame();
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        private void SendRequestFrame()
        {
            var seq = (ushort)_ttsFrameTransport.NextSendSeq();
            
            // ��¼����ʱ�䡣
            _lastRequestTimestamp = TripleTimestamp.CurrentTimestamp;

            var reqFrame = new SaiTtsFrameAppData(seq,
                _lastRequestTimestamp,
                _observer.RemoteLastSendTimestamp,
                _observer.LocalLastRecvTimeStamp,
                null);

            _ttsFrameTransport.SendSaiFrame(reqFrame);
        }

        private void SendResponseFrame()
        {
            var seq = (ushort)_ttsFrameTransport.NextSendSeq();

            // 
            var reqFrame = new SaiTtsFrameAppData(seq,
                TripleTimestamp.CurrentTimestamp,
                _observer.RemoteLastSendTimestamp,
                _observer.LocalLastRecvTimeStamp,
                null);

            _ttsFrameTransport.SendSaiFrame(reqFrame);
        }

        private void OnSaiFrameReceived(object sender, SaiFrameIncomingEventArgs e)
        {
            try
            {
                if (e.Frame.FrameType != SaiFrameType.TTS_AppData)
                {
                    return;
                }

                var ttsAppFrame = e.Frame as SaiTtsFrameAppData;
                
                // ���û��UserData����˵����TTSʱ��ƫ�������������Ӧ���ġ�
                if (ttsAppFrame.UserData != null)
                {
                    return;
                }

                // ����յ���֡�Ƿ�Ϊ��Ӧ���ġ�
                var isResponse = (ttsAppFrame.ReceiverLastSendTimestamp == _lastRequestTimestamp);

                if (isResponse)
                {
                    this.HandleResponseFrame(ttsAppFrame, TripleTimestamp.CurrentTimestamp);
                }
                else
                {
                    // �յ�ʱ��ƫ�Ƹ���������ظ�������Ӧ��
                    var isRequest = (DateTime.Now - _lastResponseTime).TotalSeconds;
                    if (isRequest > MinInterval)
                    {
                        _lastResponseTime = DateTime.Now;

                        this.SendResponseFrame();
                    }
                    else
                    {
                        // ���ϴη�����Ӧ��ʱ���ֵС����С����ʱ�䣬����Ӧ��
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        /// <summary>
        /// ����ʱ��ƫ��������Ӧ��Ϣ
        /// </summary>
        /// <param name="offsetRsp">��Ӧ����</param>
        /// <param name="currentTimestamp">�յ���Ӧ����ʱ��ʱ���</param>
        private void HandleResponseFrame(SaiTtsFrame offsetRsp, UInt32 currentTimestamp)
        {
            try
            {
                long minOffset = (long)offsetRsp.ReceiverLastSendTimestamp - (long)offsetRsp.SenderLastRecvTimestamp;
                long maxOffset = (long)currentTimestamp - (long)offsetRsp.SenderTimestamp;

                // Notify observer
                _observer.OnTimeOffsetUpdated(minOffset, maxOffset);
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }
        #endregion

        #region "Public methods"

        /// <summary>
        /// ��������ʱ��ƫ������
        /// </summary>
        public void UpdateClockOffset()
        {
            this.StopTimer();

            this.SendRequestFrame();

            this.StartTimer();
        }
        #endregion


        #region IDisposable ��Ա

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_ttsFrameTransport != null)
                    {
                        _ttsFrameTransport.SaiFrameReceived -= OnSaiFrameReceived;
                    }

                    CloseTimer();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
