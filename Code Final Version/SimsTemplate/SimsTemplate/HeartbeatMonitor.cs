using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Timers;
using System.Web;

namespace SimsTemplate
{
    /// <summary>
    /// Maintains all current users that are currently online. Does this by setting a timer that goes off at a set interval.
    /// When the timer goes off, it updates a dictionary with the current time.
    /// 
    /// @author Taylor Dixon
    /// </summary>

    public class HeartbeatMonitor 
    {

        private const int CUTOFF_TIME_SECONDS            = 40;
        private const int HEARTBEAT_TIME_MILLISECONDS    = 30000;

        private string uid;
        private Timer lTimer;
        private HttpContextBase context;

        /// <summary>
        /// Creates a login timer object.
        /// </summary>
        /// <param name="uid">The uid of the user to monitor.</param>
        /// <param name="context">The current HttpContext of the page.</param>
        public HeartbeatMonitor(int uid, HttpContextBase context)
        {
            //If the uid is null, do not do anything.
            if (uid != -1)
            {
                this.uid = Convert.ToString(uid);
                this.context = context;

                lTimer = new Timer();

                lTimer.Elapsed += lTimer_Tick;
                lTimer.Interval = HEARTBEAT_TIME_MILLISECONDS;

                lTimer.Start();

                //Update right away to signify that the user is on the page
                updateHeartbeat();
            }
        }

        
        /// <summary>
        /// Occurs whenever the timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lTimer_Tick(object sender, EventArgs e)
        {
            updateHeartbeat();
        }

        /// <summary>
        /// Update the usersonline dictionary to set the current users last heartbeat time to now.
        /// </summary>
        private void updateHeartbeat()
        {
            context.Application.Lock();
            ((Dictionary<String, DateTime>)context.Application["UsersOnline"])[uid] = DateTime.Now;
            context.Application.UnLock();
            
        }

        /// <summary>
        /// Gets all the users that meet the cutoff time defined.
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        /// <returns>A list of strings containing the uids of all users that meet the cutoff time.</returns>
        public static List<int> getOnlineUsers(HttpContextBase context)
        {
            //Get the current time
            DateTime now = DateTime.Now;
            DateTime cutoff = now.Subtract(new TimeSpan(0, 0, CUTOFF_TIME_SECONDS));

            context.Application.Lock();
            Dictionary<String, DateTime> users = ((Dictionary<String, DateTime>)context.Application["UsersOnline"]);
            context.Application.UnLock();

            List<String> keys = users.Keys.ToList();
            List<int> usersOnline = new List<int>();

            foreach (String key in keys)
            {
                if (users[key] > cutoff)
                {
                    usersOnline.Add(Convert.ToInt32(key));
                }
            }

            return usersOnline;
        }
    }
}