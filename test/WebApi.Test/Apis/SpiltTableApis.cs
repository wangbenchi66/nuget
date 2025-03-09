using Easy.Common.Core.Extensions;
using Easy.SqlSugar.Core;
using SqlSugar;
using SqlSugar.SplitTableExtensions;
using WBC66.Autofac.Core;

namespace WebApi.Test.Apis
{
    public class SpiltTableApis : BaseApi
    {
        private readonly SpiltTableRepository _spiltTableRepository;

        public SpiltTableApis(SpiltTableRepository spiltTableRepository)
        {
            _spiltTableRepository = spiltTableRepository;
        }

        public object InitTable()
        {
            //_spiltTableRepository.SqlSugarDbContext.CodeFirst.InitTables
            return "ok";
        }

        public object CreteData()
        {
            var entity = new SpiltTable() { Name = "test", CreateTime = "2023-02-1".ToDateTime().Value };
            _spiltTableRepository.SqlSugarDbContext.Insertable(entity).SplitTable().ExecuteReturnSnowflakeId();
            return "ok";
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <remarks>返回分表数据</remarks>
        /// <returns></returns>
        public object GetData()
        {
            return _spiltTableRepository.SqlSugarDbContext.Queryable<SpiltTable>()
                .SplitTable(tabs => tabs.Take(3))//近3张，也可以表达式选择
                .ToList();
        }
    }

    public class SpiltTableRepository : BaseSqlSugarRepository<SpiltTable>, IScoped
    {
    }

    [SplitTable(SplitType.Year)]
    [SugarTable("spilt_table_{year}_{month}_{day}")]
    [Tenant("journal")]
    public class SpiltTable
    {
        [SugarColumn(IsPrimaryKey = true)] public long Id { get; set; }
        public string Name { get; set; }
        [SplitField] public DateTime CreateTime { get; set; }
    }
}