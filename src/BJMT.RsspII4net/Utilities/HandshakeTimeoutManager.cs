/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-22 18:06:03 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.Utilities
{
    interface IHandshakeTimeoutObserver
    {
        void OnHandshakeTimeout();
    }

    /// <summary>
    /// ���ֳ�ʱ��������
    /// </summary>
    class HandshakeTimeoutManager : IDisposable
    {
        #region "Filed"
        private bool _disposed = false;
        /// <summary>
        /// һ����ʱ�������ڼ�������Ƿ�ʱ��
        /// </summary>
        private System.Timers.Timer _handshakeTimer;

        private IHandshakeTimeoutObserver _observer;
        #endregion

        #region "Constructor"
        public HandshakeTimeoutManager(int timeout, IHandshakeTimeoutObserver observer)
        {
            _observer = observer;

            _handshakeTimer = new System.Timers.Timer(timeout);
            _handshakeTimer.AutoReset = false;
            _handshakeTimer.Elapsed += OnTimerElapsed;
        }

        ~HandshakeTimeoutManager()
        {
            this.Dispose(false);
        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    CloseTimer();
                }
            }
        }
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        private void CloseTimer()
        {
            if (_handshakeTimer != null)
            {
                _handshakeTimer.Close();
                _handshakeTimer = null;
            }
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _observer.OnHandshakeTimeout();
            }
            catch (System.Exception /*ex*/)
            {
            }
        }
        #endregion

        #region "Public methods"
        public void Start()
        {
            if (_handshakeTimer != null)
            {
                _handshakeTimer.Start();
            }
        }

        public void Stop()
        {
            if (_handshakeTimer != null)
            {
                _handshakeTimer.Stop();
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
