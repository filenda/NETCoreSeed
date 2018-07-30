using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Repository
{
    using System;
    using System.Collections.Generic;
    using Any.Gym.CoreDomain.Entities;
    using Any.Gym.CoreDomain.Repository.Interfaces;
    using Any.Gym.Infrastructure.Context.DataContext;
    using Any.Gym.Infrastructure.Repository.Common;
    using System.Text;
    using System.Data.SqlClient;
    using Any.Gym.Shared;
    using Dapper;
    using MySql.Data.MySqlClient;
    using Any.Gym.Shared.ValueObject;
    using Any.Gym.Shared.Helpers;
    using System.Linq;

    namespace Any.Gym.Infrastructure.Repository
    {
        public class ActivityRepository : Repository<Activity>, IActivityRepository
        {
            public ActivityRepository(AnyGymDataContext context)
                : base(context)
            {
            }

            public PaginatedList<Activity> GetFilteredActivities(Guid GymId, string ActivityName, ActivityStatus? ActivityStatus, int PageIndex = 1, int? PageSize = null)
            {
                IEnumerable<Activity> activities;

                StringBuilder query = new StringBuilder();

                query.Append("select distinct Activities.* from Activities ");
                query.Append("left join Classes on Activities.ActivityId = Classes.activityId ");
                query.Append("left join Gyms on Gyms.GymId = Classes.GymId ");
                query.Append("where Activities.Active = 1");

                if (ActivityStatus.HasValue && ActivityStatus != null)
                    query.AppendFormat(" and Activities.ActivityStatus = {0}", (int)ActivityStatus.Value);
                if (GymId != null && GymId != default(Guid))
                    query.Append(" and Gyms.GymId = @GYMID ");
                if (!string.IsNullOrEmpty(ActivityName))
                    query.Append(" and Activities.ActivityName LIKE '%@ACTIVITYNAME%'");

                using (var conn = new MySqlConnection(Runtime.MySqlConnectionString))
                //using (var conn = new SqlConnection(Runtime.SQLConnectionString))
                {
                    conn.Open();
                    activities = conn.Query<Activity>(query.ToString(),
                        param: new
                        {
                            GYMID = GymId.ToString(),
                            ACTIVITYNAME = ActivityName
                        });
                    conn.Close();
                }

                var count = activities.Count();
                var returnObj = new PaginatedList<Activity>(activities.ToList(), count, PageIndex, activities.Count());

                //Paging
                if (PageSize.HasValue)
                {
                    var skip = PageSize * (PageIndex - 1);
                    query.AppendFormat(" LIMIT {0}", PageSize.ToString());
                    query.AppendFormat(" OFFSET {0}", skip.ToString());

                    using (var conn = new MySqlConnection(Runtime.MySqlConnectionString))
                    //using (var conn = new SqlConnection(Runtime.SQLConnectionString))
                    {
                        conn.Open();
                        activities = conn.Query<Activity>(query.ToString(),
                            param: new
                            {
                                GYMID = GymId.ToString(),
                                ACTIVITYNAME = ActivityName
                            });
                        conn.Close();
                    }

                    returnObj = new PaginatedList<Activity>(activities.ToList(), count, PageIndex, PageSize.Value);
                }

                return returnObj;
            }

            public override Activity Get(Guid id)
            {
                return base.GetWithoutTrack(a => a.ActivityId == id);
            }
        }
    }
}