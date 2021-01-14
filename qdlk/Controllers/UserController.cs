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
                    return new ReturnModel("", conn.Query<T_user>("select * from t_user").ToList());
                }
                catch (Exception ex)
                {
                    return new ReturnModel(ex.Message, null);
                }
            }
        }

        [HttpPost]
        public ActionResult<ReturnModel> AddUser([FromBody] T_user user)
        {
            user.cpwd = CommBll.DESEncrypt(user.cpwd, Configuration.GetSection("DESKeys").Value);

            using (IDbConnection conn = new SqlConnection(Configuration.GetSection("ConnectionStrings:SQLConnectionString").Value))
            {
                try
                {
                    conn.Execute("insert into T_user(cusercode, cusername, cpwd, cmemo, isex, idel) values(@cusercode, @cusername, @cpwd, @cmemo, @isex, @idel)", user);
                    return new ReturnModel();
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
    }
}
