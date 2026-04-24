using System.ComponentModel;
using System.Reflection;
using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.Infrastructure.Setups;

public enum ApiVersionInfo
{
    [Description("v1")]
    v1 = 1,

    [Description("v2")]
    v2 = 2
}

public static class SwaggerSetup
{
    private static string _swaggerName = string.Empty;

    /// <summary>
    /// 添加Swagger配置
    /// </summary>
    /// <param name="services"></param>
    /// <param name="swaggerName"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddSwaggerSetup(this IServiceCollection services, string swaggerName = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
        _swaggerName = swaggerName ?? AppService.GetProjectNameToLower();
        services.AddSwaggerGen((s) =>
        {
            //版本信息
            typeof(ApiVersionInfo).GetEnumNames().OrderBy(e => e).ToList().ForEach(version =>
            {
                s.SwaggerDoc(version, new OpenApiInfo
                {
                    Version = version,
                    Title = _swaggerName + "接口文档",
                    Description = $"{_swaggerName} HTTP Api {version}",
                    Contact = new OpenApiContact { Name = _swaggerName, Email = "69945864@qq.com" }
                });
            });
            s.OrderActionsBy(p => p.RelativePath);
            //批量生成api xml文档
            var xmlsFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
            foreach (var file in xmlsFiles)
            {
                s.IncludeXmlComments(file, true);
            }
            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "JWT授权(数据将在请求头中进行传输) 在下方输入Bearer {token} 即可，注意两者之间有空格",
            });
            //如果没有文档注释，默认显示控制器名称+方法
            s.CustomOperationIds(apiDesc =>
            {
                return (apiDesc.HttpMethod + "." + apiDesc.RelativePath).Replace("/", ".");
            });
            //swagger解析ApiResult的nuget文档
            s.SchemaFilter<ApiResultSchemaFilter>();
            //添加Swagger过滤器
            s.OperationFilter<SecurityRequirementsOperationFilter>();//添加安全要求过滤器
            s.OperationFilter<BearerAuthOperationFilter>();//添加Bearer Token鉴权过滤器
            s.OperationFilter<AllowAnonymousOperationFilter>();//添加匿名访问过滤器
            s.OperationFilter<ApiResultPlusSwaggerResponseFilter>();//解析ApiResultPlus<TSuccess,TError>
            s.OperationFilter<InheritedObsoleteFilter>();//添加继承Obsolete特性过滤器
        });
    }


    /// <summary>
    /// 使用Swagger配置
    /// </summary>
    /// <param name="app"></param>
    /// <param name="routePrefix">路由前缀，默认为前后不要带/符号</param>
    public static void UseSwaggerSetup(this WebApplication app, string routePrefix = null)
    {
        //处理路由前缀，确保前后没有/符号
        routePrefix = string.IsNullOrWhiteSpace(routePrefix) ? AppService.GetProjectNameToLower() : routePrefix.Trim('/');
        app.UseSwagger(c =>
        {
            c.RouteTemplate = routePrefix + "/swagger/{documentName}/swagger.json";
        });
        /*app.UseSwaggerUI(c =>
        {
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "api");
            c.RoutePrefix = "api";
        });*/
        //引入Knife4UI
        app.UseKnife4UI(c =>
        {
            typeof(ApiVersionInfo).GetEnumNames().OrderBy(e => e).ToList().ForEach(version =>
            {
                c.SwaggerEndpoint($"../{routePrefix}/swagger/{version}/swagger.json", version);
            });
            c.RoutePrefix = "k4j";
        });
        //scalar
        var documents = new List<ScalarDocument>();
        typeof(ApiVersionInfo).GetEnumNames().OrderBy(e => e).ToList().ForEach(version =>
        {
            documents.Add(new ScalarDocument(version, version, $"../{routePrefix}/swagger/{version}/swagger.json"));
        });
        app.MapScalarApiReference("/scalar", options =>
        {
            options.Title = _swaggerName;
            options.AddDocuments(documents);
            //设置默认的请求格式
            options.DefaultHttpClient = new(ScalarTarget.Node, ScalarClient.Axios);
            options.Layout = ScalarLayout.Modern;
            options.HideClientButton = true;
            options.Theme = ScalarTheme.DeepSpace;
        });
    }


    #region 过滤器


    /// <summary>
    /// Bearer Token鉴权过滤器，用于在Swagger UI中为需要鉴权的接口添加Bearer Token输入框。
    /// </summary>
    public class BearerAuthOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 将过滤器应用于指定的Swagger操作。
        /// </summary>
        /// <param name="operation">Swagger操作。</param>
        /// <param name="context">操作过滤器上下文。</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                // Name = "Authorization",
                                // Type = SecuritySchemeType.ApiKey,
                                // In = ParameterLocation.Header,
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                             },
                             new string[] { }
                        },
                    }
                };
        }
    }

    /// <summary>
    /// 匿名访问过滤器，用于移除标有[AllowAnonymous]特性的操作的认证要求。
    /// </summary>
    public class AllowAnonymousOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAllowAnonymous =
                context.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null
                || context.MethodInfo.DeclaringType?.GetCustomAttribute<AllowAnonymousAttribute>() != null;

            if (hasAllowAnonymous)
            {
                // 移除认证要求
                operation.Security.Clear();
            }
        }
    }

    /// <summary>
    /// 继承Obsolete特性过滤器，用于标记被[Obsolete]特性标记的方法或类为废弃，并在Swagger UI中显示相关信息。
    /// </summary>
    public class InheritedObsoleteFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var method = context.MethodInfo;
            var type = method.DeclaringType;

            // 检查当前类 + 所有基类
            bool isObsolete = false;

            while (type != null)
            {
                if (type.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any())
                {
                    isObsolete = true;
                    break;
                }

                type = type.BaseType;
            }

            // 方法级别
            if (method.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any())
            {
                isObsolete = true;
            }

            if (isObsolete)
            {
                operation.Deprecated = true;

                operation.Description ??= "";
                operation.Description += "\n\n此接口已废弃";
            }
        }
    }

    /// <summary>
    /// apiResultPlus解析过滤器
    /// </summary>
    public class ApiResultPlusSwaggerResponseFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var returnType = context.MethodInfo.ReturnType;

            // 解包 Task<T>
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }

            // 检查是否是 ApiResultPlus<T, E>
            if (returnType.IsGenericType &&
                returnType.GetGenericTypeDefinition() == typeof(ApiResultPlus<,>))
            {
                var genericArgs = returnType.GetGenericArguments();
                var successType = genericArgs[0]; // TSuccess
                var errorType = genericArgs[1];   // TError

                // 200 OK - 成功响应
                operation.Responses["200"] = new OpenApiResponse
                {
                    Description = "操作成功返回",
                    Content = CreateJsonContent(successType)
                };

                // 400 Bad Request - 错误响应
                operation.Responses["400"] = new OpenApiResponse
                {
                    Description = "操作失败返回",
                    Content = CreateJsonContent(errorType)
                };
            }
        }

        private static Dictionary<string, OpenApiMediaType> CreateJsonContent(Type type)
        {
            if (type == typeof(void) || type == typeof(object))
            {
                return new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema { Type = "object" }
                    }
                };
            }

            return new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = type.Name
                        }
                    }
                }
            };
        }
    }

    #endregion 过滤器
}

/// <summary>
/// ApiResult统一返回结构的SchemaFilter
/// </summary>
public class ApiResultSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.Name.StartsWith("ApiResult", StringComparison.OrdinalIgnoreCase))
            return;

        schema.Description = "统一返回结构";

        foreach (var prop in schema.Properties)
        {
            var key = prop.Key.ToLowerInvariant(); // 忽略大小写
            switch (key)
            {
                case "success":
                    prop.Value.Description = "是否成功";
                    break;

                case "msg":
                    prop.Value.Description = "返回消息";
                    break;

                case "data":
                    prop.Value.Description = "数据内容";
                    break;

                case "statecode":
                    prop.Value.Description = "HTTP状态码";
                    break;

                case "traceid":
                    prop.Value.Description = "消息唯一码";
                    break;
            }
        }
    }
}