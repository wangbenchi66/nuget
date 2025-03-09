namespace WebApi.Test.Apis
{
    /// <summary>
    /// 动态api测试
    /// </summary>
    public class DynamicApis : BaseApi
    {
        private readonly IUserRepository _userRepository;

        public DynamicApis(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 返回用户
        /// </summary>
        /// <remarks>
        /// ok
        /// </remarks>
        /// <returns></returns>
        public object GetUser()
        {
            //return _userRepository.GetUserId();
            return "ok";
        }
    }
}