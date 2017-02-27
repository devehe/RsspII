/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 15:16:34 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net
{
    /// <summary>
    /// RSSP-IIЭ���ս�㶨�塣
    /// </summary>
    class RsspEndPoint
    {
        /// <summary>
        /// ��ȡ�ս��ID
        /// </summary>
        public string ID
        {
            get
            {
                if (this.IsInitiator)
                {
                    return string.Format("Client(LID_{0}, RID_{1})", this.LocalID, this.RemoteID);
                }
                else
                {
                    return string.Format("Server(LID_{0}, RID_{1})", this.LocalID, this.RemoteID);
                }
            }
        }

        /// <summary>
        /// ���ڵ��Ƿ�Ϊ���𷽡�
        /// </summary>
        public bool IsInitiator { get; set; }

        /// <summary>
        /// ����ID��
        /// </summary>
        public uint LocalID { get; set; }
        /// <summary>
        /// �����豸���͡�
        /// </summary>
        public byte LocalEquipType { get; set; }

        /// <summary>
        /// Ӧ�����͡�
        /// </summary>
        public byte ApplicatinType { get; set; }

        /// <summary>
        /// Զ��ID��
        /// </summary>
        public uint RemoteID { get; set; }
        /// <summary>
        /// Զ���豸���͡�
        /// </summary>
        public byte RemoteEquipType { get; set; }

        /// <summary>
        /// �������ͣ�Ĭ��ֵΪD�����
        /// </summary>
        public ServiceType ServiceType { get; set; }
        /// <summary>
        /// ��Ϣ����ʹ�õļ����㷨��Ĭ��ֵΪ3DES��
        /// </summary>
        public EncryptionAlgorithm Algorithm { get; set; }

        /// <summary>
        /// ��Ϣ�ӳٷ���������Ĭ��ֵ��EC��
        /// </summary>
        public MessageDelayDefenseTech DefenseTech { get; set; }

        /// <summary>
        /// ��ȡ/����192λ����֤��ԿKeyMAC��24�ֽڣ���
        /// </summary>
        public byte[] AuthenticationKeys { get; set; }

        /// <summary>
        /// ��ȡ/����EC����ֵ��������ʹ��EC��������ʱ��Ч��
        /// ��ֵ��ʾ��Ч��
        /// </summary>
        public ushort EcInterval { get; set; }

        /// <summary>
        /// ��ȡ/����һ��ֵ�����ڱ�ʾ��Ϣ���м�����N��N-1�Ǵ�������ʧ��Ϣ����������Ҫ����Nֵ��Nֵ���ڻ����1��
        /// </summary>
        public byte SeqNoThreshold { get; set; }


        /// <summary>
        /// ��ȡ�ɽ��ܵĿͻ����б� �����ڷ���������Ч��
        /// </summary>
        public IEnumerable<KeyValuePair<uint, List<IPEndPoint>>> AcceptableClients { get; private set; }

        /// <summary>
        /// ���캯��
        /// </summary>
        public RsspEndPoint(uint localID, uint remoteID,
            byte appType,
            byte localEquipType,
            bool isInitiator,
            byte seqWinLen,
            ushort ecInterval,
            byte[] macKey, IEnumerable<KeyValuePair<uint, List<IPEndPoint>>> acceptableClients/* = null*/)
        {
            this.ServiceType = ServiceType.D;
            this.Algorithm = EncryptionAlgorithm.TripleDES;
            this.DefenseTech = MessageDelayDefenseTech.EC;
            this.SeqNoThreshold = seqWinLen;

            this.LocalID = localID;
            this.RemoteID = remoteID;
            this.ApplicatinType = appType;
            this.LocalEquipType = localEquipType;
            this.IsInitiator = isInitiator;            
            this.EcInterval = ecInterval;

            this.AuthenticationKeys = new byte[macKey.Length];
            Array.Copy(macKey, this.AuthenticationKeys, this.AuthenticationKeys.Length);

            if (acceptableClients != null && acceptableClients.Count() > 0)
            {
                this.AcceptableClients = new List<KeyValuePair<uint, List<IPEndPoint>>>(acceptableClients);
            }
        }

        public bool IsClientAcceptable(uint clientId, IPEndPoint clientEP)
        {
            if (this.AcceptableClients == null) return true;

            if (this.AcceptableClients.Count() == 0) return true;

            foreach (var item in this.AcceptableClients)
            {
                if(item.Key == clientId)
                {
                    return HelperTools.IsEndPointValid(item.Value, clientEP);
                }
            }

            return false;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(200);

            sb.AppendFormat("����ID={0}��Զ��ID={1}���Ƿ���={2}��", 
                this.LocalID, this.RemoteID, this.IsInitiator);

            sb.AppendFormat("�����豸����={0}, Զ���豸����={1}, Ӧ������={2}����������={3}��",
                this.LocalEquipType, this.RemoteEquipType, this.ApplicatinType, this.ServiceType);

            sb.AppendFormat("��Ϣ���м�����N={0}��",
                this.SeqNoThreshold);

            if (this.DefenseTech == MessageDelayDefenseTech.EC)
            {
                sb.AppendFormat("��������={0}������={1}����\r\n", this.DefenseTech, this.EcInterval);
            }
            else
            {
                sb.AppendFormat("��������={0}��\r\n", this.DefenseTech);
            }

            return sb.ToString();
        }

    }
}
