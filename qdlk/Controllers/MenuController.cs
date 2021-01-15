using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using qdlk.CommonBussiness;
using qdlk.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace qdlk.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MenuController : Controller
    {
        #region 声明全局
        private readonly IConfiguration Configuration;

        public MenuController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion 
        /// <summary>
        /// 获取全部菜单list
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ReturnModel> GetMenuList([FromBody] T_sys_menus menus)
        {
            using (IDbConnection conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SQLConnectionString").Value))
            {
                try
                {
                    string strWhere = "";
                    #region 拼接查询条件 
                    if (!string.IsNullOrWhiteSpace(menus.cmenucode))
                    {
                        strWhere += " and  cmenucode ='" + menus.cmenucode + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(menus.cmenuname))
                    {
                        strWhere += " and  cmenuname like'%" + menus.cmenuname + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(menus.idel.ToString()))
                    {
                        strWhere += " and  idel ='" + menus.idel + "'";
                    }
                    #endregion

                    return new ReturnModel("", conn.Query<T_sys_menus>(string.Format(" select * from T_sys_menus where {0}", strWhere)).ToList());
                }
                catch (Exception ex)
                {
                    return new ReturnModel(ex.Message, null);
                }
            }
        }
        /// <summary>
        /// 获取菜单分页数据
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ReturnModel> GetMenuListByPager([FromBody] MenuDto menus)
        {
            //读取appsettig配置
            //var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            //var configuration = builder.Build(); 

            using (IDbConnection conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SQLConnectionString").Value))
            {
                try
                {
                    string strWhere = "";
                    #region 拼接查询条件 
                    if (!string.IsNullOrWhiteSpace(menus.cmenucode))
                    {
                        strWhere += " and  cmenucode ='" + menus.cmenucode + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(menus.cmenuname))
                    {
                        strWhere += " and  cmenuname like'%" + menus.cmenuname + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(menus.idel.ToString()))
                    {
                        strWhere += " and  idel ='" + menus.idel + "'";
                    }
                    #endregion 

                    var dblist = conn.Query<T_sys_menus>(string.Format(" select * from T_sys_menus where {0}", strWhere)).ToList();

                    if (dblist.Count > 0)
                    {
                        menus.pagers.Records = dblist.Skip((menus.pagers.PageIndex - 1) * menus.pagers.PageSize).Take(menus.pagers.PageSize).ToList();
                        menus.pagers.Totals = dblist.Count;
                    }

                    return new ReturnModel("", menus.pagers);
                }
                catch (Exception ex)
                {
                    return new ReturnModel(ex.Message, null);
                }
            }
        }
        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ReturnModel> AddMenu([FromBody] T_sys_menus menus)
        {
            using (IDbConnection conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SQLConnectionString").Value))
            {
                try
                {
                    if (!exits(menus))
                    {
                        conn.Execute(@"insert into T_sys_menus
                                                        ( cmenucode, cmenuname, cparentcode, isort, idel)
                                                    values (@cmenucode, @cmenuname, @cparentcode, @isort, @idel)", menus);
                        return new ReturnModel();
                    }
                    else
                    {
                        return new ReturnModel(menus.cmenucode + " 菜单编码重复,请确认", null);
                    }

                }
                catch (Exception ex)
                {
                    return new ReturnModel(ex.Message, null);
                }
            }
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        [HttpPost]
        public bool exits(T_sys_menus menus)
        {
            using (IDbConnection conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SQLConnectionString").Value))
            {
                return conn.Query<int>(" select * from T_sys_menus where cmenucode=@cmenucode", menus).FirstOrDefault() > 0 ? true : false;
            }
        }
    }
}
