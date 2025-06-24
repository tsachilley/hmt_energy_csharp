using System.Collections.Generic;

namespace hmt_energy_csharp.ResponseResults
{
    public class ResponseResult
    {
        //是否成功执行
        public bool IsSuccess { get; set; } = true;

        //返回内容
        public object ResultObject { get; set; }

        //需要保存的日志内容
        public IList<LogBook> LogContents { get; set; } = new List<LogBook>();

        //需要保存的日志内容
        public IList<LogBook> LogContentsEn { get; set; } = new List<LogBook>();

        //失败错误信息
        public string ErrorMessage { get; set; }
    }

    public class LogBook
    {
        public string type { get; set; }
        public string content { get; set; }
        public string time { get; set; }
    }
}