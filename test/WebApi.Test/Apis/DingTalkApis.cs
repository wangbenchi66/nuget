using System.Threading.Tasks;
using Easy.Common.Core.Extensions;

namespace WebApi.Test.Apis
{
    public class DingTalkApis : BaseApi
    {
        private DingTalkExtensions _dingTalkExtend = new DingTalkExtensions("src", "accessToken");

        public async Task Get()
        {
            await _dingTalkExtend.SendTextMsgAsync("测试消息");
        }
    }
}