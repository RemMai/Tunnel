﻿using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Libs.JsonConverters
{
    /// <summary>
    ///  
    /// </summary>
    public sealed class IpAddressJsonConverter : JsonConverter<IPAddress>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IPAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return IPAddress.Parse(reader.GetString() ?? string.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
