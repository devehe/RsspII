/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BPL
//
// �� �� �ˣ�zhh_217
// �������ڣ�09/08/2011 14:32:29 
// ��    �䣺zhh_217@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾ 2009-2015 ��������Ȩ��
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace BJMT.RsspII4net.SAI.TTS
{
    /// <summary>
    /// ����ʱ���
    /// </summary>
    class TripleTimestamp
    {
        #region "Filed"

        private ITripleTimestampObserver _observer = null;
        #endregion

        #region "Constructor"
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="observer"></param>
        public TripleTimestamp(ITripleTimestampObserver observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException();
            }

            _observer = observer;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// ��ȡ��ǰ��ʱ���
        /// 
        /// 1. Environment.TickCount: �����Ե�ֵ��ϵͳ��ʱ������������ 32 λ�з�����������ʽ�洢��
        ///    ��ˣ����ϵͳ�������У�TickCount ����Լ 24.9 ���ڴ�������� Int32.MaxValue��Ȼ��
        ///    ���� Int32.MinValue������һ�������������ڽ������� 24.9 ���ڵ������㡣
        /// 
        /// 2. TickCount���Եķֱ���С�� 500 ���롣
        /// 
        /// �μ���"http://msdn.microsoft.com/zh-cn/library/system.environment.tickcount(v=vs.80).aspx?cs-save-lang=1&amp;cs-lang=csharp#code-snippet-1
        /// ע�����TTS��ʱ���ʹ��8���ֽڱ�ʾ�������ʹ��DateTime.Now.Ticks(long)/10000��
        /// </summary>
        public static UInt32 CurrentTimestamp { get { return (UInt32)Environment.TickCount / 10; } }

        /// <summary>
        /// ��ȡһ��ֵ�����ڶԷ���һ�δ��͸�������ʱ�����
        /// </summary>
        public UInt32 RemoteLastSendTimestamp { get; private set; }

        /// <summary>
        /// ��ȡһ��ֵ�����ڱ�ʾ��һ�δӶԷ����յ���Ϣʱ��ʱ�����
        /// </summary>
        public UInt32 LocalLastRecvTimeStamp { get; private set; }
        
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        /// <summary>
        /// ��λ
        /// </summary>
        public void Reset()
        {
            this.RemoteLastSendTimestamp = 0;
            this.LocalLastRecvTimeStamp = 0;
        }

        /// <summary>
        /// ���¡���һ�ν��շ�ʱ�������
        /// </summary>
        /// <param name="newValue">�µ�ʱ�����</param>
        public void UpdateRemoteLastSendTimestamp(uint newValue)
        {
            // ������ͷ���һ�ε�ʱ��������µ�ʱ�������˵�������ˡ�����㡱����
            if (this.RemoteLastSendTimestamp > newValue)
            {
                _observer.OnTimestampZeroPassed(newValue, this.RemoteLastSendTimestamp);
            }

            this.RemoteLastSendTimestamp = newValue; 
        }

        /// <summary>
        /// ���¡���һ���յ���Ϣʱ��ʱ�������
        /// </summary>
        /// <param name="newValue">�µ�ʱ�����</param>
        public void UpdateLocalLastRecvTimeStamp(uint newValue)
        {
            this.LocalLastRecvTimeStamp = newValue;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(200);

            sb.AppendFormat("��ǰʱ���={0}����һ�ν��շ�ʱ���={1}����һ���յ���Ϣʱ��ʱ���={2}��\r\n",
                    TripleTimestamp.CurrentTimestamp, this.RemoteLastSendTimestamp, this.LocalLastRecvTimeStamp);

            return sb.ToString();
        }
        #endregion

    }
}
