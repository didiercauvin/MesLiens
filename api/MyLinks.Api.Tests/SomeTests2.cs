using Moq;
using System.Collections.Generic;
using Xunit;

namespace MyLinks.Api.Tests.Bis
{
    public class SomeTests2
    {
        [Fact]
        public void ShouldReturnEmptyArrayOfCategories()
        {
            var persistence = new Mock<ICategoryPersistence>();
            persistence
                .Setup(x => x.GetAll())
                .Returns(new Category[0]);

            var query = new GetAllCategoriesQueryHandler(persistence.Object);
            var categories = query.Execute(new GetAllCategoriesQueryParameter());

            Assert.Empty(categories);
        }
        [Fact]
        public void ShouldReturnCategoriesIfAny()
        {
            var persistence = new Mock<ICategoryPersistence>();
            persistence
                .Setup(x => x.GetAll())
                .Returns(new Category[] { new Category() });

            var query = new GetAllCategoriesQueryHandler(persistence.Object);
            var categories = query.Execute(new GetAllCategoriesQueryParameter());

            Assert.NotEmpty(categories);
        }

        //[Fact]
        //public void ShouldCreateCategory()
        //{
        //    var command = new CreateCategoryCommand
        //}

    }

    public interface ICategoryPersistence
    {
        IEnumerable<Category> GetAll();
    }

    // Permet d'étendre l'implémentation des QueryHandlers  pour y ajouter des fonctionnalités
    // comme les logs, les timers, etc (cross cutting concerns)
    // Ainsi, un controller ou tout autre consommateur de query handler aura simplement à spécifier
    // dans sa construction une abstraction de query handler (ie IQueryHandler)
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        TResult Execute(TQuery query);
    }


    public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQueryParameter, IEnumerable<Category>>
    {
        private readonly ICategoryPersistence persistence;

        public GetAllCategoriesQueryHandler(ICategoryPersistence context)
        {
            this.persistence = context;
        }

        public IEnumerable<Category> Execute(GetAllCategoriesQueryParameter query)
        {
            // Exploiter les paramètres de la query et passer par la persistence pour
            // effectuer la requete
            return persistence.GetAll();
        }
    }

    public interface IQuery<TResult>
    {
    }

    public class GetAllCategoriesQueryParameter : IQuery<IEnumerable<Category>>
    {
    }

    public class Category
    {
    }

}

