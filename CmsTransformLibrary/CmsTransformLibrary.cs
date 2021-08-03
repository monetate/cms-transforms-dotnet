using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace MonetateLibraries
{
    public static class CmsTransformLibrary
    {
        private static List<String> TRANSFORM_METHODS = new List<String> {
            "addToList",
            "addToObject",
            "insertAfter",
            "moveToIndex",
            "replace",
            "remove"
        };

        /// <summary>
        /// Apply the replace transform to the parsed representation of the JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying a node to replace.</param>
        /// <param name="newValue">The replacement value.</param>
        /// <returns>The transformed JSON.</returns>
        public static JObject ReplacePath<T>(this JToken root, string path, T newValue)
        {
            foreach (var value in root.SelectTokens(path).ToList())
            {
                if (value == root)
                {
                    root = JToken.FromObject(newValue);
                }
                else
                {
                    value.Replace(JToken.FromObject(newValue));
                }
            }

            return (JObject)root;
        }

        /// <summary>
        /// Parse the JSON string and apply the replace transform.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">The JSON string to parse.</param>
        /// <param name="path">The JSON path identifying a node to replace.</param>
        /// <param name="newValue">The replacement value.</param>
        /// <returns>The transformed JSON string.</returns>
        public static string ReplacePath<T>(string jsonString, string path, T newValue)
        {
            return JToken.Parse(jsonString).ReplacePath(path, newValue).ToString();
        }

        /// <summary>
        /// Apply the moveToIndex transform to the parsed representation of the JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying a node to move</param>
        /// <param name="toIndex">To index to which the node should be moved.</param>
        /// <param name="errors">List containing errors from execution</param>
        /// <returns>The transformed JSON.</returns>
        public static JObject MoveToIndex<T>(this JToken root, string path, int toIndex, List<string> errors)
        {
            char[] pathArray = path.ToCharArray();
            Array.Reverse(pathArray);
            if (!Char.IsNumber(pathArray[1]))
            {
                errors.Add(String.Format("Path {0} is not a valid path.", path));
                return (JObject)root;
            }

            JToken node = root.SelectToken(path);
            JToken arr = node.Parent;
            List<JToken> items = arr.ToList();
            try
            {
                int index = items.IndexOf(node);
                items.Insert(toIndex, node);
                items.RemoveAt(index + 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                errors.Add(String.Format("Index, {0}, is out of bounds", toIndex));
                return (JObject)root;
            }

            root.SelectToken(arr.Path).Replace(JArray.FromObject(items));

            return (JObject)root;
        }

        /// <summary>
        /// Parse the JSON string and apply the moveToIndex transform.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">The JSON string to parse.</param>
        /// <param name="path">The JSON path identifying a node to move.</param>
        /// <param name="toIndex">To index to which the node should be moved.</param>
        /// <param name="errors">List containing errors from execution</param>
        /// <returns>The transformed JSON string.</returns>
        public static string MoveToIndex<T>(string jsonString, string path, int toIndex, List<string> errors)
        {
            return JToken.Parse(jsonString).MoveToIndex<T>(path, toIndex, errors).ToString();
        }

        /// <summary>
        /// Insert a new node directly after the node specified by the JSON Path expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying the reference node.</param>
        /// <param name="newValue">The value of the new node to be inserted</param>
        /// <param name="errors">A list of errors.</param>
        /// <returns>The transformed JSON.</returns>
        public static JObject InsertAfter<T>(this JToken root, string path, T newValue, List<string> errors) {
            char[] pathArray = path.ToCharArray();
            Array.Reverse(pathArray);
            if (!Char.IsNumber(pathArray[1]))
            {
                errors.Add(String.Format("Path {0} is not a valid path.", path));
                return (JObject)root;
            }

            JToken node = (JToken)root.SelectToken(path);
            JToken jvalue = JToken.FromObject(newValue);
            node.AddAfterSelf(jvalue);

            return (JObject)root;
        }

        /// <summary>
        /// Insert a new node directly after the node specified by the JSON Path expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying the reference node.</param>
        /// <param name="newValue">The value of the new node to be inserted</param>
        /// <param name="errors">A list of errors.</param>
        /// <returns>The transformed JSON.</returns>
        public static string InsertAfter<T>(string jsonString, string path, T value, List<string> errors) {
            return JToken.Parse(jsonString).InsertAfter<T>(path, value, errors).ToString();
        }

        /// <summary>
        /// Insert a key/value pair into dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying the reference node.</param>
        /// <param name="key">The key for the key/value pair.
        /// <param name="value">The value of the key/value pair</param>
        /// <param name="errors">A list of errors.</param>
        /// <returns>The transformed JSON.</returns>
        public static string AppendToDict<T>(string jsonString, string path, string key, T value, List<string> errors) {
            return JToken.Parse(jsonString).AppendToDict<T>(path, key, value, errors).ToString();
        }

        /// <summary>
        /// Insert key/value pair to a dict.
        /// </summary>
        /// <param name="root">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying the reference node.</param>
        /// <param name="key">The key for the key/value pair to add to dictionary.</param>
        /// <param name="value">The value of the key/value pair to add to dictionary.</param>
        /// <param name="errors">A list of errors during execution time.</param>
        /// <returns>The transformed content.</returns>
        public static JObject AppendToDict<T>(this JToken root, string path, string key, T value, List<string> errors) {
            JObject node = (JObject)root.SelectToken(path);
            JToken jvalue = JToken.FromObject(value);
            node.Add(key, jvalue);
            return (JObject)root;
        }

        /// <summary>
        /// Insert a new node in the the node specified by the JSON Path expression (should be an array).
        /// </summary>
        /// <param name="root">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying the reference node.</param>
        /// <param name="value">The value of the new node to be inserted.</param>
        /// <param name="errors">A list of errors during execution time.</param>
        /// <returns>The transformed content.</returns>
        public static JObject AppendToList<T>(this JToken root, string path, T value, List<string> errors) {
            var node = (JArray)root.SelectToken(path);
            JToken jvalue = JToken.FromObject(value);
            node.Add(jvalue);
            return (JObject)root;
        }

        /// <summary>
        /// Insert a new node in the the node specified by the JSON Path expression (should be an array).
        /// </summary>
        /// <param name="root">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying the reference node.</param>
        /// <param name="value">The value of the new node to be inserted.</param>
        /// <param name="errors">A list of errors during execution time.</param>
        /// <returns>The transformed content.</returns>
        public static string AppendToList<T>(string jsonString, string path, T value, List<string> errors) {
            return JToken.Parse(jsonString).AppendToList<T>(path, value, errors).ToString();
        }

        /// <summary>
        /// Remove the specified node from the deserialized JSON content.
        /// If the specified node is a child of a list item, it will be removed from the list.
        /// Otherwise, the jsonPath is assumed to refer a key/value pair of an object,
        /// and that key/value pair will be removed from the object.  (If the intent is instead
        /// to nullify the value but maintain the key, the 'replace' transform should be used.)
        /// </summary>
        /// <param name="root">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying the reference node.</param>
        /// <param name="errors">A list of errors during execution time.</param>
        /// <returns>The transformed content.</returns>
        public static JObject Remove(this JToken root, string path, List<string> errors) {
            char[] pathArray = path.ToCharArray();
            Array.Reverse(pathArray);
            if (!Char.IsNumber(pathArray[1])) {
                errors.Add(String.Format("No node index specified for removal in path: {0}.", path));
                return (JObject)root;
            }
            var node = root.SelectToken(path);

            node.Remove();
            return (JObject)root;
        }

        /// <summary>
        /// Remove the specified node from the deserialized JSON content.
        /// If the specified node is a child of a list item, it will be removed from the list.
        /// Otherwise, the jsonPath is assumed to refer a key/value pair of an object,
        /// and that key/value pair will be removed from the object.  (If the intent is instead
        /// to nullify the value but maintain the key, the 'replace' transform should be used.)
        /// </summary>
        /// <param name="root">The parsed representation of the JSON.</param>
        /// <param name="path">The JSON path identifying the reference node.</param>
        /// <param name="errors">A list of errors during execution time.</param>
        /// <returns>The transformed content.</returns>
        public static string Remove(string jsonString, string path, List<string> errors) {
            return JToken.Parse(jsonString).Remove(path, errors).ToString();
        }

        /// <summary>
        /// Given JSON content and list of transforms, apply the transforms.
        /// </summary>
        /// <param name="jsonContent">The JSON content</param>
        /// <param name="transforms">The list of transforms to apply.</param>
        /// <returns>The transformed content, along with a list of errors.</returns>
        public static (string, List<string>) ApplyTransforms(string jsonContent, List<Dictionary<string, object>> transforms)
        {
            List<string> errors = new List<string>();
            var removeTransforms = new List<Dictionary<string, object>>();

            JObject parsedContent = JObject.Parse(jsonContent);

            foreach (Dictionary<string, object> transform in transforms)
            {
                var jsonPath = (string)transform["jsonPath"];
                JToken value = parsedContent.SelectToken(jsonPath);

                if (value == null || jsonPath == "") {
                    errors.Add(String.Format("Node at '{0}' does not exist.", jsonPath));
                    continue;
                }

                if (TRANSFORM_METHODS.Contains(transform["method"])) {
                    switch (transform["method"])
                    {
                        case "replace":
                            parsedContent = ReplacePath(parsedContent, jsonPath, transform["value"]);
                            break;
                        case "moveToIndex":
                            try
                            {
                                int transformValue = (int)transform["value"];
                                parsedContent = MoveToIndex<JObject>(parsedContent, jsonPath, transformValue, errors);
                            }
                            catch (System.InvalidCastException) {
                                String error = String.Format("'{0}' is not an integer", transform["value"]);
                                errors.Add(error);
                            }
                            break;
                        case "insertAfter":
                            parsedContent = InsertAfter(parsedContent, jsonPath, transform["value"], errors);
                            break;
                        case "addToObject":
                            try {
                                var key = transform["key"];
                                parsedContent = AppendToDict(parsedContent, jsonPath, (string)key, transform["value"], errors);
                            } catch (Exception ex) {
                                switch (ex) {
                                    case InvalidCastException:
                                        errors.Add(String.Format("Key could not be cast to a string."));
                                        break;
                                    case KeyNotFoundException:
                                        errors.Add("Required field 'key' missing in modifications input");
                                        break;
                                }
                                continue;
                            }
                            break;
                        case "addToList":
                            parsedContent = AppendToList(parsedContent, jsonPath, transform["value"], errors);
                            break;
                        case "remove":
                            removeTransforms.Add(transform);
                            break;
                    }
                } else {
                    errors.Add(String.Format("{0} is not a valid method.", transform["method"]));
                }
            }

            foreach (Dictionary<string, object> removeTransform in removeTransforms) {
                var jsonPath = (string)removeTransform["jsonPath"];
                parsedContent = Remove(parsedContent, jsonPath, errors);
            }
            
            string output = JsonConvert.SerializeObject(parsedContent);
            return (output, errors);
        }
    }
}