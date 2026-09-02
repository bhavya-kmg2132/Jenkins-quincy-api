using System;
using System.Collections.Concurrent;
using Application.Common.Interfaces;
using Domain.Dto;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class AutoLoginStore : IAutoLoginStore
    {
        private readonly ConcurrentDictionary<string, AutoLoginRecord> _store = new();
        private readonly int _expiryMinutes;

        public AutoLoginStore(IConfiguration configuration)
        {
            _expiryMinutes = configuration.GetValue<int>("AutoLogin:KeyExpiryMinutes", 30);
        }

        public string MintKey(string userId)
        {
            var key = Guid.NewGuid().ToString("N");
            _store[key] = new AutoLoginRecord
            {
                UserId = userId,
                Expiry = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                IsUsed = false
            };
            return key;
        }

        public AutoLoginRecord ValidateAndConsume(string key)
        {
            if (!_store.TryGetValue(key, out var record))
                return null;

            if (record.Expiry < DateTime.UtcNow)
            {
                _store.TryRemove(key, out _);
                return null;
            }

            _store.TryRemove(key, out _);
            return record;
        }
    }
}

