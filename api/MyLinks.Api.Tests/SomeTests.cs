using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace MyLinks.Api.Tests
{
    public class SomeTests
    {
        [Fact]
        public void ShouldReturnEmptyArrayOfCategories()
        {
            var services = new ServiceCollection();
            
            var queryHandler = new Mock<GetAllCategoriesQueryHandler>();
            queryHandler
                .Setup(x => x.Execute(It.IsAny<GetAllCategoriesQuery>()))
                .Returns(new Category[0]);

            services.AddScoped(typeof(IQueryHandler<GetAllCategoriesQuery, IEnumerable<Category>>), typeof(GetAllCategoriesQueryHandler));

            var serviceProvider = services.BuildServiceProvider();

            var queryDispatcher = new QueryDispatcher(serviceProvider);
            var gateway = new CategoryServiceGateway(queryDispatcher);

            var categories = gateway.GetAllCategories(new GetAllCategoriesQuery());

            Assert.Empty(categories);
        }
        
        //[Fact]
        //public void ShouldReturnCategories()
        //{
        //    var queryHandler = new Mock<GetAllCategoriesQueryHandler>();
        //    queryHandler
        //        .Setup(x => x.Execute())
        //        .Returns(new Category[] { new Category()});

        //    var queryDispatcher = new QueryDispatcher(queryHandler.Object, null);
        //    var gateway = new CategoryServiceGateway(queryDispatcher);

        //    var categories = gateway.GetAllCategories(new GetAllCategoriesQuery());

        //    Assert.NotEmpty(categories);
        //}

        //[Fact]
        //public void ShouldReturnCategoryById()
        //{
        //    var queryHandler = new Mock<GetCategoryByIdQueryHandler>();

        //    var queryDispatcher = new QueryDispatcher(null, queryHandler.Object);

        //    var gateway = new CategoryServiceGateway(queryDispatcher);

        //    var category = gateway.GetCategoryById(1);

        //    Assert.NotNull(category);
        //}

    }
    
    public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, Category>
    {
        public virtual Category Execute(GetCategoryByIdQuery query)
        {
            throw new NotImplementedException();
        }
    }

    public class GetCategoryByIdQuery : IQuery<Category>
    {
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
        //private GetAllCategoriesQueryHandler getAllCategoriesQueryHandler;

        public QueryDispatcher(IServiceProvider provider)
        {
            //if (getAllCategoriesQueryHandler == null)
            //{
            //    throw new ArgumentNullException("GetAllCategoriesQueryHandler must not be null");
            //}
            //if (getCategoryByIdQueryHandler == null)
            //{
            //    throw new ArgumentNullException("GetCategoryByIdQueryHandler must not be null");
            //}

            //this.getAllCategoriesQueryHandler = getAllCategoriesQueryHandler;
            this.provider = provider;
        }

        public TResult Execute<IQuery, TResult>(IQuery query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            var handler = (dynamic)provider.GetService(handlerType);
            return handler.Execute(query);
        }
    }

    public interface IQueryDispatcher
    {
        TResult Execute<IQuery, TResult>(IQuery query);
    }

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
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
            return queryDispatcher.Execute<GetAllCategoriesQuery, IEnumerable<Category>>(query);
        }

        internal object GetCategoryById(int v)
        {
            throw new NotImplementedException();
        }
    }

    public class GetAllCategoriesQuery : IQuery<IEnumerable<Category>>
    {
    }

    public class Category
    {
    }
    
}

