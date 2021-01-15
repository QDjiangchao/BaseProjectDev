using System;
namespace qdlk.Controllers
{
    /// <summary>
    /// 返回对象
    /// </summary>
    public class ReturnModel
    {
        /// <summary>
        /// 状态码 0：成功  -1 ：失败
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 错误消息 正常为空
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 返回数据对象
        /// </summary>
        public Object Data { get; set; }

        public ReturnModel()
        {
            Code = 0;
        }
        public ReturnModel(string msg, Object data)
        {
            Code = string.IsNullOrWhiteSpace(msg) ? 0 : -1;
            Msg = msg;
            Data = data;
        }
    }
}
