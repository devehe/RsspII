/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BPL
//
// �� �� �ˣ�zhh_217
// �������ڣ�09/08/2011 11:30:44 
// ��    �䣺zhh_217@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;

namespace BJMT.RsspII4net.SAI.TTS
{
    /// <summary>
    /// ʱ��ƫ�Ƽ����������в�����λ��Ϊ10ms��
    /// </summary>
    class TimeOffsetCalculator
    {
        #region "Filed"
        /// <summary>
        /// �Ƿ�Ϊ���𷽡�true��ʾ���𷽣�false��ʾ��������
        /// </summary>
        private bool _isInitiator;

        #endregion

        #region "Constructor"
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="isInitiator">true��ʾΪ���𷽣�false��ʾΪӦ��</param>
        /// <param name="extraDelay">���ͷ���ϵͳ����Ӧ�����ݵĸ����ӳٵ��ܺ�</param>
        /// <param name="maxOffset">��������ʱ��ƫ�</param>
        public TimeOffsetCalculator(bool isInitiator, ushort extraDelay, uint maxOffset)
        {
            _isInitiator = isInitiator;

            this.ExtraDelay = extraDelay;
            this.MaxOffset = maxOffset;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// ��ȡ/���÷�����Ӧ��ʱ��ƫ�����ֵ֮�������������
        /// </summary>
        public UInt32 MaxOffset { get; private set; }

        /// <summary>
        /// ��ȡ/���÷��ͷ�����Ӧ�����ݵ��ӳ�ʱ�䡣
        /// </summary>
        public ushort ExtraDelay { get; private set; }

        /// <summary>
        /// ��ȡ/���÷��𷽵�һ��ʱ�����
        /// </summary>
        public UInt32 InitTimestamp1 { get; set; }
        /// <summary>
        /// ��ȡ/���÷��𷽵ڶ���ʱ�����
        /// </summary>
        public UInt32 InitTimestamp2 { get; set; }
        /// <summary>
        /// ��ȡ/���÷��𷽵�����ʱ�����
        /// </summary>
        public UInt32 InitTimestamp3 { get; set; }


        /// <summary>
        /// ��ȡ/����Ӧ�𷽵�һ��ʱ�����
        /// </summary>
        public UInt32 ResTimestamp1 { get; set; }
        /// <summary>
        /// ��ȡ/����Ӧ�𷽵ڶ���ʱ�����
        /// </summary>
        public UInt32 ResTimestamp2 { get; set; }
        /// <summary>
        /// ��ȡ/����Ӧ�𷽵�����ʱ�����
        /// </summary>
        public UInt32 ResTimestamp3 { get; set; }

        /// <summary>
        /// ��ȡ���𷽹�������ƫ��ֵ��
        /// </summary>
        public Int64 InitiatorMaxOffset { get; private set; }
        /// <summary>
        /// ��ȡ���𷽹������Сƫ��ֵ��
        /// </summary>
        public Int64 InitiatorMinOffset { get; private set; }


        /// <summary>
        /// ��ȡ/����Ӧ�𷽹�������ƫ��ֵ��
        /// </summary>
        public Int64 ResMaxOffset { get; set; }
        /// <summary>
        /// ��ȡ/����Ӧ�𷽹������Сƫ��ֵ��
        /// </summary>
        public Int64 ResMinOffset { get; set; }

        #endregion

        #region "Private methods"

        /// <summary>
        /// �����ͷ���ʱ���ת��Ϊ���շ���ʱ�����
        /// </summary>
        /// <param name="senderTimestamp">����ʱ���</param>
        /// <returns>����ʱ�����Ӧ�Ľ��շ�ʱ���</returns>
        private Int64 CovertTimestamp(UInt32 senderTimestamp)
        {
            if (_isInitiator)
            {
                return (Int64)senderTimestamp - (Int64)this.ExtraDelay + this.InitiatorMinOffset;
            }
            else
            {
                return (Int64)senderTimestamp - (Int64)this.ExtraDelay + this.ResMinOffset;
            }
        }
        #endregion

        #region "Public methods"
        /// <summary>
        /// ���𷽽���ʱ��ƫ�ƹ���
        /// </summary>
        public void EstimateInitOffset()
        {
            this.InitiatorMaxOffset = (Int64)this.InitTimestamp2 - (Int64)this.ResTimestamp2;
            this.InitiatorMinOffset = (Int64)this.InitTimestamp1 - (Int64)this.ResTimestamp1;
            
            LogUtility.Info(string.Format("���𷽽���ʱ��ƫ�ƹ��㣺minOffset = {0}, maxOffset = {1}",
                this.InitiatorMinOffset, this.InitiatorMaxOffset));
        }

        /// <summary>
        /// Ӧ�𷽽���ʱ��ƫ�ƹ���
        /// </summary>
        public void EstimateResOffset()
        {
            this.ResMaxOffset = (Int64)this.ResTimestamp3 - (Int64)this.InitTimestamp3;
            this.ResMinOffset = (Int64)this.ResTimestamp2 - (Int64)this.InitTimestamp2;

            LogUtility.Info(string.Format("Ӧ�𷽽���ʱ��ƫ�ƹ��㣺minOffset = {0}, maxOffset = {1}",
                this.ResMinOffset, this.ResMaxOffset));
        }

        /// <summary>
        /// �����жϹ����ƫ��ֵ�Ƿ���Ч
        /// </summary>
        /// <returns>true��ʾʱ��ƫ�ƹ���ֵ��Ч�������ʾ��Ч��</returns>
        public bool IsEstimationValid()
        {
            var value1 = Math.Abs(this.InitiatorMaxOffset + this.ResMinOffset);
            var value2 = Math.Abs(this.InitiatorMinOffset + this.ResMaxOffset);

            LogUtility.Info(string.Format("|Tinit_offset_max + Tres_offset_min| = {0}", value1));
            LogUtility.Info(string.Format("|Tinit_offset_min + Tres_offset_max| = {0}, Toffset_max = {1}", 
                value2, this.MaxOffset));

            return (value1 == 0) && (value2 < this.MaxOffset);
        }

        /// <summary>
        /// ����ʱ��ƫ��
        /// </summary>
        /// <param name="minOffset">��Сƫ��ֵ</param>
        /// <param name="maxOffset">���ƫ��ֵ</param>
        public void UpdateOffset(long minOffset, long maxOffset)
        {
            if (_isInitiator)
            {
                this.InitiatorMinOffset = minOffset;
                this.InitiatorMaxOffset = maxOffset;
            }
            else
            {
                this.ResMinOffset = minOffset;
                this.ResMaxOffset = maxOffset;
            }
        }

        /// <summary>
        /// ������Ϣ���ӳ�ʱ��
        /// </summary>
        /// <param name="localCurrentTime">���صĵ�ǰʱ�����</param>
        /// <param name="remoteSendTime">�Է��ķ���ʱ�����</param>
        /// <returns>��Ϣ��ʱ��</returns>
        public Int64 CalcTimeDelay(UInt32 localCurrentTime, UInt32 remoteSendTime)
        {
            var delay = (Int64)localCurrentTime - CovertTimestamp(remoteSendTime);

#if DEBUG
            if (delay < this.ExtraDelay)
            {
                LogUtility.Warn(String.Format("Delay = {0}, ReceiverCurrentTime = {1}, SenderTime = {2}\r\n",
                    delay, localCurrentTime, remoteSendTime));

                if (_isInitiator)
                {
                    LogUtility.Warn(String.Format("����ʹ�õ���Сʱ��ƫ�� = {0}\r\n", InitiatorMinOffset));
                }
                else
                {
                    LogUtility.Warn(String.Format("Ӧ��ʹ�õ���Сʱ��ƫ�� = {0}\r\n", ResMinOffset));
                }
            }
#endif

            // ����ʱ��
            if (delay < this.ExtraDelay)
            {
                delay = this.ExtraDelay;
            }

            return delay;
        }
        #endregion

    }
}
