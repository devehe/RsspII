/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 21:03:41 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Linq;
using BJMT.RsspII4net.Utilities;
using BJMT.RsspII4net.MASL.Frames;
using BJMT.RsspII4net.Exceptions;

namespace BJMT.RsspII4net.MASL.State
{
    class MaslWaitingforArState : MaslState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public MaslWaitingforArState(IMaslStateContext context)
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
            throw new NotArObtainedAfterAu3Exception();
        }

        public override void HandleAu2Frame(MaslAu2Frame frame)
        {
            throw new NotArObtainedAfterAu3Exception();
        }

        public override void HandleAu3Frame(MaslAu3Frame frame)
        {
            throw new NotArObtainedAfterAu3Exception();
        }

        public override void HandleArFrame(Frames.MaslArFrame arFrame)
        {
            var expectedDir = MaslFrameDirection.Server2Client;
            if (arFrame.Direction != expectedDir)
            {
                throw new DirectionFlagException(expectedDir);
            }

            // �յ�AR���ر����ּ�ʱ����
            this.Context.StopHandshakeTimer();

            // ��֤MAC
            var actualMac = arFrame.MAC;
            var expectedMac = this.Context.AuMessageMacCalculator.CalcArMAC(arFrame, this.Context.RsspEP.LocalID);

            if (!ArrayHelper.Equals(expectedMac, actualMac))
            {
                throw new MacInArException(string.Format("Ar��ϢMac����ʧ�ܣ�����ֵ={0}��ʵ��ֵ={1}",
                    HelperTools.ConvertToString(expectedMac),
                    HelperTools.ConvertToString(actualMac)));
            }

            this.Context.Connected = true;
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
