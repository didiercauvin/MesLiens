using System;
using System.Collections.Generic;
using System.Text;

namespace MyLinks.Application.Entity
{
    public class Category
    {
        public Category(Guid id, string name)
        {
            Id = id;
            this.Name = name;
        }

        public string Name { get; }
        public Guid Id { get; }
    }
}
