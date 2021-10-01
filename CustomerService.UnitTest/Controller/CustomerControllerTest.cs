using System;
using System.Collections.Generic;
using System.Net;

using Microsoft.AspNetCore.Mvc;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using CustomerService.Business;
using CustomerService.Entities;
using CustomerService.Controller;

namespace CustomerService.UnitTest
{
    [TestFixture]
    public class CustomerControllerTest
    {
        private CustomerController _controller;
        private Mock<IBizManager<Customer>> _ibizManager;

        [SetUp]
        public void Setup()
        {
            _ibizManager = new Mock<IBizManager<Customer>>();
            _controller = new CustomerController(_ibizManager.Object);
        }

        [Test]
        public void ShouldReturNoContent_when_GetAllCustomersEmpty ()
        {
            // Arrange
            var listOfCustomers = (IList<Customer>) null;
            _ibizManager
                .Setup (ibiz => ibiz.GetAll ())
                .Returns (listOfCustomers);

            // Act
            var response = _controller.GetAllCustomers ();

            // Assert
            response.Should ().NotBeNull ();
            response.Should ().BeOfType<NotFoundResult> ();
            ((NotFoundResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.NotFound));
        }

        [Test]
        public void ShouldReturnAllCustomerList_when_GetAllCustomersNotEmpty ()
        {
            // Arrange 
            var mockedResponse = new List<Customer>() { 
                new Customer () {Id = "12345678" , FirstName ="John" , LastName ="Smith" , DOB ="04/06/1980" , SSN ="1234"},
                new Customer () {Id = "87456123" , FirstName = "Mary", LastName ="Couper", DOB ="12/12/1984" , SSN ="2345"}
            };

            _ibizManager
                .Setup(ibiz => ibiz.GetAll())
                .Returns(mockedResponse);

            // Act
            var response = _controller.GetAllCustomers ();

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();

            var result = response as OkObjectResult;

            result.StatusCode.Should().Be(Convert.ToInt32 (HttpStatusCode.OK));
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<List<Customer>> ();

            var values = result.Value as List<Customer>;

            values.Count.Should().Be (2);
        }

        [Test]
        public void ShouldReturn_Ok_When_AddingCustomer_Successfull ()
        {
            // Arrange 
            var customer = new Customer() { FirstName = "John", LastName = " Smith", DOB = "04/06/1980", SSN = "1234" };

            // Act
            var response = _controller.AddCustomer(customer);

            // Assert
            response.Should ().NotBeNull ();
            response.Should().BeOfType<CreatedAtRouteResult> ();
            var result = response as CreatedAtRouteResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.Created));
            result.RouteName.Should().Be("GetCustomerByID");
        }

        [Test]
        public void ShouldReturnNoContentResult_when_updatingCustomer ()
        {
            // Arrange
            var mockedResponse = new Customer() { Id = "12345678", FirstName = "John", LastName = "Smith", DOB = "04/06/1980", SSN = "1234" };

            _ibizManager
                .Setup(ibiz => ibiz.GetByID("12345678"))
                .Returns(mockedResponse);

            // Act
            var response = _controller.UpdateCustomerByID("12345678", mockedResponse);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<NoContentResult>();
            var result = response as NoContentResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.NoContent));
        }

        [Test]
        public void ShouldReturnBadRequest_When_UpdatingCustomer_with_EmptyID ()
        {
            // Arrange
            var customer = new Customer() { FirstName = "John", LastName = "Smith", DOB = "04/06/1980", SSN = "1234" };

            // Act
            var response = _controller.UpdateCustomerByID("", customer);

            // Assert 
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestResult>();
            ((BadRequestResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }

        [Test]
        public void ShouldReturnCustomer_When_GettingCustomerByID ()
        {
           // Arrange
           var mockedResponse = new Customer() { Id = "12345678", FirstName = "John", LastName = "Smith", DOB = "04/06/1980", SSN = "1234" };

            _ibizManager
                .Setup(ibiz => ibiz.GetByID("12345678"))
                .Returns(mockedResponse);

            // Act
            var response = _controller.GetCustomerByID("12345678");

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<OkObjectResult>();
            var result = response as OkObjectResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.OK));
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<Customer>();
            var value = result.Value as Customer;
        }

        [Test]
        public void ShouldReturnBadRequest_When_GetCustomerByID_With_EmptyID ()
        {
            // Act 
            var response = _controller.GetCustomerByID("");

            // Assert 
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
            ((BadRequestObjectResult)response).Value.Should().Be ("Invalid Customer Id");
        }

        [Test]
        public void ShouldReturnOk_When_DeletingCustomerByID ()
        {
            // Arrange
            _ibizManager
                .Setup(ibiz => ibiz.DeleteByID("12345678"))
                .Returns(true);

            // Act
            var response = _controller.DeleteCustomerByID("12345678");

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<NoContentResult>();
            var result = response as NoContentResult;
            result.StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.NoContent));
        }

        [Test]
        public void ShouldReturnBadRequest_When_DeletingCustomerById_With_EmptyID ()
        {
            // Act
            var response = _controller.DeleteCustomerByID("");

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<BadRequestResult>();
            ((BadRequestResult)response).StatusCode.Should().Be(Convert.ToInt32(HttpStatusCode.BadRequest));
        }
    }
}