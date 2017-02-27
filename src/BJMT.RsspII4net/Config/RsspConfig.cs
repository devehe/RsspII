/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 15:49:25 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.Config
{
    /// <summary>
    /// RSSP�ڵ������ࡣ
    /// </summary>
    public class RsspConfig
    {
        /// <summary>
        /// ���ر�š�
        /// </summary>
        public uint LocalID { get; set; }

        /// <summary>
        /// Ӧ�����͡�
        /// </summary>
        public byte ApplicationType { get; set; }

        /// <summary>
        /// �����豸���͡�
        /// </summary>
        public byte LocalEquipType { get; set; }

        /// <summary>
        /// �Ƿ�Ϊ�������𷽡�
        /// </summary>
        public bool IsInitiator { get; set; }

        /// <summary>
        /// ��ȡ/������Ϣ�ӳٷ���������Ĭ��EC������
        /// </summary>
        public MessageDelayDefenseTech DefenseTech { get; set; }

        /// <summary>
        /// ��ȡ/���÷������ͣ�Ĭ��D�����
        /// </summary>
        public ServiceType @ServiceType { get; set; }

        /// <summary>
        /// ��Ϣ����ʹ�õļ����㷨��Ĭ��ֵΪ3DES��
        /// </summary>
        public EncryptionAlgorithm Algorithm { get; set; }

        /// <summary>
        /// ��ȡ/����192λ����֤��Կ��24�ֽڣ���
        /// </summary>
        public byte[] AuthenticationKeys { get; set; }

        /// <summary>
        /// ��ȡ/����EC����ֵ��Ĭ��ֵ1000���롣������ʹ��EC��������ʱ��Ч��
        /// </summary>
        public ushort EcInterval { get; set; }

        /// <summary>
        /// ��ȡ/����һ��ֵ�����ڱ�ʾ��Ϣ���м�����N��N-1�Ǵ�������ʧ��Ϣ��������
        /// ��Ҫ����Nֵ��Nֵ���ڻ����1��Ĭ��ֵ3��
        /// </summary>
        public byte SeqNoThreshold { get; set; }

        /// <summary>
        /// ���캯����
        /// </summary>
        public RsspConfig()
        {
            this.DefenseTech = MessageDelayDefenseTech.EC;
            this.Algorithm = EncryptionAlgorithm.TripleDES;
            this.ServiceType = ServiceType.D;
            this.EcInterval = 1000;
            this.SeqNoThreshold = 3;

            // Ĭ��ֵ��
            this.AuthenticationKeys = new byte[] 
            { 
                0xA3, 0x45, 0x34, 0x68, 0x98, 0x01, 0x2A, 0xBF,
                0xCD, 0xBE, 0x34, 0x56, 0x78, 0xBF, 0xEA, 0x32,
                0x12, 0xAE, 0x34, 0x21, 0x45, 0x78, 0x98, 0x50
            };
        }
    }
}
