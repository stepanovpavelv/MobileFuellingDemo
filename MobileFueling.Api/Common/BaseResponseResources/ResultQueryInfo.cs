using Microsoft.Extensions.Localization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MobileFueling.Api.Common.BaseResponseResources
{
    public class ResultQueryInfo : IResultQueryInfo
    {
        private readonly ConcurrentBag<Message> _messages;

        public ICollection<Message> Messages
        {
            get { return _messages.ToList(); }
        }
        
        public ResultQueryInfo()
        {
            _messages = new ConcurrentBag<Message>();
        }

        public void AddMessage(MessageType type, LocalizedString text)
        {
            _messages.Add(new Message
            {
                Text = text,
                Type = type
            });
        }

        public void AddException(Exception ex)
        {
            _messages.Add(new Message
            {
                Text = new LocalizedString("system exception", ex?.ToString()),
                Type = MessageType.ERROR
            });
        }

        public bool HasError
        {
            get
            {
                if (_messages != null && _messages.Any())
                {
                    return _messages.Any(x => x.Type == MessageType.ERROR);
                }

                return false;
            }
        }
    }
}