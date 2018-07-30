namespace NETCoreSeed.Web.Controllers
{
    public class ActivityController : BaseController
    {
        public ActivityController(IMapper mapper) : base(mapper) { }

        public async Task<ActionResult> Index(Guid GymId, ActivityStatus? activitystatus, int? PageIndex)
        {
            var activitiesVm = new PaginatedList<ActivityViewModel>();

            if (LoggedOut())
            {
                TempData.Put<ValidationResult>("ValidationResult", new ValidationResult().Add("Por favor, logue denovo!"));
                return RedirectToAction("Login", "Home");
            }

            var activities = await LoadActivitiesAsync(GymId, activitystatus, PageIndex, 3);

            activitiesVm = _mapper.Map<PaginatedList<ActivityViewModel>>(activities);

            activitiesVm.TotalPages = activities.TotalPages;
            activitiesVm.PageIndex = activities.PageIndex;

            return View(activitiesVm);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult RefuseActivity(Guid ActivityId)
        {
            var activityRefusal = new ActivityRefusalViewModel();

            activityRefusal.ActivityId = ActivityId;

            return PartialView("_RefuseActivity");
        }

        [HttpPost]
        public async Task<ActionResult> RefuseActivity(ActivityRefusalViewModel refusal)
        {
            var result = await RefuseActivityAsync(refusal);

            if (result.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData.Put<ValidationResult>("ValidationResult", result);
                return View("Index", "Activity");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ApproveActivity(Guid ActivityId)
        {
            var result = await ApproveActivityAsync(ActivityId);

            if (result.IsValid)
            {
                return Json(new { redirectURL = Url.Action("Index") });
            }
            else
            {
                TempData.Put<ValidationResult>("ValidationResult", result);
                return Json(new { redirectURL = Url.Action("Index") });
            }
        }

        [HttpGet]
        public async Task<JsonResult> LoadActivitiesAsync()
        {
            var availableActivities = await base.LoadActivitiesAsync();

            return Json(availableActivities.Items.Select(a => new { id = a.ActivityId, name = a.ActivityName }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(ActivityViewModel activity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await CreateActivityAsync(activity);

                    if (result.IsValid)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData.Put<ValidationResult>("ValidationResult", result);
                        return View("Create", activity);
                    }
                }
                catch
                {
                    return View(nameof(Create), activity);
                }
            }
            return View();
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteAsync(Guid ActivityId)
        {
            try
            {
                var result = await DeleteActivitiyAsync(ActivityId);

                if (result)
                {
                    return Json(new { redirectURL = Url.Action("Index") });
                }
                else
                {
                    return Json(null);
                }
            }
            catch
            {
                return Json(null);
            }
        }

        public async Task<ActionResult> Edit(Guid ActivityId)
        {
            if (ActivityId == null | ActivityId == default(Guid))
                return RedirectToAction(nameof(Index));

            var activity = await LoadActivityById(ActivityId);

            return View(_mapper.Map<ActivityViewModel>(activity));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(ActivityViewModel activity)
        {
            try
            {
                var result = await UpdateActivityAsync(activity);

                if (result.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData.Put<ValidationResult>("ValidationResult", result);
                    return View("Edit", activity);
                }
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #region Methods
        public async Task<ValidationResult> ApproveActivityAsync(Guid ActivityId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var url = CreateRequestURI(String.Format("activity/{0}?operation={1}", ActivityId.ToString(), "approve"));

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url))
                {
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

        public async Task<ValidationResult> RefuseActivityAsync(ActivityRefusalViewModel refusal)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var url = CreateRequestURI(String.Format("activity/{0}?operation={1}&reason={2}", refusal.ActivityId.ToString(), "refuse", refusal.Reason));

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url))
                {
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

        public async Task<ValidationResult> UpdateActivityAsync(ActivityViewModel activity)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("id", activity.ActivityId.ToString()));

                var url = CreateRequestURI("activity", parameters: parameters);

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, url))
                {
                    ActivityDTO activityDto = _mapper.Map<ActivityDTO>(activity);

                    var body = JsonConvert.SerializeObject(activityDto, _settings);

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

        public async Task<ActivityDTO> LoadActivityById(Guid gymId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(new KeyValuePair<string, string>("id", gymId.ToString()));

                var url = CreateRequestURI("activity", parameters: parameters);

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
                            var jsonObject = JsonConvert.DeserializeObject(readHttpResponse, typeof(ActivityDTO), _settings);

                            return (ActivityDTO)jsonObject;
                        }

                        return null;
                    }
                }
            }
        }

        public async Task<ValidationResult> CreateActivityAsync(ActivityViewModel activity)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var url = CreateRequestURI("activity");

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    ActivityDTO activityDto = _mapper.Map<ActivityDTO>(activity);

                    var body = JsonConvert.SerializeObject(activityDto, _settings);

                    var content = new StringContent(body, UTF8Encoding.UTF8, "application/json");

                    // TODO : Optional setup for PUT and POST methods, should be removed for GET method.
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

        public async Task<bool> DeleteActivitiyAsync(Guid ActivityId)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.Get<UserDTO>("user").Token);

                var id = new KeyValuePair<string, string>("id", ActivityId.ToString());
                var parameters = new List<KeyValuePair<string, string>>();

                parameters.Add(id);

                var url = CreateRequestURI("activity", parameters: parameters);

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, url))
                {
                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false))
                    {
                        return httpResponse.IsSuccessStatusCode;
                    }
                }
            }
        }
        #endregion
    }
}