//using System.Security.Claims;

//namespace EcoAppApi.Utils
//{
//    public class UserContextService : IUserContextService
//    {
//        private readonly IHttpContextAccessor _httpContextAccessor;

//        public UserContextService(IHttpContextAccessor httpContextAccessor)
//        {
//            _httpContextAccessor = httpContextAccessor;
//        }
//        public int GetUserId()
//        {
//            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
//                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

//            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
//            {
//                throw new UnauthorizedAccessException("User is not authenticated or User ID is invalid.");
//            }

//            return userId;
//        }

//    }
//}
