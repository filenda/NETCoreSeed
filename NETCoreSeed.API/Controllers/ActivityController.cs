using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NETCoreSeed.API.Controllers
{
    [Route("api/Activity")]
    public class ActivityController : BaseController
    {
        private readonly IActivityService _service;

        public ActivityController(IActivityService service, IMapper mapper, IAuthorizationService authorizationService) : base(mapper, authorizationService)
        {
            _service = service;
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("{id:guid}")]
        public IActionResult Post(string reason, Guid id, string operation)
        {
            var result = new ValidationResult();
            var activity = _service.Get(id);

            if (activity == null)
                return NotFound();

            switch (operation)
            {
                case "approve":
                    {
                        activity.ActivityStatus = Shared.ValueObject.ActivityStatus.Allowed;
                        result = _service.Update(activity);
                        break;
                    }
                case "refuse":
                    {
                        activity.ActivityStatus = Shared.ValueObject.ActivityStatus.Denied;
                        result = _service.Update(activity);
                        break;
                    }
                default:
                    {
                        return BadRequest();
                    }
            }

            if (result.IsValid)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        public IActionResult Post([FromBody]ActivityDTO ActivityDto)
        {
            var _activity = new Activity();

            _activity = _mapper.Map<Activity>(ActivityDto);

            var result = _service.Add(_activity);

            if (result.IsValid)
            {
                return new OkObjectResult(result);
            }
            else
            {
                return new BadRequestObjectResult(result);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public ActivityDTO Get(Guid id)
        {
            var Activity = _service.Get(id);

            return _mapper.Map<ActivityDTO>(Activity);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public IActionResult Put(Guid id, [FromBody]ActivityDTO activityDto)
        {
            var activity = _service.Get(id);

            if (activity == null)
                return BadRequest(BuildListOfErrors(new ValidationResult().Add("Modalidade não encontrada")));

            if ((User.FindFirst("AnyGym").Value.Equals("Admin")))
            {
                activity = _mapper.Map<Activity>(activityDto);

                var result = _service.Update(activity);

                if (result.IsValid)
                {
                    return new OkObjectResult(result);
                }
                else
                {
                    return new BadRequestObjectResult(result);
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            if (id == null || id == default(Guid))
                return BadRequest(new ValidationResult().Add("Modalidade inexistente na base"));

            var activityToDelete = _service.Get(id);

            if (activityToDelete != null)
            {
                activityToDelete.Active = false;
                var result = _service.Update(activityToDelete);

                if (result.IsValid)
                    return Ok();
                else
                    return BadRequest();
            }

            return NotFound();
        }

        [AllowAnonymous]
        [HttpGet]
        public PaginatedList<ActivityDTO> Get(Guid GymId, string ActivityName, ActivityStatus? ActivityStatus, int? PageIndex = null, int? PageSize = null)
        {
            PaginatedList<Activity> activities;

            if (!User.Identity.IsAuthenticated)
                activities = _service.GetFilteredActivities(GymId, ActivityName, Shared.ValueObject.ActivityStatus.Allowed, PageIndex, PageSize);
            else if (User.FindFirst("AnyGym").Value == "Attendee")
                activities = _service.GetFilteredActivities(GymId, ActivityName, Shared.ValueObject.ActivityStatus.Allowed, PageIndex, PageSize);
            else
                activities = _service.GetFilteredActivities(GymId, ActivityName, ActivityStatus, PageIndex, PageSize);

            var returnObj = _mapper.Map<PaginatedList<ActivityDTO>>(activities);

            returnObj.TotalPages = activities.TotalPages;
            returnObj.PageIndex = activities.PageIndex;

            return returnObj;
        }
    }
}