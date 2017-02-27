/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 21:03:59 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using BJMT.RsspII4net.Exceptions;
using BJMT.RsspII4net.MASL.Frames;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.MASL.State
{
    class MaslConnectedState : MaslState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public MaslConnectedState(IMaslStateContext context)
            :base(context)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        public override void HandleAu1Frame(MaslAu1Frame frame)
        {
            throw new SequenceIntegrityException();
        }

        public override void HandleAu2Frame(MaslAu2Frame frame)
        {
            throw new SequenceIntegrityException();
        }

        public override void HandleAu3Frame(MaslAu3Frame frame)
        {
            throw new SequenceIntegrityException();
        }

        public override void HandleArFrame(MaslArFrame frame)
        {
            throw new SequenceIntegrityException();
        }

        public override void HandleDtFrame(Frames.MaslDtFrame dtFrame)
        {
            var expectedDir = !this.Context.RsspEP.IsInitiator ? MaslFrameDirection.Client2Server : MaslFrameDirection.Server2Client;
            if (dtFrame.Direction != expectedDir)
            {
                throw new DirectionFlagException(expectedDir);
            }

            // ��֤MAC
            var actualMac = dtFrame.MAC;
            var expectedMac = this.Context.AuMessageMacCalculator.CalcDtMAC(dtFrame, this.Context.RsspEP.LocalID);
            if (!ArrayHelper.Equals(expectedMac, actualMac))
            {
                throw new MacInDtException(string.Format("Dt��ϢMac����ʧ�ܣ�����ֵ={0}��ʵ��ֵ={1}",
                    HelperTools.ConvertToString(expectedMac),
                    HelperTools.ConvertToString(actualMac)));
            }

            // �ύ���ϲ㡣
            this.Context.Observer.OnMaslUserDataArrival(dtFrame.UserData);             
        }

        public override void SendUserData(byte[] saiPacket)
        {
            var maslPacket = this.Context.AuMessageBuilder.BuildDtPacket(saiPacket);

            this.Context.AleConnection.SendUserData(maslPacket);
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
