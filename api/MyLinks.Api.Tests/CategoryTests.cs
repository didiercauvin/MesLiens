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
            var result = new List<ValidationResult>();
            var command = new CreateCategoryCommand();
            var isValid = Validator.TryValidateProperty(command.Name, new ValidationContext(command) { MemberName = "Name" }, result);

            Assert.False(isValid);
        }

        [Fact(DisplayName = "Clients must specify a GUID in order to create a category")]
        public void MustSpecifyGuid()
        {
            var result = new List<ValidationResult>();
            var command = new CreateCategoryCommand();

            var isValid = Validator.TryValidateProperty(command.Id, new ValidationContext(command) { MemberName = "Id" }, result);

            Assert.False(isValid);
        }

        [Fact]
        public void ShouldFindByName()
        {
            IPersistence persistence = new FakePersistence();

            var tech = new Category(name: "tech");

            persistence.Create(tech);

            Assert.Equal(persistence.FindByName(tech.Name), tech);
        }

        [Fact]
        public void NameMustBeUnique()
        {
            var command1 = new CreateCategoryCommand()
            {
                Name = "name"
            };

            var command2 = new CreateCategoryCommand()
            {
                Name = "name"
            };

            IPersistence persistence = new FakePersistence();
            var commandHandler = new CreateCategoryCommandHandler(persistence);

            commandHandler.Execute(command1);

            Assert.Throws<ArgumentException>(() => commandHandler.Execute(command2));
        }

        // une catégorie doit avoir un nom unique
        // on doit pouvoir rechercher une categorie par son nom
        // on doit pouvoir sauver une catégorie
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
            if (persistence.FindByName(command.Name) != null)
            {
                throw new ArgumentException();
            }

            persistence.Create(new Category(name: command.Name));
        }
    }

    internal class Category
    {
        public Category(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }

    internal class CreateCategoryCommand
    {
        [Required]
        public Guid? Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
