using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Easy.SqlSugar.Core;
using SqlSugar;

namespace WebApi.Test.Apis
{
    /// <summary>
    /// sqlsugar sqlfunc扩展函数
    /// </summary>
    public class SqlSugarSqlFuncApis : BaseApi
    {
        private readonly BaseSqlSugarRepository<User> _repository;
        public SqlSugarSqlFuncApis(BaseSqlSugarRepository<User> repository)
        {
            _repository = repository;
        }

        //获取不为空的用户列表
        public async Task<object> GetIsNotNull()
        {
            var users = await _repository.AsQueryable()
                .Where(u => u.Name.IsNotNull())
                .ToListAsync();
            return users;
        }

        //获取为空的用户列表
        public async Task<object> GetIsNull()
        {
            var users = await _repository.AsQueryable()
                .Where(u => u.Name.IsNull())
                .ToListAsync();
            return users;
        }
    }
}