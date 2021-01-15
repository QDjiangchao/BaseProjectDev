using System;
namespace qdlk.Models
{
    /// <summary>
    /// 系统菜单表
    /// </summary>
    public class T_sys_menus
    {
        public Guid id { get; set; }
        /// <summary>
        /// 菜单编码
        /// </summary>
        public string cmenucode { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string cmenuname { get; set; }
        /// <summary>
        /// 父级编码
        /// </summary>
        public string cparentcode { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int isort { get; set; }
        /// <summary>
        /// 删除标示 0：有效 1：删除
        /// </summary>
        public int idel { get; set; }
    }
    public class MenuDto : T_sys_menus
    {
        /// <summary>
        /// 分页参数
        /// </summary>
        public Pagers pagers { get; set; }

    }
}
