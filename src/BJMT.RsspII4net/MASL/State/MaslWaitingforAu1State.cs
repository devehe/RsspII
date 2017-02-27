/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-12 21:03:03 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.Exceptions;
using BJMT.RsspII4net.MASL.Frames;

namespace BJMT.RsspII4net.MASL.State
{
    class MaslWaitingforAu1State : MaslState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public MaslWaitingforAu1State(IMaslStateContext context)
            :base(context)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        public override void HandleAu2Frame(MaslAu2Frame frame)
        {
            throw new MaslException(MaslErrorCode.SequenceIntegrityFailure);
        }

        public override void HandleAu3Frame(MaslAu3Frame frame)
        {
            throw new MaslException(MaslErrorCode.SequenceIntegrityFailure);
        }

        public override void HandleArFrame(MaslArFrame frame)
        {
            throw new MaslException(MaslErrorCode.SequenceIntegrityFailure);
        }

        public override void HandleAu1Frame(MaslAu1Frame au1Frame)
        {
            if (au1Frame.EncryAlgorithm != EncryptionAlgorithm.TripleDES)
            {
                throw new SafetyFeatureNotSupportedException();
            }

            var expectedDir = MaslFrameDirection.Client2Server;
            if (au1Frame.Direction != expectedDir)
            {
                throw new DirectionFlagException(expectedDir);
            }

            // ����Զ���豸�������������͡�
            this.Context.RsspEP.RemoteEquipType = au1Frame.DeviceType;

            // ����RandomB
            this.Context.MacCalculator.RandomB = au1Frame.RandomB;

            // ��ʼ��Mac��������
            this.Context.MacCalculator.UpdateSessionKeys();

            // ������һ��״̬��
            this.Context.CurrentState = new MaslWaitingforAu3State(this.Context);

            // �������ּ�ʱ�����ȴ�AU3
            this.Context.StartHandshakeTimer();
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
