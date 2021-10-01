using System.Linq;

using FluentAssertions;
using NUnit.Framework;

using CustomerService.Entities;
using CustomerService.Business;

namespace UnitTests.Manager
{
    [TestFixture]
    public class BizManagerTest
    {
        private IBizManager<Customer> _bizManager;

        [SetUp]
        public void Setup()
        {
            _bizManager = new BizManager();
        }

        [Test]
        public void ShouldGenerateNewGuid_When_AddingNewCustomer ()
        {
            // Arrange
            var Customer = new Customer() { FirstName = "John", LastName = "Smith", DOB = "04/06/1980", SSN = "1234" };

            // Act
            _bizManager.Add(Customer);

            // Assert
            _bizManager.GetAll().Count.Should().Be(1);
            var result = _bizManager.GetAll().FirstOrDefault();
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Id.Should().BeOfType<string>();
        }

        [Test]
        public void ShouldReturnTrue_When_DeletingCustomerByExistingID ()
        {
            // Arrange
            var customer = new Customer() { FirstName = "John", LastName = "Smith", DOB = "04/06/1980", SSN = "1234" };
            _bizManager.Add(customer);
            var id = _bizManager.GetAll().FirstOrDefault().Id;

            // Act
            var results = _bizManager.DeleteByID(id);

            // Assert
            results.Should().BeTrue();
        }

        [Test]
        public void ShouldReturnFalse_When_DeletingCustomerByNotExistingID()
        {
            // Arrange
            var customer = new Customer() { FirstName = "John", LastName = "Smith", DOB = "04/06/1980", SSN = "1234" };
            _bizManager.Add(customer);

            // Act
            var results = _bizManager.DeleteByID("12345678");

            // Assert
            results.Should().BeFalse();
        }

        [Test]
        public void ShouldreturnCustomer_When_GetCustomerByIDForExistingID()
        {
            // Arrange
            var customer=new Customer() { FirstName = "John", LastName = "Smith", DOB = "04/06/1980", SSN = "1234" };
            _bizManager.Add(customer);
            var id = _bizManager.GetAll().FirstOrDefault().Id;

            // Act
            var results = _bizManager.GetByID(id);

            // Assert
            results.Should().NotBeNull();
            results.Id.Should().Be(id);
        }
    }
}
