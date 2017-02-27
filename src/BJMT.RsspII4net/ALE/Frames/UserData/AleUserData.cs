/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 13:27:53 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using BJMT.RsspII4net.Exceptions;

namespace BJMT.RsspII4net.ALE.Frames
{
    /// <summary>
    /// ALE�û����ݻ��ࡣ
    /// </summary>
    abstract class AleUserData
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public AleUserData(AleFrameType frameType)
        {
            this.FrameType = frameType;
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// ��ȡЭ��֡���͡�
        /// </summary>
        public AleFrameType FrameType { get; private set; }

        /// <summary>
        /// ��ȡ���ݳ��ȡ�
        /// </summary>
        public abstract ushort Length { get; }
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
        public abstract void ParseBytes(byte[] bytes, int startIndex, int endIndex);

        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        public static AleUserData Parse(AleFrameType frameType, byte[] bytes, int startIndex, int endIndex)
        {
            AleUserData result = null;

            if (frameType == AleFrameType.ConnectionRequest)
            {
                result = new AleConnectionRequest();
            }
            else if (frameType == AleFrameType.ConnectionConfirm)
            {
                result = new AleConnectionConfirm();
            }
            else if (frameType == AleFrameType.DataTransmission)
            {
                result = new AleDataTransmission();
            }
            else if (frameType == AleFrameType.Disconnect)
            {
                result = new AleDisconnect();
            }
            else if (frameType == AleFrameType.SwitchN2R)
            {
                result = new AleSwitchN2R();
            }
            else if (frameType == AleFrameType.SwitchR2N)
            {
                result = new AleSwitchR2N();
            }
            else if (frameType == AleFrameType.KANA)
            {
                result = new AleKeepAliveOnNonActiveLink();
            }
            else if (frameType == AleFrameType.KAA)
            {
                result = new AleKeepAliveOnActiveLink();
            }
            else
            {
                throw new AleFrameParsingException(string.Format("�޷���ָ�����ֽ�������ΪALE����û����ݣ����� = {0}��", frameType));
            }
            
            result.ParseBytes(bytes, startIndex, endIndex);

            return result;
        }
        #endregion

    }
}
