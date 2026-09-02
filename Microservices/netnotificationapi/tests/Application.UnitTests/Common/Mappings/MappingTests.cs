using AutoMapper;
using Application.Common.Mappings;
using Application.Users.Queries;
using Application.Users.Queries.Permission;
using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;

namespace Application.UnitTests.Common.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [TestCase(typeof(Domain.Entities.User), typeof(UserDto))]
        [TestCase(typeof(Domain.Entities.Permission), typeof(PermissionDto))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            var instance = GetInstanceOf(source);

            _mapper.Map(instance, source, destination);
        }

        private static object GetInstanceOf(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance(type);

            return RuntimeHelpers.GetUninitializedObject(type);
        }
    }
}
