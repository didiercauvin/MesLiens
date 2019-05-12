using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MyLinks.Api.Tests
{
    public class SomeTests
    {
        [Fact]
        public void ShouldReturnEmptyArrayOfCategories()
        {
            //var queryHandler = new Mock<GetAllCategoriesQueryHandler>();
            //queryHandler
            //    .Setup(x => x.Execute())
            //    .Returns(new Category[0]);

            var services = new ServiceCollection();
            services.AddScoped(typeof(IQueryHandler<GetAllCategoriesQuery,IEnumerable<Category>>), typeof(GetAllCategoriesQueryHandler));

            var service = services.BuildServiceProvider();
            var queryDispatcher = new QueryDispatcher(service);
            var gateway = new CategoryServiceGateway(queryDispatcher);

            var categories = gateway.GetAllCategories(new GetAllCategoriesQuery());

            Assert.Empty(categories);
        }
        
    }

    public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, IEnumerable<Category>>
    {
        public virtual IEnumerable<Category> Execute(GetAllCategoriesQuery query)
        {
            return new Category[0];
        }
    }

    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider provider;

        public QueryDispatcher(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = provider.GetService(handlerType);
            return handler.Execute((dynamic)query);
        }
    }

    public interface IQueryDispatcher
    {
        TResult Execute<TResult>(IQuery<TResult> query);
    }

    public interface IQueryHandler<TQuery, TResult> where TQuery: IQuery<TResult>
    {
        TResult Execute(TQuery query);
    }

    public interface IQuery<TResult>
    {
    }
    

    public class CategoryServiceGateway
    {
        private IQueryDispatcher queryDispatcher;

        public CategoryServiceGateway(IQueryDispatcher queryDispatcher)
        {
            this.queryDispatcher = queryDispatcher;
        }

        public IEnumerable<Category> GetAllCategories(GetAllCategoriesQuery query)
        {
            return queryDispatcher.Execute(query);
        }
    }

    public class GetAllCategoriesQuery : IQuery<IEnumerable<Category>>
    {
    }

    public class Category
    {
    }
    
}

