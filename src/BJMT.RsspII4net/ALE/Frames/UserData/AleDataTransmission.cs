/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 13:48:05 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.ALE.Frames
{
    /// <summary>
    /// ALE�����ݴ���ṹ���塣
    /// </summary>
    class AleDataTransmission : AleUserData
    {
        /// <summary>
        /// �û����ݣ���AU3/AR/DT SaPDU��
        /// </summary>
        public byte[] UserData { get; set; }

        public override ushort Length
        {
            get
            {
                int value = 0;
                if (this.UserData != null)
                {
                    value += this.UserData.Length;
                }

                return (ushort)value;
            }
        }

        public AleDataTransmission()
            : base(AleFrameType.DataTransmission)
        {

        }

        public AleDataTransmission(byte[] userData)
            : this()
        {
            this.UserData = userData;
        }

        public override byte[] GetBytes()
        {
            var bytes = new byte[this.Length];
            int startIndex = 0;

            // �û�����
            if (this.UserData != null)
            {
                Array.Copy(this.UserData, 0, bytes, startIndex, this.UserData.Length);
            }

            return bytes;
        }

        public override void ParseBytes(byte[] bytes, int startIndex, int endIndex)
        {
            // �û�����
            var len = endIndex - startIndex + 1;
            if (len > 0)
            {
                this.UserData = new byte[len];
                Array.Copy(bytes, startIndex, this.UserData, 0, len);
            }
        }
    }
}
