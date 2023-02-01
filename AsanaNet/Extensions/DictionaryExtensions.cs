using System;
using System.Collections.Generic;
using System.Linq;

namespace AsanaNet.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, object> WhereContainsAnyAssignee(this Dictionary<string, object> dictionary, IEnumerable<AsanaUser> asanaUsers)
        {
            if (asanaUsers == null) 
                return dictionary;

            var users = asanaUsers.ToList();
            if (!users.Any()) 
                return dictionary;

            var ids = users.Select(x => x.ID.ToString());
            dictionary.Add("assignee.any", string.Join(",", ids));

            return dictionary;
        }

        public static Dictionary<string, object> WhereContainsAnyFollower(this Dictionary<string, object> dictionary, IEnumerable<AsanaUser> asanaUsers)
        {
            if (asanaUsers == null) 
                return dictionary;

            var users = asanaUsers.ToList();
            if (!users.Any()) 
                return dictionary;

            var userIds = users.Select(x => x.ID.ToString());
            dictionary.Add("followers.any", string.Join(",", userIds));

            return dictionary;
        }

        public static Dictionary<string, object> WhereStartsBefore(this Dictionary<string, object> dictionary, DateTime date)
        {
            dictionary.Add("start_on.before", date.ToString("yyyy-MM-dd"));

            return dictionary;
        }       
        public static Dictionary<string, object> WhereStartsAfter(this Dictionary<string, object> dictionary, DateTime date)
        {
            dictionary.Add("start_on.after", date.ToString("yyyy-MM-dd"));

            return dictionary;
        }


        public static Dictionary<string, object> WhereEndsBefore(this Dictionary<string, object> dictionary, DateTime date)
        {
            dictionary.Add("due_at.before", date.ToString("yyyy-MM-dd"));

            return dictionary;
        }
        public static Dictionary<string, object> WhereEndsAfter(this Dictionary<string, object> dictionary, DateTime date)
        {
            dictionary.Add("due_at.after", date.ToString("yyyy-MM-dd"));

            return dictionary;
        }

        public static Dictionary<string, object> WhereInRange(this Dictionary<string, object> dictionary, DateTime start, DateTime end)
        {
            dictionary.Add("start_on.after", start.ToString("yyyy-MM-dd"));
            dictionary.Add("due_at.before", end.ToString("yyyy-MM-dd"));

            return dictionary;
        }
        public static Dictionary<string, object> WhereIntersectRange(this Dictionary<string, object> dictionary, DateTime start, DateTime end)
        {
            dictionary.Add("start_on.before", end.ToString("yyyy-MM-dd"));
            dictionary.Add("due_at.after", start.ToString("yyyy-MM-dd"));

            return dictionary;
        }

        public static Dictionary<string, object> AddLimit(this Dictionary<string, object> dictionary, uint limit)
        {
            dictionary.Add("limit", limit.ToString());
            return dictionary;
        }
    }
}