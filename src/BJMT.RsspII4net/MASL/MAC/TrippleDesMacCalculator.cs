/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-8 15:52:08 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace BJMT.RsspII4net.MASL
{
    /// <summary>
    /// 3DES-MAC��������
    /// </summary>
    class TrippleDesMacCalculator : IMacCalculator, IDisposable
    {
        #region "Field"
        /// <summary>  
        /// ������ʼֵ��
        /// </summary>  
        private static byte[] DesRgbIV = { 0, 0, 0, 0, 0, 0, 0, 0 };

        private bool _disposed = false;

        /// <summary>
        /// DES���ܷ���
        /// </summary>
        private DESCryptoServiceProvider _desService = new DESCryptoServiceProvider() { Padding = PaddingMode.Zeros };

        /// <summary>
        /// ��֤��Կ: ���������ӽ������������ɻỰ��Կ��
        /// Authentication keys: Session key derivation in connection establishment.
        /// </summary>
        private byte[] KeyMac1 { get; set; }
        private byte[] KeyMac2 { get; set; }
        private byte[] KeyMac3 { get; set; }


        /// <summary>
        /// �����A
        /// </summary>
        public byte[] RandomA { get; set; }
        /// <summary>
        /// �����B
        /// </summary>
        public byte[] RandomB { get; set; }

        /// <summary>
        /// �Ự��Կ: ������ȫʵ��֮������ݴ��䡣
        /// Key for Session: Protection of data transfer between safety entities.
        /// </summary>
        private byte[] KeySession1 { get; set; }
        private byte[] KeySession2 { get; set; }
        private byte[] KeySession3 { get; set; }

        /// <summary>
        /// �Ự��Կ��Ӧ��ת�����㡣
        /// </summary>
        private ICryptoTransform _transform1;
        private ICryptoTransform _transform2;
        private ICryptoTransform _transform3;
        #endregion

        #region "Constructor"
        /// <summary>
        /// ���캯����
        /// </summary>
        public TrippleDesMacCalculator(byte[] authKeys)
        {
            if (authKeys== null || authKeys.Length != 24)
            {
                throw new ArgumentException("��֤��Կ�ĳ��ȱ���Ϊ192λ(24�ֽ�)��");
            }

            // 
            var keys = this.SplitTo8BytesBlock(authKeys);
            this.KeyMac1 = keys[0];
            this.KeyMac2 = keys[1];
            this.KeyMac3 = keys[2];

            // 
            this.InitRandomNumber();
            this.UpdateSessionKeys();
        }

        /// <summary>
        /// �սắ����
        /// </summary>
        ~TrippleDesMacCalculator()
        {
            this.Dispose(false);
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transform1 != null)
                    {
                        _transform1.Dispose();
                        _transform1 = null;
                    }
                    if (_transform2 != null)
                    {
                        _transform2.Dispose();
                        _transform2 = null;
                    }
                    if (_transform3 != null)
                    {
                        _transform3.Dispose();
                        _transform3 = null;
                    }

                    if (_desService != null)
                    {
                        _desService.Dispose();
                        _desService = null;
                    }
                }

                _disposed = true;
            }
        }

        #region "private methods"
        /// <summary>  
        /// DES���ܡ�  
        /// </summary>  
        private byte[] DesEncrypt(byte[] key, byte[] data, ICryptoTransform transform)
        {
            using (var mStream = new MemoryStream())
            {
                using (var cStream = new CryptoStream(mStream, transform, CryptoStreamMode.Write))
                {
                    cStream.Write(data, 0, data.Length);
                    cStream.FlushFinalBlock();

                    return mStream.ToArray();
                }
            }
        }

        /// <summary>  
        /// DES���ܡ�
        /// </summary>  
        private byte[] DesDecrypt(byte[] key, byte[] data, ICryptoTransform transform)
        {
            using (var mStream = new MemoryStream())
            {
                using (var cStream = new CryptoStream(mStream, transform, CryptoStreamMode.Write))
                {
                    cStream.Write(data, 0, data.Length);
                    cStream.FlushFinalBlock();

                    return mStream.ToArray();
                }
            }
        }

        private byte[] CalcKeySession1()
        {
            using (var transform1 = _desService.CreateEncryptor(KeyMac1, DesRgbIV))
            {
                var data = RandomA.Take(4).Concat(RandomB.Take(4)).ToArray();
                var value1 = DesEncrypt(KeyMac1, data, transform1);

                using (var transform2 = _desService.CreateDecryptor(KeyMac2, DesRgbIV))
                {
                    var value2 = DesDecrypt(KeyMac2, value1, transform2);

                    using (var transform3 = _desService.CreateEncryptor(KeyMac3, DesRgbIV))
                    {
                        return DesEncrypt(KeyMac3, value2, transform3);
                    }
                }
            }
        }

        private byte[] CalcKeySession2()
        {
            using (var transform1 = _desService.CreateEncryptor(KeyMac1, DesRgbIV))
            {
                var data = RandomA.Skip(4).Concat(RandomB.Skip(4)).ToArray();
                var value1 = DesEncrypt(KeyMac1, data, transform1);

                using (var transform2 = _desService.CreateDecryptor(KeyMac2, DesRgbIV))
                {
                    var value2 = DesDecrypt(KeyMac2, value1, transform2);

                    using (var transform3 = _desService.CreateEncryptor(KeyMac3, DesRgbIV))
                    {
                        return DesEncrypt(KeyMac3, value2, transform3);
                    }
                }
            }
        }

        private byte[] CalcKeySession3()
        {
            using (var transform1 = _desService.CreateEncryptor(KeyMac3, DesRgbIV))
            {
                var data = RandomA.Take(4).Concat(RandomB.Take(4)).ToArray();
                var value1 = DesEncrypt(KeyMac3, data, transform1);

                using (var transform2 = _desService.CreateDecryptor(KeyMac2, DesRgbIV))
                {
                    var value2 = DesDecrypt(KeyMac2, value1, transform2);

                    using (var transform3 = _desService.CreateEncryptor(KeyMac1, DesRgbIV))
                    {
                        return DesEncrypt(KeyMac1, value2, transform3);
                    }
                }
            }
        }


        /// <summary>
        /// ��ָ��������ָ�Ϊ8�ֽڿ顣
        /// </summary>
        private List<byte[]> SplitTo8BytesBlock(byte[] value)
        {
            var result = new List<byte[]>();

            var remainder = value.Length % 8;
            var num = value.Length / 8;
            if (remainder != 0)
            {
                num++;
            }

            for (int i = 0; i < num; i++)
            {
                var block = new byte[8];

                if (i == num - 1)
                {
                    var len = value.Length - (i * 8);
                    Array.Copy(value, i * 8, block, 0, len);
                }
                else
                {
                    Array.Copy(value, i * 8, block, 0, 8);
                }

                result.Add(block);
            }

            return result;
        }

        private byte[] XorArray(byte[] array1, byte[] array2)
        {
            if (array2 == null || array2.Length == 0)
            {
                return array1;
            }

            if (array1 == null || array1.Length == 0)
            {
                return array2;
            }            

            var len = Math.Max(array1.Length, array2.Length);
            var result = new byte[len];

            for (int i = 0; i < result.Length; i++)
            {
                var x1 = array1.Length > i ? array1[i] : 0;
                var x2 = array2.Length > i ? array2[i] : 0;

                result[i] = (byte)(x1 ^ x2);
            }

            return result;
        }
        #endregion

        #region "public methods"

        /// <summary>
        /// ��ʼ���������
        /// </summary>
        public void InitRandomNumber()
        {
            var random = new Random(DateTime.Now.Millisecond);

            // ����������RandomB��
            this.RandomB = new byte[] { 0x1F, 0xD3, 0xA1, 0xCE, 0x7B, 0x87, 0xE9, 0xB0 };
            random.NextBytes(this.RandomB);

            // ����������RandomA��
            this.RandomA = new byte[] { 0x0F, 0x95, 0xEF, 0x4A, 0x66, 0x25, 0xA9, 0x0D };
            random.NextBytes(this.RandomA);
        }

        /// <summary>
        /// ���»Ự��Կ��
        /// </summary>
        public void UpdateSessionKeys()
        {
            this.KeySession1 = CalcKeySession1();
            _transform1 = _desService.CreateEncryptor(this.KeySession1, DesRgbIV);

            this.KeySession2 = CalcKeySession2();
            _transform2 = _desService.CreateDecryptor(this.KeySession2, DesRgbIV);

            this.KeySession3 = CalcKeySession3();
            _transform3 = _desService.CreateEncryptor(this.KeySession3, DesRgbIV);
        }

        /// <summary>
        /// ����ָ�����ݵ�MAC��
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>8�ֽڵ�MAC��</returns>
        public byte[] CalcMac(byte[] userData)
        {
            var blocks = this.SplitTo8BytesBlock(userData);

            byte[] mac = null;
            for (int i = 0; i < blocks.Count; i++)
            {
                var xorValue = this.XorArray(blocks.ElementAt(i), mac);
                mac = this.DesEncrypt(this.KeySession1, xorValue, _transform1);
            }

            mac = this.DesDecrypt(this.KeySession2, mac, _transform2);

            mac = this.DesEncrypt(this.KeySession3, mac, _transform3);

            return mac;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
