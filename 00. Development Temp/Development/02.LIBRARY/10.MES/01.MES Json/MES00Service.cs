using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Development
{
    class MES00Service
    {
        private MES00SendPCB MESSend;
        private SemaphoreSlim modbusSemaphore = new SemaphoreSlim(1, 1);
        public bool isAccept { get; set; }
        //public string ReceivedLog;


        public MES00Service(TCPSetting tcpSetting)
        {
            this.MESSend = new MES00SendPCB(tcpSetting.Ip, tcpSetting.Port);
        }

        public async Task<Mes00Check> SendConfig(Mes00Check entity , string DeviceID ,string Recipe)
        {

            await modbusSemaphore.WaitAsync();
            try
            {
                if (entity.EquipmentId.Length != 9)
                {
                    entity.EquipmentId = entity.EquipmentId.PadRight(9, ' ');
                }
                if (entity.DIV.Length != 14)
                {
                    entity.DIV = entity.DIV.PadRight(14, ' ');
                }
                if (entity.CheckSum.Length != 14)
                {
                    entity.CheckSum = entity.CheckSum.PadRight(14, ' ');
                }
                return await this.MESSend.SendConfig(entity, DeviceID, Recipe, "");
            }
            finally
            {
                modbusSemaphore.Release();
            }
                
        }
        public async Task<Mes00Check> SendLogIn(Mes00Check entity)
        {
            await modbusSemaphore.WaitAsync();
            try
            {
                if (entity.EquipmentId.Length != 9)
                {
                    entity.EquipmentId = entity.EquipmentId.PadRight(9, ' ');
                }
                if (entity.DIV.Length != 14)
                {
                    entity.DIV = entity.DIV.PadRight(14, ' ');
                }
                return await this.MESSend.SendLogin(entity, "");
            }
            finally
            {
                modbusSemaphore.Release();
            }

        }
        public async Task<Mes00Check> SendWorkin(Mes00Check entity, string CH = "")
        {
            await modbusSemaphore.WaitAsync();
            try
            {

                if (entity.EquipmentId.Length != 9)
                {
                    entity.EquipmentId = entity.EquipmentId.PadRight(9, ' ');
                }
                if (entity.DIV.Length != 14)
                {
                    entity.DIV = entity.DIV.PadRight(14, ' ');
                }
                if (entity.CheckSum.Length != 14)
                {
                    entity.CheckSum = entity.CheckSum.PadRight(14, ' ');
                }
                return await this.MESSend.SendWorkin(entity, CH);
            }
            finally
            {
                modbusSemaphore.Release();
            }
        }
        public async Task<Mes00Check> SendWorkout(Mes00Check entity, string CH = "")
        {
            await modbusSemaphore.WaitAsync();
            try
            {

                if (entity.EquipmentId.Length != 9)
                {
                    entity.EquipmentId = entity.EquipmentId.PadRight(9, ' ');
                }
                if (entity.DIV.Length != 14)
                {
                    entity.DIV = entity.DIV.PadRight(14, ' ');
                }
                if (entity.CheckSum.Length != 14)
                {
                    entity.CheckSum = entity.CheckSum.PadRight(14, ' ');
                }
                return await this.MESSend.SendWorkout(entity, CH);
            }
            finally
            {
                modbusSemaphore.Release();
            }
        }
        public async Task<bool> SendReady(Mes00Check entity, string CH = "")
        {
            await modbusSemaphore.WaitAsync();
            try
            {
                if (entity.EquipmentId.Length != 9)
                {
                    entity.EquipmentId = entity.EquipmentId.PadRight(9, ' ');
                }
                return await this.MESSend.SendReady(entity, CH);
            }
            finally
            {
                modbusSemaphore.Release();
            }
        }
        public async Task<Mes00Check> SendPCB(Mes00Check entity, string CH)
        {
            await modbusSemaphore.WaitAsync();
            try
            {

                if (entity.EquipmentId.Length != 9)
                {
                    entity.EquipmentId = entity.EquipmentId.PadRight(9, ' ');
                }
                if (entity.DIV.Length != 14)
                {
                    entity.DIV = entity.DIV.PadRight(14, ' ');
                }
                if (entity.CheckSum.Length != 14)
                {
                    entity.CheckSum = entity.CheckSum.PadRight(14, ' ');
                }
                return await this.MESSend.SendPCB(entity, CH);
            }
            finally
            {
                modbusSemaphore.Release();
            }
        }
        public async Task Start()
        {
            await this.MESSend.Start();
            this.isAccept = this.MESSend.isAccept;
        }
        public void Stop()
        {
            this.MESSend.Stop();
        }
        public bool CheckConnect(out string Ipconnect, out string PortConnect)
        {
           bool Connect = MESSend.CheckConnect(out string IP,out string PORT);
           if(Connect)
           {
                Ipconnect = IP;
                PortConnect = PORT;
                return true;
           }    
           else
           {
                Ipconnect = "No IP";
                PortConnect = "No Port";
                return false;
           }   
        }
    }
}
