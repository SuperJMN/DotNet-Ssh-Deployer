using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace DotNetSsh.UserSecrets
{
    public class JsonFlattener
    {
        private enum JSONType
        {
            OBJECT, ARRAY
        }
        public static Dictionary<string, string> Flatten(JObject jsonObject)
        {
            IEnumerable<JToken> jTokens = jsonObject.Descendants().Where(p => p.Count() == 0);
            Dictionary<string, string> results = jTokens.Aggregate(new Dictionary<string, string>(), (properties, jToken) =>
            {
                properties.Add(jToken.Path, jToken.ToString());
                return properties;
            });
            return results;
        }

        public static JObject Unflatten(IDictionary<string, string> keyValues)
        {
            JContainer result = null;
            JsonMergeSettings setting = new JsonMergeSettings();
            setting.MergeArrayHandling = MergeArrayHandling.Merge;
            foreach (var pathValue in keyValues)
            {
                if (result == null)
                {
                    result = UnflatenSingle(pathValue);
                }
                else
                {
                    result.Merge(UnflatenSingle(pathValue), setting);
                }
            }
            return result as JObject;
        }

        private static JContainer UnflatenSingle(KeyValuePair<string, string> keyValue)
        {
            string path = keyValue.Key;
            string value = keyValue.Value;
            var pathSegments = SplitPath(path);

            JContainer lastItem = null;
            //build from leaf to root
            foreach (var pathSegment in pathSegments.Reverse())
            {
                var type = GetJSONType(pathSegment);
                switch (type)
                {
                    case JSONType.OBJECT:
                        var obj = new JObject();
                        if (null == lastItem)
                        {
                            obj.Add(pathSegment, value);
                        }
                        else
                        {
                            obj.Add(pathSegment, lastItem);
                        }
                        lastItem = obj;
                        break;
                    case JSONType.ARRAY:
                        var array = new JArray();
                        int index = GetArrayIndex(pathSegment);
                        array = FillEmpty(array, index);
                        if (lastItem == null)
                        {
                            array[index] = value;
                        }
                        else
                        {
                            array[index] = lastItem;
                        }
                        lastItem = array;
                        break;
                }
            }
            return lastItem;
        }

        public static IList<string> SplitPath(string path)
        {
            IList<string> result = new List<string>();
            Regex reg = new Regex(@"(?!\.:)([^. :^\[\]]+)|(?!\[)(\d+)(?=\])");
            foreach (Match match in reg.Matches(path))
            {
                result.Add(match.Value);
            }
            return result;
        }

        private static JArray FillEmpty(JArray array, int index)
        {
            for (int i = 0; i <= index; i++)
            {
                array.Add(null);
            }
            return array;
        }

        private static JSONType GetJSONType(string pathSegment)
        {
            int x;
            return int.TryParse(pathSegment, out x) ? JSONType.ARRAY : JSONType.OBJECT;
        }

        private static int GetArrayIndex(string pathSegment)
        {
            int result;
            if (int.TryParse(pathSegment, out result))
            {
                return result;
            }
            throw new Exception("Unable to parse array index: " + pathSegment);
        }

    }
}