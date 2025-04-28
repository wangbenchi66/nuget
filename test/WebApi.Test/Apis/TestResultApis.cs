using Easy.Common.Core;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Test.Apis
{
    public class TestResultApis : BaseApi
    {

        #region 返回数据FluentResults

        public ApiResultPlus<string, ErrorInfo> GetResult()
        {
            return "手动成功";
        }

        [HttpGet("GetResultError")]
        public ApiResultPlus<string, ErrorInfo> GetResultError()
        {
            return ErrorInfo.Error("手动错误");
        }

        [HttpGet("GetResult")]
        public ApiResult GetResult(int type)
        {
            var result = new ApiResultPlus<string, ErrorInfo>();
            if (type == 1)
                result = "成功";
            else
                result = ErrorInfo.Error("错误");
            var res = result.Match(
                success =>
                {
                    // 处理成功情况
                    return ApiResult.Ok(success);
                },
                error =>
                {
                    // 处理错误情况
                    return ApiResult.Fail(error.Msg, error.Data);
                }
            );
            return res;
        }

        #endregion 返回数据FluentResults
    }
}