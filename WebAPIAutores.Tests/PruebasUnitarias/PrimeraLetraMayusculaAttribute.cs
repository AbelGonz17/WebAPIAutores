using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            //preparacion 
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "felipe";
            var valContext = new ValidationContext(new { Nombre = valor });
            //ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //verificacion
            Assert.AreEqual("La primera letra debe ser Mayuscula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void ValorNulo_NoDevueleError()
        {
            //preparacion 
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });
            //ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //verificacion
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelvaSuccess()
        {
            //preparacion 
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "Felipe";
            var valContext = new ValidationContext(new { Nombre = valor });
            //ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);
            //verificacion
            Assert.AreEqual(ValidationResult.Success,resultado);
        }
    }
}
