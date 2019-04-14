using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeControl.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HomeControl.Web.Activities
{
    public class ActivityFactory : IActivityFactory
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly Dictionary<string, Type> s_activityTypes = new Dictionary<string, Type>
        {
            { ActivityKey.TheaterOff, typeof(TheaterOffActivity) },
            { ActivityKey.TheaterOn, typeof(TheaterOnActivity) },
            { ActivityKey.TheaterXboxOn, typeof(TheaterXboxActivity) },
            { ActivityKey.TheaterPs4On, typeof(TheaterPs4Activity) },
            { ActivityKey.TheaterFireTvOn, typeof(TheaterFireTvActivity) },
            { ActivityKey.TheaterAppleTvOn, typeof(TheaterAppleTvActivity) },
            { ActivityKey.TheaterVolumeUp, typeof(TheaterVolumeUpActivity) },
            { ActivityKey.TheaterVolumeDown, typeof(TheaterVolumeDownActivity) },
            { ActivityKey.TheaterVolumeToggleMute, typeof(TheaterToggleMuteActivity) },
        };

        public ActivityFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IActivity GetActivity(string activityKey)
        {
            if (s_activityTypes.TryGetValue(activityKey, out var activityType))
            {
                return (IActivity)_serviceProvider.GetService(activityType);
            }
            throw new ArgumentException($"activityKey not found: {activityKey}", nameof(activityKey));
        }
    }
}
