/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-27 10:39:51 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.SAI.Events
{
    class SaiFrameIncomingEventArgs : EventArgs
    {
        #region "Filed"
        #endregion

        #region "Constructor"
        public SaiFrameIncomingEventArgs(SaiFrame frame)
        {
            this.Frame = frame;
        }
        #endregion

        #region "Properties"
        public SaiFrame Frame { get; private set; }
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        #endregion

        #region "Public methods"
        #endregion

    }
}
