using MyLinks.Application.Contracts;
using MyLinks.Application.Entity;
using System;

namespace MyLinks.Application.Messages.CreateCategory
{
    public class CreateCategoryCommandHandler
    {
        private readonly IPersistence persistence;

        public CreateCategoryCommandHandler(IPersistence persistence)
        {
            this.persistence = persistence;
        }

        public void Execute(CreateCategoryCommand command)
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
}
