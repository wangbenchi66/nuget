using Mapster;

namespace WebApi.Test.Apis
{
    /// <summary>
    /// Mapster测试
    /// </summary>
    public class MapsterApis : BaseApi
    {
        /// <summary>
        /// 最简单的映射
        /// </summary>
        /// <returns></returns>
        public object Get()
        {
            var user = new User
            {
                Id = 1,
                FirstName = "张",
                LastName = "三",
                Address = new Address
                {
                    City = "北京",
                    Street = "长安街"
                }
            };

            // 假设 UserDto 有相同属性，直接映射
            var dto = user.Adapt<UserDto>();
            //因为 UserDto 里 FullName、City 不对应，需要配置或映射自定义。
            return dto;
        }
        /// <summary>
        /// 配置映射规则
        /// </summary>
        /// <returns></returns>
        public object GetMappingConfig()
        {
            //这里应当是在应用启动时配置一次，而不是每次请求都配置。
            TypeAdapterConfig<User, UserDto>.NewConfig()
                .Map(dest => dest.FullName, src => $" {src.FirstName} {src.LastName}")
                .Map(dest => dest.City, src => src.Address.City);
            var user = new User
            {
                Id = 1,
                FirstName = "张",
                LastName = "三",
                Address = new Address
                {
                    City = "北京",
                    Street = "长安街"
                }
            };
            // 使用配置后的映射
            var dto = user.Adapt<UserDto>();
            return dto;
        }
        /// <summary>
        /// 即时映射（无需预先配置）
        /// </summary>
        /// <returns></returns>
        public object GetInstantMapping()
        {
            var user = new User
            {
                Id = 1,
                FirstName = "张",
                LastName = "三",
                Address = new Address
                {
                    City = "北京",
                    Street = "长安街"
                }
            };

            var dto = user.Adapt<UserDto>(new TypeAdapterConfig()
    .NewConfig<User, UserDto>()
        .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
        .Map(dest => dest.City, src => src.Address.City)
        .Config);  // ⚠️ 注意要加 .Config

            return dto;
        }

        /// <summary>
        /// 列表批量映射
        /// </summary>
        /// <returns></returns>
        public object GetListMapping()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "张",
                    LastName = "三",
                    Address = new Address
                    {
                        City = "北京",
                        Street = "长安街"
                    }
                },
                new User
                {
                    Id = 2,
                    FirstName = "李",
                    LastName = "四",
                    Address = new Address
                    {
                        City = "上海",
                        Street = "南京路"
                    }
                }
            };
            // 使用批量映射
            var dtos = users.Adapt<List<UserDto>>(new TypeAdapterConfig()
                .NewConfig<User, UserDto>()
                .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
                .Map(dest => dest.City, src => src.Address.City)
                .Config);
            return dtos;
        }
        /// <summary>
        /// 深拷贝示例
        /// </summary>
        /// <returns></returns>
        public object GetDeepCopy()
        {
            var user = new User
            {
                Id = 1,
                FirstName = "张",
                LastName = "三"
            };
            // 使用深拷贝
            var dto = user.Adapt<User>();
            // 修改原对象不会影响 dto
            user.FirstName = "李";
            user.LastName = "四";
            return new
            {
                Original = user,
                Copied = dto
            };
        }

        private class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public Address Address { get; set; } = new();
        }

        private class Address
        {
            public string City { get; set; } = "";
            public string Street { get; set; } = "";
        }

        private class UserDto
        {
            public int Id { get; set; }
            public string FullName { get; set; } = "";
            public string City { get; set; } = "";
        }
    }
}