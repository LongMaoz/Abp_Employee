using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
    public class PageViewModel : QueryViewModel
    {
        /// <summary>
        /// 查询主键
        /// </summary>
        public int? ID { get; set; }
        /// <summary>
        /// 创建记录人员ID
        /// </summary>
        public int? CreatorID { get; set; }
        private int _from = 0;
        /// <summary>
        /// 数据开始项 默认为0
        /// </summary>
        public int From
        {
            get
            {
                return this._from;
            }
            set
            {
                this._from = value;
            }
        }
        private int _size = 10;
        /// <summary>
        /// 每次返回的总数目  默认为10  当为-1时 不进行分页，一次性全部返回
        /// </summary>
        public int Size
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;
            }
        }

        private bool _desc = true;
        /// <summary>
        /// 是否倒叙排列  默认true 倒序(从大到小)  false正序  默认排序因子为创建时间
        /// </summary>
        public virtual bool Desc
        {
            get { return this._desc; }
            set { this._desc = value; }
        }
    }
}
