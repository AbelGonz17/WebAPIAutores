﻿using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebApiAutores.Utilidades
{
    //esto es para pasarle las versiones por la cabecera

    public class CabeceraEstaPresenteAttribute : Attribute, IActionConstraint
    {
        private readonly string cabecera;
        private readonly string valor;

        public CabeceraEstaPresenteAttribute(string cabecera , string valor )
        {
            this.cabecera = cabecera;
            this.valor = valor;
        }


        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var cabeceras = context.RouteContext.HttpContext.Request.Headers;

            if(!cabeceras.ContainsKey(cabecera))
            {
                return false;
            }

            return string.Equals(cabeceras[cabecera], valor, StringComparison.OrdinalIgnoreCase);
        }
    }
}
