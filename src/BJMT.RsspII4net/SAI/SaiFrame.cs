/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 15:41:31 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using BJMT.RsspII4net.SAI.EC.Frames;
using BJMT.RsspII4net.SAI.TTS.Frames;

namespace BJMT.RsspII4net.SAI
{
    /// <summary>
    /// SAI��Э��֡���ඨ�塣
    /// </summary>
    abstract class SaiFrame
    {
        /// <summary>
        /// TTS���λ�ĳ���
        /// </summary>
        public const byte TtsPaddingLength = 12;

        /// <summary>
        /// SAI���û�������󳤶ȡ�
        /// </summary>
        public const ushort MaxUserDataLength = 8 * 1024;

        #region "Filed"
        #endregion

        #region "Constructor"

        protected SaiFrame(SaiFrameType type, ushort seqNo)
        {
            this.FrameType = type;
            this.SequenceNo = seqNo;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// ��Ϣ����
        /// </summary>
        public SaiFrameType FrameType { get; set; }

        /// <summary>
        /// ���к�
        /// </summary>
        public UInt16 SequenceNo { get; set; }
        #endregion

        #region "Virtual methods"

        /// <summary>
        /// ���л�Ϊ�ֽ�����
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetBytes();

        /// <summary>
        /// ���ֽ��������л���
        /// </summary>
        /// <param name="bytes"></param>
        public abstract void ParseBytes(byte[] bytes);
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        public static bool IsEcFrame(SaiFrameType type)
        {
            return (type == SaiFrameType.EC_Start
                || type == SaiFrameType.EC_AppDataAskForAck
                || type == SaiFrameType.EC_AppDataAcknowlegment
                || type == SaiFrameType.EC_AppData) ;
        }

        public static bool IsTtsFrame(SaiFrameType type)
        {
            return (type == SaiFrameType.TTS_OffsetStart
                || type == SaiFrameType.TTS_OffsetAnswer1
                || type == SaiFrameType.TTS_OffsetAnswer2
                || type == SaiFrameType.TTS_OffsetEstimate
                || type == SaiFrameType.TTS_OffsetEnd
                || type == SaiFrameType.TTS_AppData) ;
        }

        public static SaiFrame Parse(byte[] bytes)
        {
            SaiFrame theFrame = null;

            var theFrameType = (SaiFrameType)bytes[0];

            if (theFrameType == SaiFrameType.TTS_OffsetStart)
            {
                theFrame = new SaiTtsFrameOffsetStart();
            }
            else if (theFrameType == SaiFrameType.TTS_OffsetAnswer1)
            {
                theFrame = new SaiTtsFrameOffsetAnswer1();
            }
            else if (theFrameType == SaiFrameType.TTS_OffsetAnswer2)
            {
                theFrame = new SaiTtsFrameOffsetAnswer2();
            }
            else if (theFrameType == SaiFrameType.TTS_OffsetEstimate)
            {
                theFrame = new SaiTtsFrameEstimate();
            }
            else if (theFrameType == SaiFrameType.TTS_OffsetEnd)
            {
                theFrame = new SaiTtsFrameOffsetEnd();
            }
            else if (theFrameType == SaiFrameType.TTS_AppData)
            {
                theFrame = new SaiTtsFrameAppData();
            }
            else if (theFrameType == SaiFrameType.EC_Start)
            {
                theFrame = new SaiEcFrameStart();
            }
            else if (theFrameType == SaiFrameType.EC_AppData)
            {
                theFrame = new SaiEcFrameApplication();
            }
            else if (theFrameType == SaiFrameType.EC_AppDataAskForAck)
            {
                theFrame = new SaiEcFrameAskForAck();
            }
            else if (theFrameType == SaiFrameType.EC_AppDataAcknowlegment)
            {
                theFrame = new SaiEcFrameAcknowlegment();
            }
            else
            {
                throw new InvalidOperationException(string.Format("�޷�����ָ����Sai֡������ʶ�������{0}��", theFrameType));
            }

            theFrame.ParseBytes(bytes);

            return theFrame;
        }
        #endregion

    }
}
