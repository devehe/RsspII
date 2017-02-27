/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-18 9:45:26 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using BJMT.RsspII4net.Exceptions;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.ALE.Frames
{
    /// <summary>
    /// ALE�������������������ֽ�������ΪALEЭ��֡��
    /// </summary>
    class AleStreamParser
    {
        /// <summary>
        /// ALE������������󳤶ȡ�
        /// </summary>
        public const ushort AleStreamMaxLength = 16 * 1024;

        #region "Filed"
        /// <summary>
        /// ���ݽ��ջ�����
        /// </summary>
        private byte[] _recvBuffer = new byte[AleStreamMaxLength]; 
        
        /// <summary>
        /// ���ջ���������Ч����
        /// </summary>
        private int _recvBufLen = 0;

        /// <summary>
        /// ָʾ�Ƿ��յ���ʼ��־
        /// </summary>
        private bool _startFlagRecved = false;

        /// <summary>
        /// �����յ������ݳ���
        /// </summary>
        private int _expectedLen = 0;
        #endregion

        #region "Constructor"
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"

        private ushort GetPacketLength(byte[] buffer, int startIndex)
        {
            if ((buffer == null) || (buffer.Length - startIndex < 2))
            {
                return 0;
            }

            return RsspEncoding.ToHostUInt16(buffer, startIndex);
        }
        #endregion

        #region "Public methods"
        /// <summary>
        /// ��λ���ջ��ơ�
        /// </summary>
        public void Reset()
        {
            _recvBuffer.Initialize();
            _recvBufLen = 0;
            _startFlagRecved = false;
            _expectedLen = 0;
        }

        /// <summary>
        /// ��TCP�ޱ߽��ֽ����з�����ALE����
        /// </summary>
        public List<byte[]> ParseTcpStream(byte[] tcpStream, int length)
        {
            var result = new List<byte[]>();

            try
            {
                for (int i = 0; i < length; i++)
                {
                    var data = tcpStream[i];

                    if (!_startFlagRecved)
                    {
                        var pktLen = this.GetPacketLength(tcpStream, i);
                        if (pktLen >= AleFrame.HeadLength - 2)
                        {
                            this.Reset();

                            // ���������ĳ��ȡ�
                            _expectedLen = pktLen + 2;
                            if (_expectedLen > _recvBuffer.Length)
                            {
                                throw new AleFrameParsingException(string.Format("ALE���������峤��({0})���㣬�����ĳ�����{1}��", 
                                    _recvBuffer.Length, _expectedLen));
                            }

                            _startFlagRecved = true;
                            _recvBuffer[_recvBufLen++] = data;
                        }
                    }
                    else
                    {
                        if (_expectedLen > _recvBufLen)
                        {
                            _recvBuffer[_recvBufLen++] = data;
                        }
                        
                        // �յ������ݳ�����������ֵһ��
                        if (_expectedLen == _recvBufLen)
                        {
                            // �����յ�������
                            var newAleStream = new byte[_recvBufLen];
                            Array.Copy(_recvBuffer, 0, newAleStream, 0, _recvBufLen);
                            result.Add(newAleStream);

                            // ��λ��
                            this.Reset();
                        }
                    }
                }
            }
            catch (Exception)
            {
                this.Reset();

                throw;
            }

            return result;
        }
        #endregion

    }
}
