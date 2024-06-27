using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace PowerShellRuntimeExtensions
{
    public static class JsonObject
    {
        public static object ConvertFromJson(string input, out ErrorRecord error)
        {
            return JsonObject.ConvertFromJson(input, false, out error);
        }

        public static object ConvertFromJson(string input, bool returnHashtable, out ErrorRecord error)
        {
            return JsonObject.ConvertFromJson(input, returnHashtable, new int?(1024), out error);
        }

        public static object ConvertFromJson(string input, bool returnHashtable, int? maxDepth, out ErrorRecord error)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            error = null;
            object obj2;
            try
            {
                if (Regex.Match(input, "^\\s*\\[").Success)
                {
                    JArray.Parse(input);
                }

                object obj = JsonConvert.DeserializeObject(input, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                    MaxDepth = maxDepth
                });
                JObject jobject = obj as JObject;
                if (jobject == null)
                {
                    JArray jarray = obj as JArray;
                    if (jarray == null)
                    {
                        obj2 = obj;
                    }
                    else
                    {
                        obj2 = (returnHashtable
                            ? JsonObject.PopulateHashTableFromJArray(jarray, out error)
                            : JsonObject.PopulateFromJArray(jarray, out error));
                    }
                }
                else if (returnHashtable)
                {
                    obj2 = JsonObject.PopulateHashTableFromJDictionary(jobject, out error);
                }
                else
                {
                    obj2 = JsonObject.PopulateFromJDictionary(jobject,
                        new JsonObject.DuplicateMemberHashSet(jobject.Count), out error);
                }
            }
            catch (JsonException ex)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Json Deserialisation Failed",
                        new object[] { ex.Message }), ex);
            }

            return obj2;
        }

        private static PSObject PopulateFromJDictionary(JObject entries,
            JsonObject.DuplicateMemberHashSet memberHashTracker, out ErrorRecord error)
        {
            error = null;
            PSObject psobject = new PSObject(entries.Count);
            foreach (KeyValuePair<string, JToken> keyValuePair in entries)
            {
                if (string.IsNullOrEmpty(keyValuePair.Key))
                {
                    string text = string.Format(CultureInfo.CurrentCulture, "Empty key in JSON string", new object[0]);
                    error = new ErrorRecord(new InvalidOperationException(text), "EmptyKeyInJsonString",
                        ErrorCategory.InvalidData, null);
                    return null;
                }

                string text2;
                if (memberHashTracker.TryGetValue(keyValuePair.Key, out text2) &&
                    string.Equals(keyValuePair.Key, text2, StringComparison.Ordinal))
                {
                    string text3 = string.Format(CultureInfo.CurrentCulture, "Duplicate keys in Json string",
                        new object[] { keyValuePair.Key });
                    error = new ErrorRecord(new InvalidOperationException(text3), "DuplicateKeysInJsonString",
                        ErrorCategory.InvalidData, null);
                    return null;
                }

                string text4;
                if (memberHashTracker.TryGetValue(keyValuePair.Key, out text4))
                {
                    string text5 = string.Format(CultureInfo.CurrentCulture, "KeysWithDifferentCasingInJsonString",
                        new object[] { text4, keyValuePair.Key });
                    error = new ErrorRecord(new InvalidOperationException(text5), "KeysWithDifferentCasingInJsonString",
                        ErrorCategory.InvalidData, null);
                    return null;
                }

                JToken value = keyValuePair.Value;
                JArray jarray = value as JArray;
                if (jarray == null)
                {
                    JObject jobject = value as JObject;
                    if (jobject == null)
                    {
                        JValue jvalue = value as JValue;
                        if (jvalue != null)
                        {
                            psobject.Properties.Add(new PSNoteProperty(keyValuePair.Key, jvalue.Value));
                        }
                    }
                    else
                    {
                        PSObject psobject2 = JsonObject.PopulateFromJDictionary(jobject,
                            new JsonObject.DuplicateMemberHashSet(jobject.Count), out error);
                        if (error != null)
                        {
                            return null;
                        }

                        psobject.Properties.Add(new PSNoteProperty(keyValuePair.Key, psobject2));
                    }
                }
                else
                {
                    ICollection<object> collection = JsonObject.PopulateFromJArray(jarray, out error);
                    if (error != null)
                    {
                        return null;
                    }

                    psobject.Properties.Add(new PSNoteProperty(keyValuePair.Key, collection));
                }

                memberHashTracker.Add(keyValuePair.Key, keyValuePair.Key);
            }

            return psobject;
        }

        private static ICollection<object> PopulateFromJArray(JArray list, out ErrorRecord error)
        {
            error = null;
            object[] array = new object[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                JToken jtoken = list[i];
                JArray jarray = jtoken as JArray;
                if (jarray == null)
                {
                    JObject jobject = jtoken as JObject;
                    if (jobject == null)
                    {
                        JValue jvalue = jtoken as JValue;
                        if (jvalue != null)
                        {
                            array[i] = jvalue.Value;
                        }
                    }
                    else
                    {
                        PSObject psobject = JsonObject.PopulateFromJDictionary(jobject,
                            new JsonObject.DuplicateMemberHashSet(jobject.Count), out error);
                        if (error != null)
                        {
                            return null;
                        }

                        array[i] = psobject;
                    }
                }
                else
                {
                    ICollection<object> collection = JsonObject.PopulateFromJArray(jarray, out error);
                    if (error != null)
                    {
                        return null;
                    }

                    array[i] = collection;
                }
            }

            return array;
        }

        private static Hashtable PopulateHashTableFromJDictionary(JObject entries, out ErrorRecord error)
        {
            error = null;
            Hashtable hashtable = new Hashtable(entries.Count);
            foreach (KeyValuePair<string, JToken> keyValuePair in entries)
            {
                if (hashtable.ContainsKey(keyValuePair.Key))
                {
                    string text = string.Format(CultureInfo.CurrentCulture, "DuplicateKeysInJsonString",
                        new object[] { keyValuePair.Key });
                    error = new ErrorRecord(new InvalidOperationException(text), "DuplicateKeysInJsonString",
                        ErrorCategory.InvalidData, null);
                    return null;
                }

                JToken value = keyValuePair.Value;
                JArray jarray = value as JArray;
                if (jarray == null)
                {
                    JObject jobject = value as JObject;
                    if (jobject == null)
                    {
                        JValue jvalue = value as JValue;
                        if (jvalue != null)
                        {
                            hashtable.Add(keyValuePair.Key, jvalue.Value);
                        }
                    }
                    else
                    {
                        Hashtable hashtable2 = JsonObject.PopulateHashTableFromJDictionary(jobject, out error);
                        if (error != null)
                        {
                            return null;
                        }

                        hashtable.Add(keyValuePair.Key, hashtable2);
                    }
                }
                else
                {
                    ICollection<object> collection = JsonObject.PopulateHashTableFromJArray(jarray, out error);
                    if (error != null)
                    {
                        return null;
                    }

                    hashtable.Add(keyValuePair.Key, collection);
                }
            }

            return hashtable;
        }

        private static ICollection<object> PopulateHashTableFromJArray(JArray list, out ErrorRecord error)
        {
            error = null;
            object[] array = new object[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                JToken jtoken = list[i];
                JArray jarray = jtoken as JArray;
                if (jarray == null)
                {
                    JObject jobject = jtoken as JObject;
                    if (jobject == null)
                    {
                        JValue jvalue = jtoken as JValue;
                        if (jvalue != null)
                        {
                            array[i] = jvalue.Value;
                        }
                    }
                    else
                    {
                        Hashtable hashtable = JsonObject.PopulateHashTableFromJDictionary(jobject, out error);
                        if (error != null)
                        {
                            return null;
                        }

                        array[i] = hashtable;
                    }
                }
                else
                {
                    ICollection<object> collection = JsonObject.PopulateHashTableFromJArray(jarray, out error);
                    if (error != null)
                    {
                        return null;
                    }

                    array[i] = collection;
                }
            }

            return array;
        }

        public static string ConvertToJson(object objectToProcess, in ConvertToJsonContext context)
        {
            string text;
            try
            {
                JsonObject._maxDepthWarningWritten = false;
                object obj = JsonObject.ProcessValue(objectToProcess, 0, context);
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    MaxDepth = new int?(1024),
                    StringEscapeHandling = context.StringEscapeHandling
                };
                if (context.EnumsAsStrings)
                {
                    jsonSerializerSettings.Converters.Add(new StringEnumConverter());
                }

                if (!context.CompressOutput)
                {
                    jsonSerializerSettings.Formatting = Formatting.Indented;
                }

                text = JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            }
            catch (OperationCanceledException)
            {
                text = null;
            }

            return text;
        }

        private static object ProcessValue(object obj, int currentDepth, in ConvertToJsonContext context)
        {
            if (obj == null || obj == AutomationNull.Value)
            {
                return null;
            }

            PSObject psobject = obj as PSObject;
            if (psobject != null)
            {
                obj = psobject.BaseObject;
            }

            bool flag = false;
            bool flag2 = false;
            object obj2;
            if (obj == null || obj == DBNull.Value)
            {
                obj2 = null;
            }
            else if (obj is string || obj is char || obj is bool || obj is DateTime || obj is DateTimeOffset ||
                     obj is Guid || obj is Uri || obj is double || obj is float || obj is decimal)
            {
                obj2 = obj;
            }
            else
            {
                JObject jobject = obj as JObject;
                if (jobject != null)
                {
                    obj2 = jobject.ToObject<Dictionary<object, object>>();
                }
                else
                {
                    Type type = obj.GetType();
                    if (type.IsPrimitive)
                    {
                        obj2 = obj;
                    }
                    else if (type.IsEnum)
                    {
                        Type underlyingType = Enum.GetUnderlyingType(obj.GetType());
                        if (underlyingType.Equals(typeof(long)) || underlyingType.Equals(typeof(ulong)))
                        {
                            obj2 = obj.ToString();
                        }
                        else
                        {
                            obj2 = obj;
                        }
                    }
                    else if (currentDepth > context.MaxDepth)
                    {
                        if (psobject != null && psobject.BaseObject is PSCustomObject)
                        {
                            obj2 = LanguagePrimitives.ConvertTo(psobject, typeof(string), CultureInfo.InvariantCulture);
                            flag = true;
                        }
                        else
                        {
                            obj2 = LanguagePrimitives.ConvertTo(obj, typeof(string), CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        IDictionary dictionary = obj as IDictionary;
                        if (dictionary != null)
                        {
                            obj2 = JsonObject.ProcessDictionary(dictionary, currentDepth, context);
                        }
                        else
                        {
                            IEnumerable enumerable = obj as IEnumerable;
                            if (enumerable != null)
                            {
                                obj2 = JsonObject.ProcessEnumerable(enumerable, currentDepth, context);
                            }
                            else
                            {
                                obj2 = JsonObject.ProcessCustomObject<JsonIgnoreAttribute>(obj, currentDepth, context);
                                flag2 = true;
                            }
                        }
                    }
                }
            }

            return JsonObject.AddPsProperties(psobject, obj2, currentDepth, flag, flag2, context);
        }

        private static object AddPsProperties(object psObj, object obj, int depth, bool isPurePSObj, bool isCustomObj,
            in ConvertToJsonContext context)
        {
            PSObject psobject = psObj as PSObject;
            if (psobject == null)
            {
                return obj;
            }

            if (isPurePSObj)
            {
                return obj;
            }

            bool flag = true;
            IDictionary dictionary = obj as IDictionary;
            if (dictionary == null)
            {
                flag = false;
                dictionary = new Dictionary<string, object>();
                dictionary.Add("value", obj);
            }

            JsonObject.AppendPsProperties(psobject, dictionary, depth, isCustomObj, context);
            if (!flag && dictionary.Count == 1)
            {
                return obj;
            }

            return dictionary;
        }

        private static void AppendPsProperties(PSObject psObj, IDictionary receiver, int depth, bool isCustomObject,
            in ConvertToJsonContext context)
        {
            if (psObj.BaseObject is string || psObj.BaseObject is DateTime)
            {
                return;
            }

            foreach (PSPropertyInfo pspropertyInfo in psObj.Properties)
            {
                object obj = null;
                try
                {
                    obj = pspropertyInfo.Value;
                }
                catch (Exception)
                {
                }

                if (!receiver.Contains(pspropertyInfo.Name))
                {
                    receiver[pspropertyInfo.Name] = JsonObject.ProcessValue(obj, depth + 1, context);
                }
            }
        }

        private static object ProcessDictionary(IDictionary dict, int depth, in ConvertToJsonContext context)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>(dict.Count);
            foreach (object obj in dict)
            {
                DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
                string text = dictionaryEntry.Key as string;
                if (text == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        "NonStringKeyInDictionary", new object[] { dict.GetType().FullName }));
                }

                dictionary.Add(text, JsonObject.ProcessValue(dictionaryEntry.Value, depth + 1, context));
            }

            return dictionary;
        }

        private static object ProcessEnumerable(IEnumerable enumerable, int depth, in ConvertToJsonContext context)
        {
            List<object> list = new List<object>();
            foreach (object obj in enumerable)
            {
                list.Add(JsonObject.ProcessValue(obj, depth + 1, context));
            }

            return list;
        }

        private static object ProcessCustomObject<T>(object o, int depth, in ConvertToJsonContext context)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            Type type = o.GetType();
            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!fieldInfo.IsDefined(typeof(T), true))
                {
                    object obj;
                    try
                    {
                        obj = fieldInfo.GetValue(o);
                    }
                    catch (Exception)
                    {
                        obj = null;
                    }

                    dictionary.Add(fieldInfo.Name, JsonObject.ProcessValue(obj, depth + 1, context));
                }
            }

            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!propertyInfo.IsDefined(typeof(T), true))
                {
                    MethodInfo getMethod = propertyInfo.GetGetMethod();
                    if (getMethod != null && getMethod.GetParameters().Length == 0)
                    {
                        object obj2;
                        try
                        {
                            obj2 = getMethod.Invoke(o, new object[0]);
                        }
                        catch (Exception)
                        {
                            obj2 = null;
                        }

                        dictionary.Add(propertyInfo.Name, JsonObject.ProcessValue(obj2, depth + 1, context));
                    }
                }
            }

            return dictionary;
        }

        private static bool _maxDepthWarningWritten;

        private sealed class DuplicateMemberHashSet : Dictionary<string, string>
        {
            public DuplicateMemberHashSet(int capacity)
                : base(capacity, StringComparer.OrdinalIgnoreCase)
            {
            }
        }
    }
}