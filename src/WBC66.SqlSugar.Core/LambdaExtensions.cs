using System.Linq.Expressions;
using System.Reflection;
using SqlSugar;

namespace Easy.SqlSugar.Core
{
    public static class LambdaExtensions
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IQueryable<T> TakePage<T>(this IQueryable<T> queryable, int page, int size = 15)
        {
            return queryable.TakeOrderByPage<T>(page, size);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static IQueryable<T> TakeOrderByPage<T>(this IQueryable<T> queryable, int page, int size = 15, Expression<Func<T, Dictionary<object, QueryOrderBy>>> orderBy = null)
        {
            if (page <= 0)
            {
                page = 1;
            }
            return queryable.GetIQueryableOrderBy(orderBy.GetExpressionToDic())
                  .Skip((page - 1) * size)
                 .Take(size);
        }

        /// <summary>
        /// 创建lambda表达式：p=&gt;true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return p => true;
        }

        /// <summary>
        /// 创建lambda表达式：p=&gt;false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>()
        {

            return p => false;
        }

        public static ParameterExpression GetExpressionParameter(this Type type)
        {

            return Expression.Parameter(type, "p");
        }

        /// <summary>
        /// 创建lambda表达式：p=&gt;p.propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Expression<Func<T, TKey>> GetExpression<T, TKey>(this string propertyName)
        {
            return propertyName.GetExpression<T, TKey>(typeof(T).GetExpressionParameter());
        }

        /// <summary>
        /// 创建委托有返回值的表达式：p=&gt;p.propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Func<T, TKey> GetFun<T, TKey>(this string propertyName)
        {
            return propertyName.GetExpression<T, TKey>(typeof(T).GetExpressionParameter()).Compile();
        }

        /// <summary>
        /// 创建lambda表达式：p=&gt;false 在已知TKey字段类型时,如动态排序OrderBy(x=&gt;x.ID)会用到此功能,返回的就是x=&gt;x.ID Expression[Func[Out_Scheduling, DateTime&gt;&gt; expression = x =&gt; x.CreateDate;指定了类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, TKey>> GetExpression<T, TKey>(this string propertyName, ParameterExpression parameter)
        {
            if (typeof(TKey).Name == "Object")
                return Expression.Lambda<Func<T, TKey>>(Expression.Convert(Expression.Property(parameter, propertyName), typeof(object)), parameter);
            return Expression.Lambda<Func<T, TKey>>(Expression.Property(parameter, propertyName), parameter);
        }

        /// <summary> 
        /// 创建lambda表达式：p=>false object不能确认字段类型(datetime,int,string),如动态排序OrderBy(x=>x.ID)会用到此功能,返回的就是x=>x.ID Expression[Func[Out_Scheduling, object>> expression = x => x.CreateDate;任意类型的字段 
        /// </summary> 
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, object>> GetExpression<T>(this string propertyName)
        {
            return propertyName.GetExpression<T, object>(typeof(T).GetExpressionParameter());
        }

