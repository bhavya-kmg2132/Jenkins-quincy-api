using System;

namespace Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InvalidateCacheAttribute : Attribute
    {
        public Type[] Queries;

        public InvalidateCacheAttribute(params Type[] queriesTypes)
        {
            Queries = queriesTypes;
        }
    }
}