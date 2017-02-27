/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-29 11:39:33 
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
    /// ��ʾʹ��RSSP-IIЭ����յ����ݰ���
    /// </summary>
    public class IncomingPackage
    {
        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="remoteID">Զ�̽ڵ�ID</param>
        /// <param name="userData">�û�����</param>
        /// <param name="timeDelay">����ʱ�ӡ�</param>
        /// <param name="defenseTech">��Ϣ�ӳټ��ʹ�õķ���������</param>
        public IncomingPackage(uint remoteID, byte[] userData, long timeDelay, MessageDelayDefenseTech defenseTech)
        {
            this.CreationTime = DateTime.Now;
            this.RemoteID = remoteID;
            this.UserData = userData;
            this.TransmissionDelay = timeDelay;
            this.DefenseTech = defenseTech;
        }

        /// <summary>
        /// ��ȡ�˲�������Ĵ���ʱ�䡣
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// ��ȡ���ݷ��ͷ��Ľڵ�ID��
        /// </summary>
        public uint RemoteID { get; private set; }

        /// <summary>
        /// ��ȡ�յ����û����ݡ�
        /// </summary>
        public byte[] UserData { get; private set; }

        /// <summary>
        /// ��ȡ�û����ݵĴ���ʱ�ӡ�
        /// ��ֵС����ʱ��Ч������0ʱ��ʾ���ݵķ���ʱ�ӣ���λ��10���룩��
        /// </summary>
        public long TransmissionDelay { get; private set; }

        /// <summary>
        /// ��ȡ�û������ڱ��ؽ��ն����е��Ŷ�ʱ�ӣ���λ��10���룩��
        /// </summary>
        public long QueuingDelay { get; internal set; }

        /// <summary>
        /// ��ȡ��Ϣ�ӳ�ʹ�õķ���������
        /// </summary>
        public MessageDelayDefenseTech DefenseTech { get; private set; }


        /// <summary>
        /// ���ر�ʾ��������ַ���������Ϣ��
        /// </summary>
        /// <returns>�ַ���������Ϣ</returns>
        public override string ToString()
        {
            var sb = new StringBuilder(512);

            sb.AppendFormat("���ͷ�ID��{0}\r\n", this.RemoteID);
            sb.AppendFormat("����������{0}\r\n", this.DefenseTech);
            sb.AppendFormat("����ʱ�ӣ�{0}����λ��10���룩\r\n", this.TransmissionDelay);
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
