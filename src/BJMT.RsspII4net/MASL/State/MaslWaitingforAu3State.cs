/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 21:03:21 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Linq;
using BJMT.RsspII4net.Utilities;
using BJMT.RsspII4net.Exceptions;
using BJMT.RsspII4net.MASL.Frames;

namespace BJMT.RsspII4net.MASL.State
{
    class MaslWaitingforAu3State : MaslState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public MaslWaitingforAu3State(IMaslStateContext context)
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
            throw new NotAu3ObtainedAfterAu2Exception();
        }

        public override void HandleAu2Frame(MaslAu2Frame frame)
        {
            throw new NotAu3ObtainedAfterAu2Exception();
        }

        public override void HandleArFrame(MaslArFrame frame)
        {
            throw new NotAu3ObtainedAfterAu2Exception();
        }

        public override void HandleAu3Frame(Frames.MaslAu3Frame au3Frame)
        {
            var expectedDir = MaslFrameDirection.Client2Server;
            if (au3Frame.Direction != expectedDir)
            {
                throw new DirectionFlagException(expectedDir);
            }

            // �յ�AU3���ر����ּ�ʱ����
            this.Context.StopHandshakeTimer();

            // ��֤MAC
            var actualMac = au3Frame.MAC;
            var expectedMac = this.Context.AuMessageMacCalculator.CalcAu3MAC(au3Frame, this.Context.RsspEP.LocalID);
            if (!ArrayHelper.Equals(expectedMac, actualMac))
            {
                throw new MacInAu3Exception(string.Format("Au3��ϢMac����ʧ�ܣ�����ֵ={0}��ʵ��ֵ={1}",
                    HelperTools.ConvertToString(expectedMac),
                    HelperTools.ConvertToString(actualMac)));
            }

            // ����AR
            var arPkt = this.Context.AuMessageBuilder.BuildArPacket();
            this.Context.AleConnection.SendUserData(arPkt);

            // ���ӽ�����
            this.Context.Connected = true;
        }

        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
