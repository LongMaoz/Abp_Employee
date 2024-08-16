using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Config
{
    public class RedisKeyExpireConfig
    {//一天为单位计算

        private int _default = 30;
        private int _qiNiuToken = 1; //小时
        private int _checkin = 30;
        private int _checkout = 30;
        private int _msg = 20;
        private int _smsValid = 10; //mininute
        private int _tTSValid;
        private int store = 60;
        public int CheckIn { get => _checkin; set => _checkin = value; }
        public int CheckOut { get => _checkout; set => _checkout = value; }
        /// <summary>
        /// 订单消息存活时间 分钟
        /// </summary>
        public int Msg { get => _msg; set => _msg = value; }
        public int DefaultExpire { get => _default; set => _default = value; }
        public int QiNiuToken { get => _qiNiuToken; set => _qiNiuToken = value; }
        public int SmsValid { get => _smsValid; set => _smsValid = value; }
        public int TTSValid { get => _tTSValid; set => _tTSValid = value; }
        public int Store { get => store; set => store = value; }
    }
    public class MemcacheKeyExpireConfig
    {
        private int _requestFrenquceExt = 5;
        private int _expireExt = 3600;

        private int _requestFrenquceExtErp = 5;
        private int _expireExtErp = 3600;
        private bool _isLimit = true;
        private bool _isLimitErp = true;

        /// <summary>
        /// 同一个请求的频率限定
        /// </summary>
        public int RequestFrenquceExt { get => _requestFrenquceExt; set => _requestFrenquceExt = value; }
        /// <summary>
        /// 内存存储过期时间 秒
        /// </summary>
        public int ExpireExt { get => _expireExt; set => _expireExt = value; }
        public List<string> FilterRouter { get; set; }
        public bool IsLimit { get => _isLimit; set => _isLimit = value; }
        public List<string> FilterRequest { get; set; }

        public int RequestFrenquceExtErp { get => _requestFrenquceExtErp; set => _requestFrenquceExtErp = value; }
        /// <summary>
        /// 内存存储过期时间 秒
        /// </summary>
        public int ExpireExtErp { get => _expireExtErp; set => _expireExtErp = value; }
        public List<string> FilterRouterErp { get; set; }
        public bool IsLimitErp { get => _isLimitErp; set => _isLimitErp = value; }
        public List<string> FilterRequestErp { get; set; }
    }

    public class LockKeyExpire
    {
        public bool IsCommon { get; set; }
        public double CommonExpire { get; set; }
        public double OrderCreated { get; set; }
        public double OrderClosed { get; set; }
        public double OrderDateUpdate { get; set; }
        public double OrderAddressUpdate { get; set; }
        public double OrderRemarkUpdate { get; set; }
        public double OrderEquipmentInsert { get; set; }
        public double OrderEquipmentDel { get; set; }
        public double EquipmentReschedule { get; set; }
        public double OrderCompleted { get; set; }
    }
}
