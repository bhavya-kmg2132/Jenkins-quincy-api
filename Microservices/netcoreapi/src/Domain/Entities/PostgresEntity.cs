using System;

namespace Domain.Entities
{
    public class PostgresEntity
    {
        public Guid Id { get; set; }
        public string EntityJson { get; set; }
    }
}