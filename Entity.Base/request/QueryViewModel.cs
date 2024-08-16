using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    public class QueryViewModel
    {

        private DateTime? _endTime;
        private bool? _unDel = true;
        private string keyWord;

        /// <summary>
        /// 未被删除 默认为true
        /// </summary>
        public bool? UnDel
        {
            get
            {
                return this._unDel;
            }
            set
            {
                this._unDel = value;
            }
        }
        /// <summary>
        /// 关键词  可不填
        /// </summary>
        public string KeyWord { get => !string.IsNullOrEmpty(keyWord) ? keyWord.TrimStart().TrimEnd() : keyWord; set => keyWord = value; }
        /// <summary>
        /// 开始时间 可不填 创建时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束时间 可不填 创建时间
        /// </summary>
        public DateTime? EndTime { get => _endTime; set => _endTime = value != null ? value.Value.AddDays(1) : value; }
    }

    public class QueryViewByUserModel : QueryViewModel
    {
        public List<int> LstUsers { get; set; }
    }
}
