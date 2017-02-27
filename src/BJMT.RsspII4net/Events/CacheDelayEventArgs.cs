/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BPL
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-6-20 14:12:45 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾ 2009����������Ȩ��
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace BJMT.RsspII4net.Events
{
    /// <summary>
    /// һ���¼������࣬�����淢������ʱʹ�á�
    /// </summary>
    public abstract class CacheDelayEventArgs : EventArgs
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        /// <summary>
        /// �޲ι��캯����
        /// </summary>
        public CacheDelayEventArgs()
        {

        }

        /// <summary>
        /// �������Ĺ��캯����
        /// </summary>
        /// <param name="name">���е����ơ�</param>
        /// <param name="count">�����еĸ�����</param>
        /// <param name="threshold">��ֵ��</param>
        /// <param name="farthestTime">��Զ��ʱ�����</param>
        public CacheDelayEventArgs(string name, int count, int threshold, DateTime farthestTime)
        {
            this.Name = name;
            this.Count = count;
            this.Threshold = threshold;
            this.FarthestTimeStamp = farthestTime;
        }
        #endregion

        #region "Properties"

        /// <summary>
        /// ��ȡ/����һ��ֵ�����ڱ�ʾ���е����ơ�
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// �����е�Ԫ�ظ�����
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// ��ȡ/����һ��ֵ�����ڱ�ʾ�����ļ��ֵ��
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// ��Զ��ʱ�����
        /// </summary>
        public DateTime FarthestTimeStamp { get; set; }
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }

    /// <summary>
    /// һ���¼������࣬�����ͻ��淢���ӳ�ʱʹ�á�
    /// </summary>
    public class OutgoingCacheDelayedEventArgs : CacheDelayEventArgs
    {
        /// <summary>
        /// �������Ĺ��캯����
        /// </summary>
        /// <param name="name">���е����ơ�</param>
        /// <param name="count">�����еĸ�����</param>
        /// <param name="threshold">��ֵ��</param>
        /// <param name="farthestTime">��Զ��ʱ�����</param>
        public OutgoingCacheDelayedEventArgs(string name, int count, int threshold, DateTime farthestTime)
            :base(name, count, threshold, farthestTime)
        {
        }
    }

    /// <summary>
    /// һ���¼������࣬�����ջ��淢���ӳ�ʱʹ�á�
    /// </summary>
    public class IncomingCacheDelayedEventArgs : CacheDelayEventArgs
    {
        /// <summary>
        /// �������Ĺ��캯����
        /// </summary>
        /// <param name="name">���е����ơ�</param>
        /// <param name="count">�����еĸ�����</param>
        /// <param name="threshold">��ֵ��</param>
        /// <param name="farthestTime">��Զ��ʱ�����</param>
        public IncomingCacheDelayedEventArgs(string name, int count, int threshold, DateTime farthestTime)
            : base(name, count, threshold, farthestTime)
        {
        }
    }
}
