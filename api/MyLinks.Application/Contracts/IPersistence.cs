using MyLinks.Application.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyLinks.Application.Contracts
{
    public interface IPersistence
    {
        void Create(Category category);
        Category FindByName(string name);
    }
}
