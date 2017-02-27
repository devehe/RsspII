/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-11-17 11:08:52 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace BJMT.RsspII4net.Utilities
{
    /// <summary>
    /// ����ϵͳ
    /// </summary>
    public static class RsspEncoding
    {
        #region "Filed"
        /// <summary>
        /// ͨѶЭ���д����ַ���ʱʹ�õı��뷽��
        /// </summary>
        private readonly static Encoding _charsEncoding = Encoding.UTF8;
        #endregion

        #region "Public methods"
        /// <summary>
        /// �������ַ������л�Ϊ�����ϴ���ĸ�ʽ��
        /// </summary>
        /// <param name="hostString">����Ҫ������ַ��� System.String��</param>
        /// <param name="length">���������鳤�ȣ�-1��ʾ������</param>
        /// <returns>һ���ֽ����飬������ָ�����ַ������б���Ľ����</returns>
        public static byte[] ToNetString(string hostString, int length = -1)
        {
            var bytes = _charsEncoding.GetBytes(hostString);
            if (length == -1 || bytes.Length == length)
            {
                return bytes;
            }
            else if (bytes.Length < length)
            {
                var stream = new byte[length];
                bytes.CopyTo(stream, 0);
                return stream;
            }
            else
            {
                return bytes.Take(length).ToArray();
            }
        }
        /// <summary>
        /// ��ͨѶ�����ϴ�����ַ�������Ϊ�����ַ�����
        /// </summary>
        /// <param name="netBytes">ͨѶЭ���е��ֽ���</param>
        /// <param name="index">��һ��Ҫ������ֽڵ�����</param>
        /// <param name="len">Ҫ������ֽ���</param>
        /// <returns>һ�������ַ���</returns>
        public static string ToHostString(byte[] netBytes, int index, int len)
        {
            if (netBytes == null || netBytes.Length == 0) return string.Empty;

            if ((len + index) > netBytes.Length)
            {
                throw new ArgumentException("ָ���Ľ��볤����Ч��");
            }

            var result = _charsEncoding.GetString(netBytes, index, len);
            return result.Trim('\0');
        }

        /// <summary>
        /// ��Int16ֵ���л�Ϊ�����ϴ���ĸ�ʽ��
        /// </summary>
        /// <param name="hostValue">�������ֽ�˳���ʾ��Ҫת��������</param>
        /// <returns>�������ֽ�˳���ʾ�Ķ�ֵ</returns>
        public static byte[] ToNetInt16(Int16 hostValue)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(hostValue));
        }
        /// <summary>
        /// �������ϴ����Int16���ݽ���Ϊ����ʹ�õ���ֵ��
        /// </summary>
        /// <param name="value">ͨѶЭ���е��ֽ�����</param>
        /// <param name="startIndex">value �ڵ���ʼλ�á�</param>
        /// <returns>�������ֽ�˳���ʾ�Ķ�ֵ</returns>
        public static short ToHostInt16(byte[] value, int startIndex)
        {
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt16(value, startIndex));
        }

        /// <summary>
        /// ���޷��Ŷ�ֵ�������ֽ�˳��ת��ΪͨѶЭ���ֽ�˳��
        /// </summary>
        /// <param name="host">�������ֽ�˳���ʾ��Ҫת��������</param>
        /// <returns>�������ֽ�˳���ʾ�Ķ�ֵ</returns>
        public static byte[] ToNetUInt16(UInt16 host)
        {
            return ToNetInt16((Int16)host);
        }
        /// <summary>
        /// ��ͨѶЭ���ֽ���ת��������˳���ʾ�Ķ�ֵ
        /// </summary>
        /// <param name="value">ͨѶЭ���е��ֽ�����</param>
        /// <param name="startIndex">value �ڵ���ʼλ�á�</param>
        /// <returns>�������ֽ�˳���ʾ�Ķ�ֵ</returns>
        public static UInt16 ToHostUInt16(byte[] value, int startIndex)
        {
            return (UInt16)ToHostInt16(value, startIndex);
        }


        /// <summary>
        /// ������ֵ�������ֽ�˳��ת��ΪͨѶЭ���ֽ�˳��
        /// </summary>
        /// <param name="host">�������ֽ�˳���ʾ��Ҫת��������</param>
        /// <returns>�������ֽ�˳���ʾ������ֵ</returns>
        public static byte[] ToNetInt32(int host)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(host));
        }
        /// <summary>
        /// ��ͨѶЭ���ֽ���ת��������˳���ʾ������ֵ
        /// </summary>
        /// <param name="value">ͨѶЭ���е��ֽ�����</param>
        /// <param name="startIndex">value �ڵ���ʼλ�á�</param>
        /// <returns>�������ֽ�˳���ʾ������ֵ</returns>
        public static int ToHostInt32(byte[] value, int startIndex)
        {
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(value, startIndex));
        }

        /// <summary>
        /// ������ֵ�������ֽ�˳��ת��ΪͨѶЭ���ֽ�˳��
        /// </summary>
        /// <param name="host">�������ֽ�˳���ʾ��Ҫת��������</param>
        /// <returns>�������ֽ�˳���ʾ������ֵ</returns>
        public static byte[] ToNetUInt32(UInt32 host)
        {
            return ToNetInt32((Int32)host);
        }
        /// <summary>
        /// ��ͨѶЭ���ֽ���ת��������˳���ʾ������ֵ
        /// </summary>
        /// <param name="value">ͨѶЭ���е��ֽ�����</param>
        /// <param name="startIndex">value �ڵ���ʼλ�á�</param>
        /// <returns>�������ֽ�˳���ʾ������ֵ</returns>
        public static UInt32 ToHostUInt32(byte[] value, int startIndex)
        {
            return (UInt32)ToHostInt32(value, startIndex);
        }

        /// <summary>
        /// ����ֵ�������ֽ�˳��ת��ΪͨѶЭ���ֽ�˳��
        /// </summary>
        /// <param name="host">�������ֽ�˳���ʾ��Ҫת��������</param>
        /// <returns>�������ֽ�˳���ʾ�ĳ�ֵ</returns>
        public static byte[] ToNetInt64(Int64 host)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(host));
        }
        /// <summary>
        /// ��ͨѶЭ���ֽ���ת��������˳���ʾ�ĳ�ֵ
        /// </summary>
        /// <param name="value">ͨѶЭ���е��ֽ�����</param>
        /// <param name="startIndex">value �ڵ���ʼλ�á�</param>
        /// <returns>�������ֽ�˳���ʾ�ĳ�ֵ</returns>
        public static long ToHostInt64(byte[] value, int startIndex)
        {
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(value, startIndex));
        }

        /// <summary>
        /// ����ֵ�������ֽ�˳��ת��ΪͨѶЭ���ֽ�˳��
        /// </summary>
        /// <param name="host">�������ֽ�˳���ʾ��Ҫת��������</param>
        /// <returns>�������ֽ�˳���ʾ�ĳ�ֵ</returns>
        public static byte[] ToNetUInt64(UInt64 host)
        {
            return ToNetInt64((Int64)host);
        }
        /// <summary>
        /// ��ͨѶЭ���ֽ���ת��������˳���ʾ�ĳ�ֵ
        /// </summary>
        /// <param name="value">ͨѶЭ���е��ֽ�����</param>
        /// <param name="startIndex">value �ڵ���ʼλ�á�</param>
        /// <returns>�������ֽ�˳���ʾ�ĳ�ֵ</returns>
        public static UInt64 ToHostUInt64(byte[] value, int startIndex)
        {
            return (UInt64)ToHostInt64(value, startIndex);
        }

        #endregion

    }
}
