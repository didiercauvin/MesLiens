using MyLinks.Application.Contracts;
using MyLinks.Application.Entity;
using MyLinks.Application.Messages.CreateCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MyLinks.Api.Tests
{
    public class CategoryTests
    {
        [Fact(DisplayName = "Category must have a name in order to be created")]
        public void RequiresName()
        {
            var command = new CreateCategoryCommand()
            {
                Id = Guid.NewGuid()
            };

            var handler = new CreateCategoryCommandHandler(new FakePersistence());

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => handler.Execute(command));
        }

        [Fact(DisplayName = "Category must have a GUID in order to be created")]
        public void MustSpecifyGuid()
        {
            var command = new CreateCategoryCommand()
            {
                Name = "name"
            };
            var handler = new CreateCategoryCommandHandler(new FakePersistence());

            var exception = Assert.Throws<ArgumentNullException>(() => handler.Execute(command));
        }

        [Theory(DisplayName = "Persistence must be able to find by name")]
        [InlineData("tech")]
        [InlineData("food")]
        public void ShouldFindByName(string name)
        {
            IPersistence persistence = new FakePersistence();

            var tech = new Category(id: Guid.NewGuid(), name: name);

            persistence.Create(tech);

            Assert.Equal(persistence.FindByName(tech.Name), tech);
        }

        [Fact(DisplayName = "Category name must be unique")]
        public void NameMustBeUnique()
        {
            var command1 = new CreateCategoryCommand()
            {
                Id = Guid.NewGuid(),
                Name = "name"
            };

            var command2 = new CreateCategoryCommand()
            {
                Id = Guid.NewGuid(),
                Name = "name"
            };

            IPersistence persistence = new FakePersistence();
            var commandHandler = new CreateCategoryCommandHandler(persistence);

            commandHandler.Execute(command1);

            Assert.Throws<ArgumentException>(() => commandHandler.Execute(command2));
        }

        [Fact(DisplayName = "Persistence must be able to save category")]
        public void ShouldSaveCategory()
        {
            var command = new CreateCategoryCommand()
            {
                Id = Guid.NewGuid(),
                Name = "name"
            };

            FakePersistence persistence = new FakePersistence();
            var commandHandler = new CreateCategoryCommandHandler(persistence);

            commandHandler.Execute(command);

            var category = persistence.FindByName(command.Name);

            Assert.Equal(command.Id.Value, category.Id);
            Assert.Equal(command.Name, category.Name);

        }
    }

    internal class FakePersistence : IPersistence
    {
        private List<Category> categories { get; set; }

        public FakePersistence()
        {
            categories = new List<Category>();
        }

        public void Create(Category category)
        {
            categories.Add(category);
        }

        public Category FindByName(string name)
        {
            return categories.FirstOrDefault(x => x.Name == name);
        }
    }

}
