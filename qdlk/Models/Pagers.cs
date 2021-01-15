using System;
namespace qdlk.Models
{
    /// <summary>
    /// 分类帮助类
    /// </summary>
    public class Pagers
    {
        public Pagers()
        {
        }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 数据记录
        /// </summary>
        public object Records { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        public int Totals { get; set; }

    }
}
