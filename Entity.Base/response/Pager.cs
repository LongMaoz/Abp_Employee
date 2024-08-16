
namespace Entity.Base
{
    /// <summary>
    /// 返回分页的信息的公共类
    /// </summary>
    /// 修改记录：
    public class Pager<T> where T : class
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 返回分页的实体集合
        /// </summary>
        public T rows { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="total"></param>
        /// <param name="rows"></param>
        public Pager(int total, T rows)
        {
            this.total = total;
            this.rows = rows;
        }
    }
    public class PagerItem<T> where T : class
    {
        /// <summary>
        /// 总数
        /// </summary>
        public long total { get; set; }

        /// <summary>
        /// 返回分页的实体集合
        /// </summary>
        public T rows { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="total"></param>
        /// <param name="rows"></param>
        public PagerItem(long total, T rows)
        {
            this.total = total;
            this.rows = rows;
        }
    }
    public class PagerItem<T,K> where T : class where K:class
    {
        /// <summary>
        /// 总数
        /// </summary>
        public long total { get; set; }

        /// <summary>
        /// 返回分页的实体集合
        /// </summary>
        public T rows { get; set; }
        /// <summary>
        /// 每行数据共有属性
        /// </summary>
        public K item { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="total"></param>
        /// <param name="rows"></param>
        /// <param name="item"></param>
        public PagerItem(long total, T rows,K item)
        {
            this.total = total;
            this.rows = rows;
            this.item = item;
        }
    }
}