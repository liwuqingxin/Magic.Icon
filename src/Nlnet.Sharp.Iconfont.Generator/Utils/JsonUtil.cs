using System;
using System.IO;
using Newtonsoft.Json;

namespace Nlnet.Sharp
{
    internal static class JsonUtil
    {
        #region NSJ:支持抽象类和接口

        /// <summary>
        /// 序列化（带类名）。（NewtonSoftJson）
        /// </summary>
        /// <returns>json文本</returns>
        public static string SerializeWithTypeNameHandlingByNsj<T>(this T t)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            return JsonConvert.SerializeObject(t, Formatting.Indented, settings);
        }

        /// <summary>
        /// 反序列化（带类名）。（NewtonSoftJson）
        /// </summary>
        /// <returns>json文本</returns>
        public static T DeserializeWithTypeNameHandlingByNsj<T>(this string json)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// 序列化（带类名）。（NewtonSoftJson）
        /// </summary>
        /// <returns>json文本</returns>
        public static string SerializeWithTypeNameHandlingByNsj<T>(this T t, JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
            }
            return JsonConvert.SerializeObject(t, Formatting.Indented, settings);
        }

        /// <summary>
        /// 反序列化（带类名）。（NewtonSoftJson）
        /// </summary>
        /// <returns>json文本</returns>
        public static T DeserializeWithTypeNameHandlingByNsj<T>(this string json, JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
            }
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        #endregion



        #region NSJ:普通格式化的序列化和反序列化

        /// <summary>
        /// 序列化。（NewtonSoftJson）
        /// </summary>
        /// <returns>json文本</returns>
        public static string SerializeByNsj<T>(this T t)
        {
            var serializer = new JsonSerializer();
            using (var textWriter = new StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(textWriter))
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jsonWriter.Indentation = 4;
                    jsonWriter.IndentChar = ' ';
                    serializer.Serialize(jsonWriter, t);
                    return textWriter.ToString();
                }
            }
        }

        /// <summary>
        /// 反序列化。（NewtonSoftJson）
        /// </summary>
        /// <param name="jsonStr">json文本</param>
        /// <returns>对象</returns>
        public static T DeserializeByNsj<T>(this string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        /// <summary>
        /// 反序列化。（NewtonSoftJson）
        /// </summary>
        /// <param name="jsonStr">json文本</param>
        /// <param name="type"></param>
        /// <returns>对象</returns>
        public static object DeserializeByNsj(this string jsonStr, Type type)
        {
            return JsonConvert.DeserializeObject(jsonStr, type);
        }

        /// <summary>
        /// 反序列化，在回调中处理异常，回调为空则抛出异常。（NewtonSoftJson）
        /// </summary>
        /// <param name="jsonStr">json文本</param>
        /// <param name="errorHandle">异常处理回调。如果为空，则抛出异常</param>
        /// <returns>对象</returns>
        public static T TryDeserializeByNsj<T>(this string jsonStr, EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> errorHandle = null) where T : class
        {
            var settings = new JsonSerializerSettings
            {
                Error = errorHandle,
            };

            return JsonConvert.DeserializeObject<T>(jsonStr, settings);
        }

        /// <summary>
        /// 反序列化，在回调中处理异常，回调为空则抛出异常。（NewtonSoftJson）
        /// </summary>
        /// <param name="jsonStr">json文本</param>
        /// <param name="type"></param>
        /// <param name="errorHandle">异常处理回调。如果为空，则抛出异常</param>
        /// <returns>对象</returns>
        public static object TryDeserializeByNsj(this string jsonStr, Type type, EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> errorHandle = null)
        {
            var settings = new JsonSerializerSettings
            {
                Error = errorHandle,
            };

            return JsonConvert.DeserializeObject(jsonStr, type, settings);
        }

        #endregion
    }
}
