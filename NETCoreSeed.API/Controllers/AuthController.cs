namespace NETCoreSeed.API.Controllers
{
    public class AuthController : BaseController
    {
        #region Properties
        protected User _user;
        private readonly IUserService _userService;
        private readonly TokenOptions _tokenOptions;
        #endregion

        #region Constructor
        public AuthController(IOptions<TokenOptions> jwtOptions, IUserService userService, IMapper mapper, IAuthorizationService authorizationService) : base(mapper, authorizationService)
        {
            _userService = userService;
            _tokenOptions = jwtOptions.Value;
        }
        #endregion

        #region Authentication
        protected async Task<string> GetEncodedJwtAsync(AuthenticateUserCommand command, ClaimsIdentity identity)
        {
            if (identity == null)
                return string.Empty;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, command.Email),
                new Claim(JwtRegisteredClaimNames.NameId, command.Email),
                new Claim(JwtRegisteredClaimNames.Email, command.Email),
                new Claim(JwtRegisteredClaimNames.Sub, command.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _tokenOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_tokenOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("AnyGym"),
                identity.FindFirst("UserId")
            };

            var jwt = new JwtSecurityToken(
                issuer: _tokenOptions.Issuer,
                audience: _tokenOptions.Audience,
                claims: claims.AsEnumerable(),
                notBefore: _tokenOptions.NotBefore,
                expires: _tokenOptions.Expiration,
                signingCredentials: _tokenOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        protected static void ThrowIfInvalidOptions(TokenOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("O período deve ser maior que zero", nameof(TokenOptions.ValidFor));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(TokenOptions.SigningCredentials));

            if (options.JtiGenerator == null)
                throw new ArgumentNullException(nameof(TokenOptions.JtiGenerator));
        }

        protected static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        protected Task<ClaimsIdentity> GetClaims(AuthenticateUserCommand command)
        {
            var user = _userService.GetByEmail(command.Email);
            ValidationResult result = new ValidationResult();

            if (user == null)
                return Task.FromResult<ClaimsIdentity>(null);

            if (!user.Authenticate(command.Email, command.Password, ref result))
            {
                if (!result.IsValid)
                {
                    return Task.FromResult<ClaimsIdentity>(null);
                }
            }

            _user = user;

            //Save user id in the session for later use
            //HttpContext.Session.Set<User>("User", _user);

            string profile = string.Empty;

            switch (user.Profile)
            {
                case UserProfile.Admin:
                    profile = "Admin";
                    break;
                case UserProfile.Attendee:
                    profile = "Attendee";
                    break;
                case UserProfile.GymOwner:
                    profile = "GymOwner";
                    break;
                case UserProfile.Enterprise:
                    profile = "Enterprise";
                    break;
            }

            return Task.FromResult(new ClaimsIdentity(
                new GenericIdentity(user.Email, "Token"),
                new[] {
                    new Claim("AnyGym", profile),
                    new Claim("UserId", user.UserId.ToString())
                }));
        }

        /// <summary>
        /// Exclusive for external facebook auth
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        protected Task<ClaimsIdentity> GetClaims(string Email, UserProfile Profile, Guid UserId)
        {
            var profile = string.Empty;

            switch (Profile)
            {
                case UserProfile.Admin:
                    profile = "Admin";
                    break;
                case UserProfile.Attendee:
                    profile = "Attendee";
                    break;
                case UserProfile.GymOwner:
                    profile = "GymOwner";
                    break;
                case UserProfile.Enterprise:
                    profile = "Enterprise";
                    break;
            }

            return Task.FromResult(new ClaimsIdentity(
                new GenericIdentity(Email, "Token"),
                new[] {
                    new Claim("AnyGym", profile),
                    new Claim("UserId", UserId.ToString())
                }));
        }


        #endregion
    }
}