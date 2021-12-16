using Microsoft.AspNetCore.Mvc.Razor;
using System;

namespace WebMvc.Vendas.Extensions
{
    public static class RazorExtensions
    {
        public static string FormataCpfCnpj(this RazorPage page, int tipoPessoa, string cpfCnpj)
        {
            return tipoPessoa == 1 ? Convert.ToUInt64(cpfCnpj).ToString(@"000\.000\.000\-00") : Convert.ToUInt64(cpfCnpj).ToString(@"00\.000\.000\/0000\-00");
        }

        public static string FormataCEP(this RazorPage page, string cep)
        {
            return Convert.ToUInt64(cep).ToString(@"00\.000\-000");
        }

        public static string FormataCelular(this RazorPage page, string tel)
        {
            return Convert.ToUInt64(tel).ToString(@"00\ 00000\-0000");
        }
        public static string FormataTelefone(this RazorPage page, string tel)
        {
            return Convert.ToUInt64(tel).ToString(@"00\ 0000\-0000");
        }

        public static string FormataTipoPessoa(this RazorPage page, int tipoPessoa, string cpfCnpj)
        {
            return tipoPessoa == 1 ? "Pessoa Física" : "Pessoa Jurídica";
        }

        public static string MarcarOpcao(this RazorPage page, int tipoPessoa, int valor)
        {
            return tipoPessoa == valor ? "checked" : "";
        }
    }
}