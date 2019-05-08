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
        public void CategoryMustHaveANameAndHasNoLinkWhenCreated()
        {
            var repo = new Mock<ICategoryRepository>();

            var category = new Category("some category");
            var service = new CategoryService(repo.Object);

            service.CreateCategory(category);
            var links = category.Links;

            repo.Verify(x => x.CreateCategory(category), Times.Once);
            Assert.Empty(links);
        }

        [Fact]
        public void LinkCanBeAddedToCategoryAndSaved()
        {
            var repo = new Mock<ICategoryRepository>();

            var category = new Category("some category");
            var service = new CategoryService(repo.Object);
            var expected = 3;

            service.AddLink(category, new Link());
            service.AddLink(category, new Link());
            service.AddLink(category, new Link());

            var actual = category.Links.Count();

            repo.Verify(x => x.SaveLink(category, It.IsAny<Link>()), Times.Exactly(expected));
            Assert.Equal(expected, actual);
        }
       
        //
        // Categories can't have same names
        // A link is part of a category
        // A link has a name and an URI
    }

    public interface ICategoryRepository
    {
        void CreateCategory(Category category);
        void SaveLink(Category category, Link link);
    }

    public class CategoryService
    {
        private readonly ICategoryRepository repo;

        public CategoryService(ICategoryRepository @object)
        {
            this.repo = @object;
        }

        public void CreateCategory(Category category)
        {
            repo.CreateCategory(category);
        }

        public void AddLink(Category category, Link link)
        {
            repo.SaveLink(category, link);
            category.AddLink(link);
        }
    }

    public class Category
    {
        private readonly List<Link> links;

        public IEnumerable<Link> Links
        {
            get
            {
                return links;
            }
        }

        public Category(string v)
        {
            links = new List<Link>();
        }

        public void AddLink(Link link)
        {
            links.Add(link);
        }
    }

    public class Link
    {
    }
}
