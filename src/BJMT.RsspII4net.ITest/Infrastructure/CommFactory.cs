/*----------------------------------------------------------------
// ��˾���ƣ���������΢���Ƽ����޹�˾
// 
// ��Ŀ���ƣ�BPL
//
// �� �� �ˣ�zhangheng
// �������ڣ�11/25/2016 15:07:17 
// ��    �䣺zhangheng@bjmut.com
//
// Copyright (C) ��������΢���Ƽ����޹�˾����������Ȩ��
//
//----------------------------------------------------------------*/

using BJMT.RsspII4net.Config;

namespace BJMT.RsspII4net.ITest.Infrastructure
{
    abstract class CommFactory
    {
        public abstract IRsspNode Create();
    }

    class ClientCommFactory : CommFactory
    {
        private IClientConfigProvider _settings;

        public ClientCommFactory(IClientConfigProvider setting)
        {
            _settings = setting;
        }

        public override IRsspNode Create()
        {
            var linkCfg = _settings.GetLinkConfig();
            var deviceType = (byte)_settings.DeviceType;
            var defenseTech = _settings.DefenseTech;
            var appType = _settings.ApplicationType;

            var cfg = new RsspClientConfig(_settings.LocalID, deviceType, appType, defenseTech, linkCfg);
            cfg.AuthenticationKeys = _settings.GetAuthenticationKeys();
            cfg.EcInterval = _settings.EcInterval;
            cfg.SeqNoThreshold = _settings.SeqNoThreshold;

            return RsspFactory.CreateClientNode(cfg);
        }
    }

    class ServerCommFactory : CommFactory
    {
        private IServerConfigProvider _settings;

        public ServerCommFactory(IServerConfigProvider setting)
        {
            _settings = setting;
        }

        public override IRsspNode Create()
        {
            var listeners = _settings.GetListeningEndPoints();
            var deviceType = (byte)_settings.DeviceType;
            var appType = _settings.ApplicationType;
            var acceptableClients = _settings.GetAcceptableClients();

            var cfg = new RsspServerConfig(_settings.LocalID, deviceType, appType, listeners, acceptableClients);
            cfg.AuthenticationKeys = _settings.GetAuthenticationKeys();
            cfg.EcInterval = _settings.EcInterval;
            cfg.SeqNoThreshold = _settings.SeqNoThreshold;

            return RsspFactory.CreateServerNode(cfg);
        }
    }
}
