/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-15 14:36:18 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System.Text;
using BJMT.RsspII4net.SAI.EC.Frames;

namespace BJMT.RsspII4net.SAI.EC
{
    /// <summary>
    /// EC�������ԡ�
    /// </summary>
    class EcDefenseStrategy : DefenseStrategy
    {
        #region "Filed"
        private bool _disposed = false;

        /// <summary>
        /// ���ؼ�������
        /// </summary>
        private EcCounter _localCounter;

        /// <summary>
        /// Զ�̼�������
        /// </summary>
        private EcCounter _remoteCounter;

        /// <summary>
        /// Deltaֵ����״̬3�Ĵ�����
        /// </summary>
        private byte _state3Count;
        #endregion

        #region "Constructor"
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="localID">���ر��</param>
        /// <param name="localCycle">EC����</param>
        public EcDefenseStrategy(uint localID, uint localCycle)
        {
            _localCounter = new EcCounter(localID, localCycle, 0);
        }
        #endregion

        #region "Properties"
        
        #endregion

        #region "Override methods"

        protected override long CalcEcTimeDelay(SaiEcFrame ecFrame)
        {
            var actualRemoteEcValue = ecFrame.EcValue;
            var delta = (long)_remoteCounter.CurrentValue - (long)actualRemoteEcValue;

            if (delta > 3)
            {
                _state3Count++;
            }

            // ���DeltaС��0��������5������Delta��ֵ��3���ϣ���ִ����������
            if (delta < 0)
            {
                return 0;
            }
            else if (_state3Count > 5)
            {
                _remoteCounter.UpdateCurrentValue(actualRemoteEcValue);
                _state3Count = 0;
                return 0;
            }
            else
            {
                return (delta * _remoteCounter.ExcutionCycle) / 10;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    if (_localCounter != null)
                    {
                        _localCounter.Dispose();
                        _localCounter = null;
                    }

                    if (_remoteCounter != null)
                    {
                        _remoteCounter.Dispose();
                        _remoteCounter = null;
                    }
                }

                base.Dispose(disposing);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(200);

            sb.AppendFormat("EC������");

            if (_localCounter != null)
            {
                sb.AppendFormat("����EC����={0}����ǰECֵ={1}��", _localCounter.ExcutionCycle, _localCounter.CurrentValue);
            }

            if (_remoteCounter != null)
            {
                sb.AppendFormat("Զ��EC����= {0}��������ECֵ= {1}��\r\n", _remoteCounter.ExcutionCycle, _remoteCounter.CurrentValue);
            }

            return sb.ToString();
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"

        public void StartRemoteCounter(uint remoteID, uint interval, uint initialValue)
        {
            _remoteCounter = new EcCounter(remoteID, interval, initialValue);
        }

        /// <summary>
        /// ��ȡ����EC�ĵ�ǰֵ��
        /// </summary>
        /// <returns></returns>
        public uint GetLocalEcValue()
        {
            return _localCounter.CurrentValue;
        }
        #endregion

    }
}
