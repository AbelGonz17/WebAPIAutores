using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers.V1;
using WebAPIAutores.Tests.Mokcs;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public  class RootControllerTest
    {
        [TestMethod]
        public async Task SiUsarioEsAdminObtenemos5Links()
        {
            //preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new UrlHelperMock();


            //ejecucion
            var resultado = await rootController.Get();

            //verificacion

            Assert.AreEqual(5, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiUsarioNoEsAdminObtenemos3Links()
        {
            //preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Failed();

            var rootController = new RootController(authorizationService);
            rootController.Url = new UrlHelperMock();


            //ejecucion
            var resultado = await rootController.Get();

            //verificacion

            Assert.AreEqual(3, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiUsarioNoEsAdminObtenemos3Links_usandoMoq()
        {
            //preparacion
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

                 mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Link(
                It.IsAny<string>(),
                It.IsAny<object>()                             
                )).Returns(string.Empty);
                             
            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockUrlHelper.Object;


            //ejecucion
            var resultado = await rootController.Get();

            //verificacion

            Assert.AreEqual(3, resultado.Value.Count());
        }

    }
}
