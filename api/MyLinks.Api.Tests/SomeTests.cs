using Microsoft.Extensions.DependencyInjection;
using System;
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
            var services = new ServiceCollection();
            services.AddScoped(typeof(IQueryHandler<GetAllCategoriesQuery, PagedResult<Category>>), typeof(GetAllCategoriesQueryHandler));

            var service = services.BuildServiceProvider();
            var queryDispatcher = new QueryDispatcher(service);

            var categories = queryDispatcher.Execute(new GetAllCategoriesQuery());

            Assert.Empty(categories.Items);
        }

        [Fact]
        public void ShouldReturnPagedResults()
        {
            var handler = new GetAllCategoriesQueryHandler();

            var pagingInfo = new PagingInformation()
            {
                PageIndex = 0,
                PageSize = 1
            };

            var query = new GetAllCategoriesQuery()
            {
                PagingInformation = pagingInfo
            };

            var pagedResult = handler.Execute(query);

            Assert.Equal(pagingInfo, pagedResult.PagingInfo);
            Assert.Single(pagedResult.Items);
        }

    }

    public class PagingInformation
    {
        /// <summary>
        /// Combien d'éléments je veux dans mon retour
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// A quelle page je veux être
        /// </summary>
        public int PageIndex { get; set; }
    }

    public class PagedResult<T>
    {
        public PagingInformation PagingInfo { get; set; }
        public IEnumerable<T> Items { get; internal set; }

        public static PagedResult<T> ApplyPaging(IQueryable<T> result, PagingInformation pagingInfo)
        {
            var items = result.Skip(pagingInfo.PageSize * pagingInfo.PageIndex).Take(pagingInfo.PageSize).ToList();
            return new PagedResult<T>
            {
                PagingInfo = pagingInfo,
                Items = items
            };
        }
    }

    public class GetAllCategoriesQuery : IQuery<PagedResult<Category>>
    {
        public PagingInformation PagingInformation { get; set; }
    }

    public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, PagedResult<Category>>
    {
        public PagedResult<Category> Execute(GetAllCategoriesQuery query)
        {
            var dataset = new DbCategory[]
            {
                new DbCategory { Id = 1 },
                new DbCategory { Id = 2 },
                new DbCategory { Id = 3 }
            };

            var result = dataset.Select(x => new Category { Id = x.Id }).AsQueryable();

            return PagedResult<Category>.ApplyPaging(result, query.PagingInformation);
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

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TResult Execute(TQuery query);
    }

    public interface IQuery<TResult>
    {
    }

    public class Category
    {
        public int Id { get; internal set; }
    }

    public class DbCategory
    {
        public int Id { get; set; }
    }

}

