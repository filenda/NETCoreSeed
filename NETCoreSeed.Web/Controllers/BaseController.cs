using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreSeed.Web.Controllers
{
    public class BaseController : Controller
    {
        protected UserDTO _currentUser
        {
            get
            {
                return HttpContext.Session.Get<UserDTO>("user");
            }
            set
            {
                HttpContext.Session.Set<UserDTO>("user", value);
            }
        }
        protected readonly JsonSerializerSettings _settings;
        protected readonly IMapper _mapper;

        public BaseController(IMapper mapper)
        {
            _mapper = mapper;
            _settings = new JsonSerializerSettings
            {
                Error = (s, args) =>
                {
                    var currentError = args.ErrorContext.Error.Message;
                    args.ErrorContext.Handled = true;
                }
            };
        }

        protected string CreateRequestURI(string controller, List<KeyValuePair<string, string>> parameters = null, RoutingOrQueryString parametersType = RoutingOrQueryString.Routing)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Util.Constants.ApplicationURL);
            sb.Append("/" + controller);

            if (parameters != null)
            {
                if (parametersType == RoutingOrQueryString.Routing)
                {
                    foreach (var parameter in parameters)
                        sb.AppendFormat("/{0}", parameter.Value);
                }
                else
                {
                    sb.Append("?");

                    foreach (var parameter in parameters)
                        sb.AppendFormat("&{0}={1}", parameter.Key, parameter.Value);
                }
            }

            return sb.ToString();
        }

        protected string CreatePagSeguroRequestURI(string endpoint, bool isSandBox = false, List<KeyValuePair<string, string>> parameters = null)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(isSandBox ? Runtime.PagSeguroSandBoxApiAdress : Runtime.PagSeguroApiAdress);
            sb.Append("/" + endpoint);

            sb.Append("?");
            sb.AppendFormat("{0}={1}", "email", Runtime.PagSeguroEmail);
            sb.AppendFormat("&{0}={1}", "token", Runtime.PagSeguroToken);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                    sb.AppendFormat("&{0}={1}", parameter.Key, parameter.Value);
            }

            return sb.ToString();
        }

        protected async Task<PaginatedList<ActivityDTO>> LoadActivitiesAsync(Guid? gymId = null, ActivityStatus? ActivityStatus = null, int? PageIndex = null, int? PageSize = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (_currentUser != null)
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("GymId", gymId.HasValue ? gymId.ToString() : null));
                parameters.Add(new KeyValuePair<string, string>("PageIndex", PageIndex.HasValue ? PageIndex.ToString() : null));
                parameters.Add(new KeyValuePair<string, string>("PageSize", PageSize.HasValue ? PageSize.ToString() : null));
                parameters.Add(new KeyValuePair<string, string>("ActivityStatus", ActivityStatus.HasValue ? ActivityStatus.ToString() : null));

                var url = CreateRequestURI("activity", parameters: parameters, parametersType: ValueObjects.RoutingOrQueryString.QueryString);

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(PaginatedList<ActivityDTO>), _settings);

                            return (PaginatedList<ActivityDTO>)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        protected async Task<IEnumerable<CompanyDTO>> LoadCompaniesAsync()
        {
            using (var httpClient = new HttpClient())
            {
                if (_currentUser != null)
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var url = CreateRequestURI("company");

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(List<CompanyDTO>), _settings);

                            return (List<CompanyDTO>)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        protected async Task<IEnumerable<PlanDTO>> LoadPlansAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var url = CreateRequestURI("plan");

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(List<PlanDTO>), _settings);

                            return (List<PlanDTO>)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        protected async Task<PaginatedList<ClassDTO>> LoadClassesAsync(Guid? GymId = null, string GooglePlaceId = null, Guid? ActivityId = null, DateTime? ScheduledFor = null, DayOfWeek? DayOfWeek = null, TimeSpan? From = null, TimeSpan? To = null, int? PageIndex = null, int? PageSize = null)
        {
            using (var httpClient = new HttpClient())
            {
                if (_currentUser != null)
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("GymId", GymId.ToString()));
                parameters.Add(new KeyValuePair<string, string>("GooglePlaceId", GooglePlaceId));
                parameters.Add(new KeyValuePair<string, string>("ActivityId", ActivityId.HasValue ? ActivityId.ToString() : null));
                parameters.Add(new KeyValuePair<string, string>("ScheduledFor", ScheduledFor.HasValue ? ScheduledFor.Value.ToString(CultureInfo.InvariantCulture) : null));
                parameters.Add(new KeyValuePair<string, string>("DayOfWeek", DayOfWeek.HasValue ? ((int)DayOfWeek).ToString() : null));
                parameters.Add(new KeyValuePair<string, string>("From", From.HasValue ? From.ToString() : null));
                parameters.Add(new KeyValuePair<string, string>("PageIndex", PageIndex.HasValue ? PageIndex.ToString() : null));
                parameters.Add(new KeyValuePair<string, string>("PageSize", PageSize.HasValue ? PageSize.ToString() : null));
                parameters.Add(new KeyValuePair<string, string>("To", To.HasValue ? To.ToString() : null));

                var url = CreateRequestURI("class", parameters, ValueObjects.RoutingOrQueryString.QueryString);

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(PaginatedList<ClassDTO>), _settings);

                            return (PaginatedList<ClassDTO>)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        protected async Task<ClassDTO> LoadClassByIdAsync(Guid classId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var url = CreateRequestURI("class/" + classId.ToString());

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(ClassDTO), _settings);

                            return (ClassDTO)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        protected async Task<GymPlanDTO> LoadGymPlanById(Guid GymPlanId, Guid GymId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var url = CreateRequestURI("gym/" + GymId.ToString() + "/gymplans/" + GymPlanId.ToString());

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(GymPlanDTO), _settings);

                            return (GymPlanDTO)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        protected async Task<PlanDTO> LoadPlanById(int planId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("id", planId.ToString()));

                var url = CreateRequestURI("plan", parameters: parameters);

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(PlanDTO), _settings);

                            return (PlanDTO)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }


        protected async Task<UserDTO> LoadUserByIdAsync(Guid userId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var url = CreateRequestURI("user/" + userId.ToString());

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(UserDTO), _settings);

                            return (UserDTO)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        protected async Task<UserTokenDTO> ValidateTokenAsync(string token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var url = CreateRequestURI("user/token");

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("Token", token),
                    });

                    // TODO : Optional setup for PUT and POST methods, should be removed for GET method.
                    httpRequestMessage.Content = content;

                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(UserTokenDTO), _settings);

                            return (UserTokenDTO)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        protected async Task<ValidationResult> UpdateUserAsync(UserViewModel user)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var url = CreateRequestURI("user/" + user.UserId);

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, url))
                {
                    UserDTO userDto = _mapper.Map<UserDTO>(user);

                    var body = JsonConvert.SerializeObject(userDto, _settings);

                    var content = new StringContent(body, UTF8Encoding.UTF8, "application/json");

                    httpRequestMessage.Content = content;

                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        // Reading result
                        string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                        // Deserializing json object and setting configure await as false to run code in background by running code in Task
                        var jsonObject = JsonConvert.DeserializeObject<ValidationResult>(readHttpResponse, _settings);

                        return jsonObject;
                    }
                }
            }
        }

        public async Task<IEnumerable<GymDTO>> LoadGymsAsync(int PlanId = default(int), string userId = "", int gymStatus = 1, Guid? activityId = null, string gymName = "", string googlePlaceIds = "", string HasHandicappedPersonAccess = "", string HasItsOwnCarParkingSite = "")
        {
            using (var httpClient = new HttpClient())
            {
                if (_currentUser != null)
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("planId", PlanId.ToString()));
                parameters.Add(new KeyValuePair<string, string>("userId", userId));
                parameters.Add(new KeyValuePair<string, string>("gymName", gymName));
                parameters.Add(new KeyValuePair<string, string>("googlePlaceIds", googlePlaceIds));
                parameters.Add(new KeyValuePair<string, string>("gymStatus", gymStatus.ToString()));
                parameters.Add(new KeyValuePair<string, string>("activityId", activityId.ToString()));
                parameters.Add(new KeyValuePair<string, string>("HasHandicappedPersonAccess", string.IsNullOrEmpty(HasHandicappedPersonAccess) ? "false" : "true"));
                parameters.Add(new KeyValuePair<string, string>("HasItsOwnCarParkingSite", string.IsNullOrEmpty(HasItsOwnCarParkingSite) ? "false" : "true"));

                var url = CreateRequestURI("gym", parameters, ValueObjects.RoutingOrQueryString.QueryString);

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            // Reading result
                            string readHttpResponse = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                            // Deserializing json object and setting configure await as false to run code in background by running code in Task
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(List<GymDTO>), _settings);

                            return (List<GymDTO>)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        protected bool LoggedOut()
        {
            return (HttpContext.Session.Get<UserDTO>("user") == null);
        }

        protected ValidationResult GetAllCurrentModelStateErrors()
        {
            ValidationResult result = new ValidationResult();

            foreach (var modelState in ModelState.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    result.Add(modelError.ErrorMessage);
                }
            }

            return result;
        }

        public RedirectToActionResult RedirectToLoginPage()
        {
            TempData.Put<ValidationResult>("ValidationResult", new ValidationResult().Add("Você não possui acesso a este recurso"));

            return RedirectToAction("Login", "Home");
        }
    }
}