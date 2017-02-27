/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 15:04:51 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BJMT.RsspII4net.ALE.Frames
{
    /// <summary>
    /// �ǻ��·�ϵ�������Ϣ
    /// </summary>
    class AleKeepAliveOnNonActiveLink : AleUserData
    {
        public override ushort Length
        {
            get { return 0; }
        }

        public AleKeepAliveOnNonActiveLink()
            : base(AleFrameType.KANA)
        {

        }

        public override byte[] GetBytes()
        {
            return null;
        }

        public override void ParseBytes(byte[] bytes, int startIndex, int endIndex)
        {
        }
    }
}
