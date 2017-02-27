/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-15 14:15:40 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;

namespace BJMT.RsspII4net.SAI.EC
{
    /// <summary>
    /// Excution cycle counter.
    /// ִ�����ڼ�������
    /// </summary>
    class EcCounter : IDisposable
    {
        #region "Filed"
        private bool _disposed = false;
        private System.Timers.Timer _timer;
        #endregion

        #region "Constructor"
        public EcCounter(uint id, uint cycle, uint initialValue)
        {
            if (cycle == 0)
            {
                throw new ArgumentException("EC���ڲ���Ϊ��ֵ��");
            }

            this.ID = id;
            this.ExcutionCycle = cycle;
            this.CurrentValue = initialValue;

            _timer = new System.Timers.Timer(cycle);
            _timer.AutoReset = true;
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        ~EcCounter()
        {
            this.Dispose(false);
        }
        #endregion

        #region "Properties"

        /// <summary>
        /// ��ȡ��ǰ�������ı�ʶ��
        /// </summary>
        public uint ID { get; private set; }

        /// <summary>
        /// ��ȡ��ǰ����ֵ��
        /// </summary>
        public uint CurrentValue { get; private set; }

        /// <summary>
        /// ��ȡ��ǰ��ִ�����ڣ����룩��
        /// </summary>
        public uint ExcutionCycle { get; private set; }
        #endregion

        #region "Private methods"
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    if (_timer != null)
                    {
                        _timer.Close();
                        _timer = null;
                    }
                }
            }
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.CurrentValue++;
            }
            catch (System.Exception)
            {
                this.CurrentValue = 0;
            }
        }
        #endregion

        #region "Public methods"
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UpdateCurrentValue(uint newValue)
        {
            this.CurrentValue = newValue;
        }
        #endregion

    }
}
