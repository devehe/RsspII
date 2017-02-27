/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-19 15:40:19 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.SAI.EC.Frames
{
    abstract class SaiEcFrame : SaiFrame
    {
        #region "Filed"
        #endregion

        #region "Constructor"

        protected SaiEcFrame(SaiFrameType type, ushort seqNo)
            : base(type, seqNo)
        {
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// EC����
        /// </summary>
        public UInt32 EcValue { get; set; }
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
