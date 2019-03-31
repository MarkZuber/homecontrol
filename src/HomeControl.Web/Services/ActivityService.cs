using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeControl.Web.Activities;

namespace HomeControl.Web.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityFactory _activityFactory;
        private readonly ConcurrentDictionary<string, IActivity> _activities = new ConcurrentDictionary<string, IActivity>();

        // Get the activities by dependency injection
        public ActivityService(IActivityFactory activityFactory)
        {
            _activityFactory = activityFactory;
        }

        private IActivity GetActivity(string activityKey)
        {
            lock (_activities)
            {
                IActivity activity;
                while (!_activities.TryGetValue(activityKey, out activity))
                {
                    _activities[activityKey] = _activityFactory.GetActivity(activityKey);
                }

                return activity;
            }
        }

        public Task ExecuteActivityAsync(string activityKey, CancellationToken cancellationToken)
        {
            return GetActivity(activityKey).ExecuteAsync(cancellationToken);
        }
    }
}
