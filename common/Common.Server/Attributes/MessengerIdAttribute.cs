﻿using System;

namespace Common.Server.Attributes
{
    /// <summary>
    /// 消息id
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MessengerIdAttribute : Attribute
    {
        /// <summary>
        /// id
        /// </summary>
        public ushort Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public MessengerIdAttribute(ushort id)
        {
            Id = id;
        }
    }

}