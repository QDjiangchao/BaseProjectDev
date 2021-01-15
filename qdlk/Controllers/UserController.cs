using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using qdlk.CommonBussiness;
using qdlk.Models;


namespace qdlk.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {

        #region 声明全局
        private readonly IConfiguration Configuration;

        public UserController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ReturnModel> GetUserList([FromBody] T_user user)
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
                    if (!string.IsNullOrWhiteSpace(user.cusercode))
                    {
                        strWhere += " and  cusercode ='" + user.cusercode + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(user.cusername))
                    {
                        strWhere += " and  cusername like'%" + user.cusername + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(user.idel.ToString()))
                    {
                        strWhere += " and  idel ='" + user.idel + "'";
                    }
                    #endregion

                    return new ReturnModel("", conn.Query<T_user>(string.Format(" select * from t_user where {0}", strWhere)).ToList());
                }
                catch (Exception ex)
                {
                    return new ReturnModel(ex.Message, null);
                }
            }
        }
        /// <summary>
        /// 获取用户信息分页
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ReturnModel> GetUserListByPager([FromBody] UserDto user)
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
                    if (!string.IsNullOrWhiteSpace(user.cusercode))
                    {
                        strWhere += " and  cusercode ='" + user.cusercode + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(user.cusername))
                    {
                        strWhere += " and  cusername like'%" + user.cusername + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(user.idel.ToString()))
                    {
                        strWhere += " and  idel ='" + user.idel + "'";
                    }
                    #endregion

                    var dblist = conn.Query<T_user>(string.Format(" select * from t_user where {0}", strWhere)).ToList();

                    if (dblist.Count > 0)
                    {
                        user.pagers.Records = dblist.Skip((user.pagers.PageIndex - 1) * user.pagers.PageSize).Take(user.pagers.PageSize).ToList();
                        user.pagers.Totals = dblist.Count;
                    }

                    return new ReturnModel("", user.pagers);
                }
                catch (Exception ex)
                {
                    return new ReturnModel(ex.Message, null);
                }
            }
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ReturnModel> AddUser([FromBody] T_user user)
        {
            using (IDbConnection conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SQLConnectionString").Value))
            {
                try
                {
                    if (!exits(user))
                    {
                        user.cpwd = CommBll.DESEncrypt(user.cpwd, Configuration.GetSection("DESKeys").Value);
                        conn.Execute("insert into T_user(cusercode, cusername, cpwd, cmemo, isex, idel) values(@cusercode, @cusername, @cpwd, @cmemo, @isex, @idel)", user);
                        //TODO:用户角色更新
                        //TODO:用户权限更新
                        return new ReturnModel();
                    }
                    else
                    {
                        return new ReturnModel(user.cusercode + " 用户编码重复,请确认", null);
                    }

                }
                catch (Exception ex)
                {
                    return new ReturnModel(ex.Message, null);
                }
            }
        }

        /// <summary>
        /// 用户登陆
        /// 用户名 密码 必传
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ReturnModel> Lgion([FromBody] T_user user)
        {
            string dbpwd = CommBll.DESEncrypt(user.cpwd, Configuration.GetSection("DESKeys").Value);

            using (IDbConnection conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SQLConnectionString").Value))
            {
                try
                {
                    var dbuser = conn.Query<T_user>("select * from T_user where cusercode =@cusercode and idel=0 ", user).FirstOrDefault();
                    if (!dbpwd.Equals(dbuser.cpwd))
                        return new ReturnModel("用户名或密码错误", dbuser);
                    else
                        return new ReturnModel();
                }
                catch (Exception ex)
                {
                    return new ReturnModel(ex.Message, null);
                }
            }
        }


        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ReturnModel> EditUser([FromBody] T_user user)
        {
            using (IDbConnection conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SQLConnectionString").Value))
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(user.cpwd))
                    {
                        user.cpwd = CommBll.DESEncrypt(user.cpwd, Configuration.GetSection("DESKeys").Value);
                    }
                    else
                    {
                        var dbuser = conn.Query<T_user>(" select * from t_user where cusercode=@cusercode", user).FirstOrDefault();
                        user.cpwd = dbuser.cpwd;
                    }
                    conn.Execute(@"update T_user
                                    set cusername  =@cusername,
                                        cpwd=@cpwd,
                                        cmemo=@cmemo,
                                        isex=@isex,
                                        idel=@idel,
                                        dupdatedate=getdate()
                                    where cusercode = @cusercode", user);

                    //TODO:用户角色更新
                    //TODO:用户权限更新
                    return new ReturnModel();
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
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public bool exits(T_user user)
        {
            using (IDbConnection conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SQLConnectionString").Value))
            {
                return conn.Query<int>(" select * from t_user where cusercode=@cusercode", user).FirstOrDefault() > 0 ? true : false;
            }
        }

    }
}
