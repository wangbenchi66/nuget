using Easy.DynamicApi;
using Easy.DynamicApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Test.Apis
{
    [DynamicApi]//动态api标识
    [ApiExplorerSettings(GroupName = "v1")]//接口分组
    public class BaseApi : IDynamicApi//动态api接口
    {
    }
}