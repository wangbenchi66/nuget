namespace Easy.Bogus.Core;

public static partial class BogusGenerator
{
    private static readonly string[] ChineseWords =
    {
        "系统", "平台", "服务", "用户", "订单", "任务", "配置", "规则", "流程", "状态",
        "日志", "消息", "通知", "数据", "统计", "结果", "方案", "策略", "标签", "资源"
    };

    private static readonly string[] ChineseTitleTemplates =
    {
        "关于{0}的处理通知",
        "{0}服务运行报告",
        "{0}状态变更提醒",
        "{0}数据汇总说明",
        "{0}流程执行结果"
    };

    private static readonly string[] ChineseRemarkTemplates =
    {
        "该记录由系统自动生成，请按实际业务校验。",
        "当前数据用于联调测试，可在发布前统一清理。",
        "已根据默认规则填充，必要时可通过自定义规则覆盖。",
        "此内容仅用于演示，不作为最终业务依据。"
    };

    private static readonly string[] ChinaProvinces =
    {
        "北京市", "上海市", "天津市", "重庆市", "广东省", "江苏省", "浙江省", "山东省", "福建省", "四川省",
        "湖北省", "湖南省", "河南省", "河北省", "陕西省", "辽宁省", "吉林省", "黑龙江省", "安徽省", "江西省"
    };

    private static readonly string[] ChinaCities =
    {
        "北京", "上海", "广州", "深圳", "杭州", "南京", "苏州", "武汉", "长沙", "郑州",
        "西安", "成都", "重庆", "青岛", "厦门", "福州", "合肥", "南昌", "沈阳", "大连"
    };

    private static readonly string[] ChinaDistricts =
    {
        "朝阳区", "海淀区", "浦东新区", "天河区", "南山区", "西湖区", "鼓楼区", "武侯区", "江汉区", "雨花区",
        "高新区", "经开区", "开发区", "滨江区", "思明区", "历下区", "金水区", "雁塔区", "渝中区", "姑苏区"
    };

    private static readonly string[] ChineseStreetNames =
    {
        "人民路", "中山路", "解放路", "建设路", "青年路", "新华路", "文化路", "和平路", "胜利路", "滨江路",
        "金融街", "科技大道", "创业路", "环城路", "学院路", "东风路", "光明路", "长江路", "黄河路", "天府大道"
    };

    private static readonly string[] ChineseCompanySuffixes =
    {
        "科技有限公司", "信息技术有限公司", "网络科技有限公司", "电子商务有限公司", "软件有限公司", "智能科技有限公司",
        "数据服务有限公司", "企业管理有限公司", "文化传媒有限公司", "咨询有限公司"
    };

    private static readonly string[] ChineseCompanyPrefixes =
    {
        "星河", "晨光", "云启", "智联", "华盛", "远航", "博睿", "乾元", "泰和", "明德",
        "天成", "鼎新", "同创", "优品", "蓝鲸", "领航", "鸿图", "云创", "卓越", "和信"
    };

    private static readonly string[] DomesticEmailDomains =
    {
        "qq.com", "163.com", "126.com", "foxmail.com", "outlook.com", "gmail.com", "sina.com", "sohu.com"
    };

    private static readonly Dictionary<char, string> PinyinCharMap = new()
    {
        ['张'] = "zhang",
        ['王'] = "wang",
        ['李'] = "li",
        ['赵'] = "zhao",
        ['刘'] = "liu",
        ['陈'] = "chen",
        ['杨'] = "yang",
        ['黄'] = "huang",
        ['吴'] = "wu",
        ['周'] = "zhou",
        ['徐'] = "xu",
        ['孙'] = "sun",
        ['胡'] = "hu",
        ['朱'] = "zhu",
        ['高'] = "gao",
        ['林'] = "lin",
        ['何'] = "he",
        ['郭'] = "guo",
        ['马'] = "ma",
        ['罗'] = "luo",
        ['梁'] = "liang",
        ['宋'] = "song",
        ['郑'] = "zheng",
        ['谢'] = "xie",
        ['韩'] = "han",
        ['唐'] = "tang",
        ['冯'] = "feng",
        ['于'] = "yu",
        ['董'] = "dong",
        ['萧'] = "xiao",
        ['程'] = "cheng",
        ['曹'] = "cao",
        ['袁'] = "yuan",
        ['邓'] = "deng",
        ['许'] = "xu",
        ['傅'] = "fu",
        ['沈'] = "shen",
        ['曾'] = "zeng",
        ['彭'] = "peng",
        ['吕'] = "lv",
        ['苏'] = "su",
        ['卢'] = "lu",
        ['蒋'] = "jiang",
        ['蔡'] = "cai",
        ['贾'] = "jia",
        ['丁'] = "ding",
        ['魏'] = "wei",
        ['薛'] = "xue"
    };
}
