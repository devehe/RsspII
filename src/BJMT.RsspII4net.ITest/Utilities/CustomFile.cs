/*----------------------------------------------------------------
// Copyright (C) 2010 ��������΢���Ƽ����޹�˾ ��Ȩ����
// 
// �����ˣ��ź�
//
//----------------------------------------------------------------*/
using System;
using System.IO;
using System.Reflection;
using BJMT.RsspII4net.Utilities;

namespace BJMT.RsspII4net.ITest
{
    class CustomFile : IDisposable
    {
        public const string StartFlag = "RSSP-II�ļ���ʼ��־";
        public const string EndFlag = "RSSP-II�ļ�������־";

        public readonly static int StartFlagLen = RsspEncoding.ToNetString(StartFlag).Length;
        public readonly static int EndFlagLen = RsspEncoding.ToNetString(EndFlag).Length;

        #region "Filed"
        private bool _disposed = false;
        private string _folder = "";      
        private FileStream _fileStream = null;
        private BinaryWriter _binWriter = null;

        #endregion

        #region "Constructor"
        public CustomFile(string deviceID, string fileExetension)
        {
            _folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\ʹ��RSSP-II������ļ�\\";

            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }

            this.Create(deviceID, fileExetension);
        }

        ~CustomFile()
        {
            this.Dispose(false);
        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    if (_binWriter != null)
                    {
                        _binWriter.Close();
                        _fileStream.Close();
                        _binWriter = null;
                    }
                }
            }
        }
        #endregion

        #region "Override methods"
        #endregion

        #region "Private methods"
        private void Create(string deviceID, string fileExetension)
        {
            string path = String.Format("{0}{1}\\", _folder, deviceID);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var filePath = path + Guid.NewGuid().ToString() + fileExetension;

            _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);

            _binWriter = new BinaryWriter(_fileStream);
        }
        #endregion

        #region "Public methods"

        public void Write(byte[] data)
        {
            if (_binWriter != null)
            {
                _binWriter.Write(data);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        public static bool ContainsFileStartFlag(byte[] data, int dataCount)
        {
            // 20��һ������ֵ�����ڷ�ֹ������û������а����ļ���ʼ��־��

            return dataCount >= CustomFile.StartFlagLen 
                && dataCount < (CustomFile.StartFlagLen + 20)
                && CustomFile.StartFlag == RsspEncoding.ToHostString(data, 0, CustomFile.StartFlagLen);
        }

        public static bool ContainsFileEndFlag(byte[] data, int dataCount)
        {
            return dataCount == CustomFile.EndFlagLen &&
                                CustomFile.EndFlag == RsspEncoding.ToHostString(data, 0, CustomFile.EndFlagLen);
        }
    }
}
