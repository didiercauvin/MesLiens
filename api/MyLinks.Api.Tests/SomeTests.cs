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
            var queryHandler = new Mock<GetAllCategoriesQueryHandler>();
            queryHandler
                .Setup(x => x.Execute())
                .Returns(new Category[0]);

            var queryDispatcher = new QueryDispatcher(queryHandler.Object, null);
            var gateway = new CategoryServiceGateway(queryDispatcher);

            var categories = gateway.GetAllCategories(new GetAllCategoriesQuery());

            Assert.Empty(categories);
        }
        
        [Fact]
        public void ShouldReturnCategories()
        {
            var queryHandler = new Mock<GetAllCategoriesQueryHandler>();
            queryHandler
                .Setup(x => x.Execute())
                .Returns(new Category[] { new Category()});

            var queryDispatcher = new QueryDispatcher(queryHandler.Object, null);
            var gateway = new CategoryServiceGateway(queryDispatcher);

            var categories = gateway.GetAllCategories(new GetAllCategoriesQuery());

            Assert.NotEmpty(categories);
        }

        [Fact]
        public void ShouldReturnCategoryById()
        {
            var queryHandler = new Mock<GetCategoryByIdQueryHandler>();

            var queryDispatcher = new QueryDispatcher(null, queryHandler.Object);

            var gateway = new CategoryServiceGateway(queryDispatcher);

            var category = gateway.GetCategoryById(1);

            Assert.NotNull(category);
        }

    }

    public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, Category>
    {
        public virtual Category Execute()
        {
            throw new NotImplementedException();
        }
    }

    public class GetCategoryByIdQuery
    {
    }

    public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, IEnumerable<Category>>
    {
        public virtual IEnumerable<Category> Execute()
        {
            throw new NotImplementedException();
        }
    }

    public class QueryDispatcher : IQueryDispatcher
    {
        private GetAllCategoriesQueryHandler getAllCategoriesQueryHandler;

        public QueryDispatcher(GetAllCategoriesQueryHandler getAllCategoriesQueryHandler, 
                               GetCategoryByIdQueryHandler getCategoryByIdQueryHandler)
        {
            if (getAllCategoriesQueryHandler == null)
            {
                throw new ArgumentNullException("GetAllCategoriesQueryHandler must not be null");
            }
            if (getCategoryByIdQueryHandler == null)
            {
                throw new ArgumentNullException("GetCategoryByIdQueryHandler must not be null");
            }

            this.getAllCategoriesQueryHandler = getAllCategoriesQueryHandler;
        }

        public IQueryHandler<IQuery, TResult> Dispatch<IQuery, TResult>(IQuery query)
        {
            return (IQueryHandler<IQuery, TResult>)getAllCategoriesQueryHandler;
        }
    }

    public interface IQueryDispatcher
    {
        IQueryHandler<IQuery, TResult> Dispatch<IQuery, TResult>(IQuery query);
    }

    public interface IQueryHandler<IQuery, TResult>
    {
        TResult Execute();
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
            var handler = queryDispatcher.Dispatch<GetAllCategoriesQuery, IEnumerable<Category>>(query);

            return handler.Execute();
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

