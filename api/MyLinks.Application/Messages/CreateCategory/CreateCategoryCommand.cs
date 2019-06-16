using System;
using System.ComponentModel.DataAnnotations;

namespace MyLinks.Application.Messages.CreateCategory
{
    public class CreateCategoryCommand
    {
        [Required]
        public Guid? Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
