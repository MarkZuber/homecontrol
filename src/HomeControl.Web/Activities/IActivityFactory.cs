using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControl.Web.Activities
{
    public interface IActivityFactory
    {
        IActivity GetActivity(string activityKey);
    }
}
