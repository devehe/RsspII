/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-13 14:19:26 
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
using BJMT.RsspII4net.SAI.TTS.Frames;
using BJMT.RsspII4net.SAI.EC.Frames;

namespace BJMT.RsspII4net.SAI
{
    abstract class SaiState
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        protected SaiState(ISaiStateContext context)
        {
            LogUtility.Info(string.Format("{0}��Sai����״̬= {1}", context.RsspEP.ID, this.GetType().Name));

            this.Context = context;
        }
        #endregion

        #region "Properties"
        protected ISaiStateContext Context { get; private set; }
        #endregion

        #region "Virtual methods"
        public abstract void HandleMaslConnected();

        public abstract void SendUserData(OutgoingPackage package);

        protected virtual void HandleEcFrame(SaiEcFrame ecFrame)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }

        protected virtual void HandleTtsFrame(SaiTtsFrame ttsFrame)
        {
            LogUtility.Error(string.Format("{0}: {1}.{2} not implement!",
                this.Context.RsspEP.ID, this.GetType().Name,
                new StackFrame(0).GetMethod().Name.Split('.').Last()));
        }
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"

        public void HandleFrame(SaiFrame saiFrame)
        {
            if (SaiFrame.IsEcFrame(saiFrame.FrameType))
            {
                this.HandleEcFrame(saiFrame as SaiEcFrame);
            }
            else if (SaiFrame.IsTtsFrame(saiFrame.FrameType))
            {
                this.HandleTtsFrame(saiFrame as SaiTtsFrame);
            }
            else
            {
                throw new NotImplementedException("ָ����SaiFrame����ʶ��SaiState�޷�����");
            }
        }
        #endregion

    }
}
