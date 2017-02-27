/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 10:56:39 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net
{
    /// <summary>
    /// ��ʾʹ��RSSP-IIЭ�鷢�͵����ݰ���
    /// </summary>
    public class OutgoingPackage
    {
        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="DestID">Ŀ�ĵ�ַ��</param>
        /// <param name="userData">�û����ݡ�</param>
        public OutgoingPackage(IEnumerable<UInt32> DestID, byte[] userData)
        {
            this.CreationTime = DateTime.Now;
            this.UserData = userData;
            this.DestID = new List<UInt32>(DestID);
        }

        /// <summary>
        /// ��ȡ�˲�������Ĵ���ʱ�䡣
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// ��Ҫ���͵��û�����
        /// </summary>
        public byte[] UserData { get; private set; }

        /// <summary>
        /// �û����ݵ�Ŀ�ĵ�ַ�б�
        /// </summary>
        public IEnumerable<UInt32> DestID { get; private set; }

        /// <summary>
        /// �û����ݵĸ���ʱ�ӣ�ת������ʱ�����õ�������λ��10ms
        /// </summary>
        public UInt32 ExtraDelay { get; private set; }

        /// <summary>
        /// ��ȡ�û������ڱ��ؽ��ն����е��Ŷ�ʱ�ӣ���λ��10���룩��
        /// </summary>
        public uint QueuingDelay { get; internal set; }


        /// <summary>
        /// ���ر�ʾ��������ַ���������Ϣ��
        /// </summary>
        /// <returns>�ַ���������Ϣ</returns>
        public override string  ToString()
        {
            var sb = new StringBuilder(512);

            sb.AppendFormat("Ŀ�ĵ�ַ��{0}\r\n", string.Join(",", this.DestID.ToArray()));
            sb.AppendFormat("����ʱ�ӣ�{0}����λ��10���룩\r\n", this.ExtraDelay);
            sb.AppendFormat("�Ŷ�ʱ�ӣ�{0}����λ��10���룩\r\n", this.QueuingDelay);
            
            if (this.UserData != null)
            {
                sb.AppendFormat("�û����ݣ�{0}����\r\n", this.UserData.Length);
                foreach (byte data in this.UserData)
                {
                    sb.AppendFormat("{0:X2} ", data);
                }
            }
            else
            {
                sb.AppendFormat("�û����ݣ���\r\n", this.UserData.Length);
            }

            return sb.ToString();
        }
    }
}