        public static Expression<Func<T, object>> GetExpression<T>(this string propertyName, ParameterExpression parameter)
        {
            return Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(parameter, propertyName), typeof(object)), parameter);
        }


        /// <summary>
        /// 表达式转换成KeyValList(主要用于多字段排序，并且多个字段的排序规则不一样) 如有多个字段进行排序,参数格式为 Expression[Func[Out_Scheduling, Dictionary[object, bool&gt;&gt;&gt; orderBy = x =&gt; new Dictionary[object, bool&gt;() { { x.ID, true }, { x.DestWarehouseName, true } }; 返回的是new Dictionary[object, bool&gt;(){{}}key为排序字段，bool为升降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, QueryOrderBy>> GetExpressionToPair<T>(this Expression<Func<T, Dictionary<object, QueryOrderBy>>> expression)
        {
            foreach (var exp in ((ListInitExpression)expression.Body).Initializers)
            {
                yield return new KeyValuePair<string, QueryOrderBy>(
                exp.Arguments[0] is MemberExpression ?
                (exp.Arguments[0] as MemberExpression).Member.Name.ToString()
                : ((exp.Arguments[0] as UnaryExpression).Operand as MemberExpression).Member.Name,
                 (QueryOrderBy)(
                 exp.Arguments[1] as ConstantExpression != null
                  ? ((ConstantExpression)exp.Arguments[1]).Value
                 //2021.07.04增加自定排序按条件表达式
                 : Expression.Lambda<Func<QueryOrderBy>>(exp.Arguments[1]).Compile()()
                 ));
            }
        }


        /// <summary>
        /// 表达式转换成KeyValList(主要用于多字段排序，并且多个字段的排序规则不一样) 如有多个字段进行排序,参数格式为 Expression[Func[Out_Scheduling, Dictionary[object, QueryOrderBy&gt;&gt;&gt; orderBy = x
        /// =&gt; new Dictionary[object, QueryOrderBy&gt;() { { x.ID, QueryOrderBy.Desc }, { x.DestWarehouseName, QueryOrderBy.Asc } }; 返回的是new Dictionary[object, QueryOrderBy&gt;(){{}}key为排序字段，QueryOrderBy为排序方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Dictionary<string, QueryOrderBy> GetExpressionToDic<T>(this Expression<Func<T, Dictionary<object, QueryOrderBy>>> expression)
        {
            //2020.09.14增加排序字段null值判断
            if (expression == null)
            {
                return new Dictionary<string, QueryOrderBy>();
            }
            return expression.GetExpressionToPair().Reverse().ToList().ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// 解析多字段排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="orderBySelector">string=排序的字段,bool=true降序/false升序</param>
        /// <returns></returns>
        public static IQueryable<TEntity> GetIQueryableOrderBy<TEntity>(this IQueryable<TEntity> queryable, Dictionary<string, QueryOrderBy> orderBySelector)
        {
            string[] orderByKeys = orderBySelector.Select(x => x.Key).ToArray();
            if (orderByKeys == null || orderByKeys.Length == 0) return queryable;

            IOrderedQueryable<TEntity> queryableOrderBy = null;
            // string orderByKey = orderByKeys[^1];
            string orderByKey = orderByKeys[orderByKeys.Length - 1];
            queryableOrderBy = orderBySelector[orderByKey] == QueryOrderBy.Desc
                ? queryableOrderBy = queryable.OrderByDescending(orderByKey.GetExpression<TEntity>())
                : queryable.OrderBy(orderByKey.GetExpression<TEntity>());

            for (int i = orderByKeys.Length - 2; i >= 0; i--)
            {
                queryableOrderBy = orderBySelector[orderByKeys[i]] == QueryOrderBy.Desc
                    ? queryableOrderBy.ThenByDescending(orderByKeys[i].GetExpression<TEntity>())
                    : queryableOrderBy.ThenBy(orderByKeys[i].GetExpression<TEntity>());
            }
            return queryableOrderBy;
        }

        /// <summary>
        /// 解析多字段排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="orderBySelector">string=排序的字段,bool=true降序/false升序</param>
        /// <returns></returns>
        public static ISugarQueryable<TEntity> GetISugarQueryableOrderBy<TEntity>(this ISugarQueryable<TEntity> queryable, Dictionary<string, QueryOrderBy> orderBySelector)
        {
            string[] orderByKeys = orderBySelector.Select(x => x.Key).ToArray();
            if (orderByKeys == null || orderByKeys.Length == 0) return queryable;

            ISugarQueryable<TEntity> queryableOrderBy = null;
            string orderByKey = orderByKeys[0];
            queryableOrderBy = orderBySelector[orderByKey] == QueryOrderBy.Desc
                ? queryableOrderBy = queryable.OrderByDescending(orderByKey.GetExpression<TEntity>())
                : queryable.OrderBy(orderByKey.GetExpression<TEntity>());

            for (int i = 1; i < orderByKeys.Length; i++)
            {
                queryableOrderBy = orderBySelector[orderByKeys[i]] == QueryOrderBy.Desc
                   ? queryableOrderBy.OrderByDescending(orderByKeys[i].GetExpression<TEntity>())
                   : queryableOrderBy.OrderBy(orderByKeys[i].GetExpression<TEntity>());
            }
            return queryableOrderBy;
        }

        /// <summary>
        /// https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/ 表达式合并(合并生产的sql语句有性能问题) 合并两个where条件，如：多个查询条件时，判断条件成立才where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            // apply composition of lambda expression bodies to parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static IQueryable<Result> GetQueryableSelect<Source, Result>(this IQueryable<Source> queryable)
        {
            Expression<Func<Source, Result>> expression = CreateMemberInitExpression<Source, Result>();
            return queryable.Select(expression);
        }

        //参照文档https://docs.microsoft.com/zh-cn/dotnet/api/system.linq.expressions.memberinitexpression?redirectedfrom=MSDN&view=netframework-4.8
        /// <summary>
        /// 动态创建表达式Expression[Func[Animal, User>> expression = CreateMemberInitExpression[Animal, User>();
        ///结果为Expression[Func[Animal, User>> expression1 = x => new User() { Age = x.Age, Species = x.Species };
        /// </summary>
        /// <typeparam name="Source"></typeparam>
        /// <typeparam name="Result"></typeparam>
        /// <returns></returns>
        public static Expression<Func<Source, Result>> CreateMemberInitExpression<Source, Result>(Type resultType = null)
        {
            resultType = resultType ?? typeof(Result);
            ParameterExpression left = Expression.Parameter(typeof(Source), "p");
            NewExpression newExpression = Expression.New(resultType);
            PropertyInfo[] propertyInfos = resultType.GetProperties();
            List<MemberBinding> memberBindings = new List<MemberBinding>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                MemberExpression member = Expression.Property(left, propertyInfo.Name);
                MemberBinding speciesMemberBinding = Expression.Bind(resultType.GetMember(propertyInfo.Name)[0], member);
                memberBindings.Add(speciesMemberBinding);
            }
            MemberInitExpression memberInitExpression = Expression.MemberInit(newExpression, memberBindings);
            Expression<Func<Source, Result>> expression = Expression.Lambda<Func<Source, Result>>(memberInitExpression, new ParameterExpression[] { left });
            return expression;
        }

        public static Expression<Func<Source, object>> CreateMemberInitExpression<Source>(Type resultType)
        {
            return CreateMemberInitExpression<Source, object>(resultType);
        }

        /// <summary>
        /// 属性判断待完
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetGenericProperties(this Type type)
        {
            return type.GetProperties().GetGenericProperties();
        }

        /// <summary>
        /// 属性判断待完
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetGenericProperties(this IEnumerable<PropertyInfo> properties)
        {
            return properties.Where(x => !x.PropertyType.IsGenericType && x.PropertyType.GetInterface("IList") == null || x.PropertyType.GetInterface("IEnumerable", false) == null);
        }
    }

    public class ParameterRebinder : ExpressionVisitor
    {

        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
}