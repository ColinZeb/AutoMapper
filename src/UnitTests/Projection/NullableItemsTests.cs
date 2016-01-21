﻿using System;

namespace AutoMapper.UnitTests.Projection
{
    namespace NullableItemsTests
    {
        using System.Linq;
        using QueryableExtensions;
        using Should;
        using Should.Core.Assertions;
        using Xunit;

        public class NullChildItemTest
        {
            private IExpressionBuilder _builder;

            public NullChildItemTest()
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<Parent, ParentDto>());
                _builder = config.CreateExpressionBuilder();
            }

            [Fact]
            public void Should_project_null_value()
            {
                var items = new[]
                {
                    new Parent
                    {
                        Value = 5
                    }
                };

                var projected = items.AsQueryable().ProjectTo<ParentDto>(_builder).ToList();

                projected[0].Value.ShouldEqual(5);
                projected[0].ChildValue.ShouldBeNull();
                projected[0].ChildGrandChildValue.ShouldBeNull();
            }


            public class ParentDto
            {
                public int? Value { get; set; }
                public int? ChildValue { get; set; }
                public int? ChildGrandChildValue { get; set; }
                public DateTime? Date { get; set; }
            }


            public class Parent
            {
                public int Value { get; set; }
                public Child Child { get; set; }
            }

            public class Child
            {
                public int Value { get; set; }
                public GrandChild GrandChild { get; set; }
            }

            public class GrandChild
            {
                public int Value { get; set; }
            }
        }

        public class CustomMapFromTest
        {
            private IExpressionBuilder _builder;

            public class Parent
            {
                public int Value { get; set; }
                
            }

            public class ParentDto
            {
                public int? Value { get; set; }
                public DateTime? Date { get; set; }
            }
            public CustomMapFromTest()
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<Parent, ParentDto>()
                    .ForMember(dto => dto.Date, opt => opt.MapFrom(src => DateTime.UtcNow)));
                _builder = config.CreateExpressionBuilder();
            }

            [Fact]
            public void Should_not_fail()
            {
                var items = new[]
                {
                    new Parent
                    {
                        Value = 5
                    }
                };

                var projected = items.AsQueryable().ProjectTo<ParentDto>(_builder).ToList();

                typeof(NullReferenceException).ShouldNotBeThrownBy(() => items.AsQueryable().ProjectTo<ParentDto>(_builder).ToList());
                Assert.NotNull(projected[0].Date);
            }
        }
    }
}