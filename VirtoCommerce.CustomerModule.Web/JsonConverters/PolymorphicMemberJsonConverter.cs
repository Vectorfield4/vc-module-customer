﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Web.JsonConverters
{
    /// <summary>
    /// Used to deserialize from JSON derived Member types
    /// </summary>
    public class PolymorphicMemberJsonConverter : JsonConverter
    {

        public PolymorphicMemberJsonConverter()
        {
        }

        public override bool CanWrite { get { return false; } }
        public override bool CanRead { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Member).IsAssignableFrom(objectType) || objectType == typeof(MembersSearchCriteria);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;
            var obj = JObject.Load(reader);
            if (typeof(Member).IsAssignableFrom(objectType))
            {
                var pt = obj["memberType"];
                if (pt == null)
                {
                    throw new ArgumentException("Missing memberType", "memberType");
                }

                var memberType = pt.Value<string>();
                retVal = AbstractTypeFactory<Member>.TryCreateInstance(memberType);
                if (retVal == null)
                {
                    throw new NotSupportedException("Unknown memberType: " + memberType);
                }

            }
            else if (objectType == typeof(MembersSearchCriteria))
            {
                retVal = AbstractTypeFactory<MembersSearchCriteria>.TryCreateInstance();
            }
            serializer.Populate(obj.CreateReader(), retVal);
            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
