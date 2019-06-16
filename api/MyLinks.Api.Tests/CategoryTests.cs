using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
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

    internal interface IPersistence
    {
        void Create(Category category);
        Category FindByName(string name);
    }

    internal class CreateCategoryCommandHandler
    {
        private readonly IPersistence persistence;

        public CreateCategoryCommandHandler(IPersistence persistence)
        {
            this.persistence = persistence;
        }

        internal void Execute(CreateCategoryCommand command)
        {
            if (!command.Id.HasValue)
            {
                throw new ArgumentNullException("Id", "Category must have a id");
            }

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                throw new ArgumentOutOfRangeException("Name", "Category must have a name");
            }

            if (persistence.FindByName(command.Name) != null)
            {
                throw new ArgumentException();
            }

            persistence.Create(new Category(id: command.Id.Value, name: command.Name));
        }
    }

    internal class Category
    {
        public Category(Guid id, string name)
        {
            Id = id;
            this.Name = name;
        }

        public string Name { get; }
        public Guid Id { get; }
    }

    internal class CreateCategoryCommand
    {
        [Required]
        public Guid? Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
