/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-15 15:23:28 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using BJMT.RsspII4net.SAI.EC.Frames;

namespace BJMT.RsspII4net.SAI.EC.State
{
    /// <summary>
    /// ��ʾ����������EC�Ͽ�״̬��
    /// </summary>
    class EcDisconnectedState : EcState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public EcDisconnectedState(ISaiStateContext context, EcDefenseStrategy strategy)
            : base(context, strategy)
        {

        }
        #endregion

        #region "Properties"
        #endregion

        #region "Override methods"
        public override void HandleMaslConnected()
        {
            try
            {
                this.SendEcStartFrame();
                LogUtility.Info(string.Format("{0}: ����������EC Start1������Tsync��", this.Context.RsspEP.ID));

                // ����������EC Tsync��
                this.Context.StartHandshakeTimer();

                // ������һ��״̬��
                this.Context.CurrentState = new EcWaitingforStart2State(this);
            }
            catch (System.Exception ex)
            {
                LogUtility.Error(ex.ToString());
            }
        }

        public override void SendUserData(OutgoingPackage package)
        {
            
        }
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
