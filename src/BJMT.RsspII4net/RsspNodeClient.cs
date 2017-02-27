/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BJMT Platform Library
//
// �� �� �ˣ�zhangheng
// �������ڣ�2016-12-26 15:47:19 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ����
//
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BJMT.RsspII4net.Config;
using BJMT.RsspII4net.Events;
using BJMT.RsspII4net.SAI;

namespace BJMT.RsspII4net
{
    /// <summary>
    /// ����RSSP-IIͨѶЭ��Ŀͻ��˽ڵ㶨�塣
    /// </summary>
    class RsspNodeClient : RsspNode,
        IRsspNode
    {
        #region "Filed"
        private bool _disposed = false;
        private RsspClientConfig _rsspConfig;

        /// <summary>
        /// Key = SaiConnection ID.
        /// </summary>
        private Dictionary<string, SaiConnectionClient> _saiConnections = new Dictionary<string, SaiConnectionClient>();
        private object _saiConnectionsLock = new object();
        #endregion

        #region "Constructor"
        /// <summary>
        /// ����һ��������ʹ�õ�ALE��������
        /// </summary>
        public RsspNodeClient(RsspClientConfig config)
            : base(config)
        {
            _rsspConfig = config;

            // 
            config.LinkInfo.ToList().ForEach(p =>
            {
                var key = this.BuildSaiConnectionID(config.LocalID, p.Key);

                var rsspEP = new RsspEndPoint(config.LocalID, p.Key,
                    _rsspConfig.ApplicationType, _rsspConfig.LocalEquipType,
                    true, config.SeqNoThreshold,
                    config.EcInterval, _rsspConfig.AuthenticationKeys, null)
                {
                    DefenseTech = config.DefenseTech,
                    ServiceType = config.ServiceType,
                    Algorithm = config.Algorithm,
                    LocalEquipType = config.LocalEquipType,
                };

                var value = new SaiConnectionClient(rsspEP, p.Value, this, this);
                this.AddSaiConnection(key, value);
            });
        }
        #endregion

        #region "Properties"
        #endregion

        #region "Virtual methods"
        #endregion

        #region "Override methods"
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    lock (_saiConnectionsLock)
                    {
                        _saiConnections.ToList().ForEach(p => p.Value.Dispose());
                        _saiConnections.Clear();
                    }
                }

                base.Dispose(disposing);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(200);

            // ��ȫ���ӡ�
            sb.AppendFormat("Saiͨ������={0}��\r\n", _saiConnections.Count);
            var index = 1;
            _saiConnections.Values.ToList().ForEach(p =>
            {
                sb.AppendFormat("\r\n\r\n��{0}����{1}", index++, p);
            });

            sb.AppendFormat("\r\n");

            return sb.ToString();
        }

        protected override void OnOpen()
        {
            lock (_saiConnectionsLock)
            {
                _saiConnections.ToList().ForEach(p =>
                {
                    p.Value.Open();
                });
            }
        }

        protected override SaiConnection GetSaiConnection(string key)
        {
            lock (_saiConnectionsLock)
            {
                SaiConnectionClient theConnection;

                _saiConnections.TryGetValue(key, out theConnection);

                return theConnection;
            }
        }
        #endregion

        #region "Private methods"

        private void AddSaiConnection(string key, SaiConnectionClient value)
        {
            lock (_saiConnectionsLock)
            {
                _saiConnections.Add(key, value);
            }
        }
        #endregion

        #region "Public methods"

        public List<uint> GetConnectedNodeID()
        {
            lock (_saiConnectionsLock)
            {
                return _saiConnections.Where(p => p.Value.Connected).
                    Select(p => p.Value.RemoteID).ToList();
            }
        }
        #endregion

    }
}
