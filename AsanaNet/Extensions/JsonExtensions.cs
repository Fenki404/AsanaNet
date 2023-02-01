using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace AsanaNet.Extensions
{
    public static class JsonExtensions
    {
        public static void FixElementArrays(this JToken root)
        {
            var regex = new Regex("^(.+)\\[[0-9]+\\]$");
            if (root is JContainer container)
            {
                var query =
                    from o in container.DescendantsAndSelf().OfType<JObject>()
                    let matches = o.Properties()
                        .Select(p => (Property: p, Match: regex.Match(p.Name)))
                        .Where(m => m.Match.Success)
                        .Select(m => (m.Property, Name: m.Match.Groups[1].Value))
                    let groups = matches.GroupBy(m => m.Name)
                    from g in groups
                    select (Object: o, Name: g.Key, Values: g.Select(m => m.Property.Value).ToList());
                foreach (var g in query.ToList())
                {
                    IList<JToken> objAsList = g.Object;
                    // DescendantsAndSelf() returns items in document order, which ordering is preserved by GroupBy, so index of first item should be first index.
                    var insertIndex = objAsList.IndexOf(g.Values[0].Parent);
                    g.Values.ForEach(v => v.RemoveFromLowestPossibleParent());
                    objAsList.Insert(insertIndex, new JProperty(g.Name, new JArray(g.Values)));
                }
            }
        }

        public static JToken RemoveFromLowestPossibleParent(this JToken node)
        {
            if (node == null)
                return null;
            // If the parent is a JProperty, remove that instead of the token itself.
            var property = node.Parent as JProperty;
            var contained = property ?? node;
            if (contained.Parent != null)
                contained.Remove();
            // Also detach the node from its immediate containing property -- Remove() does not do this even though it seems like it should
            if (property != null)
                property.Value = null;
            return node;
        }
    }
}