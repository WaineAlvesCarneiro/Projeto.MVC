//using System;
//using System.ComponentModel.DataAnnotations;
//using System.Collections.Generic;
//using System.Linq;

//namespace WebMvc.Vendas.Validacao
//{
//    public class CustomValidation
//    {
//        #region Valida Pessoa Idade
//        public class Min18Anos : ValidationAttribute
//        {
//            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//            {
//                var pessoa = (Pessoa)validationContext.ObjectInstance;

//                int DiaAtual = DateTime.Today.Day;
//                int MesAtual = DateTime.Today.Month;
//                int AnoAtual = DateTime.Today.Year;

//                int DiaNascimento = pessoa.DtAniversario.Day;
//                int MesNascimento = pessoa.DtAniversario.Month;
//                int AnoNascimento = pessoa.DtAniversario.Year;

//                int Idade = AnoAtual - AnoNascimento;

//                if ((MesNascimento > MesAtual) || (MesNascimento == MesAtual && DiaNascimento > DiaAtual))
//                {
//                    Idade--;
//                }

//                return (Idade >= 18) ? ValidationResult.Success
//                    : new ValidationResult("Somente é permitido cadastrar maiores de 18 anos.");
//            }
//        }
//        #endregion

//        #region Pessoa Validation
//        public class CpfCnpjValido : ValidationAttribute
//        {
//            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//            {
//                var pessoa = (Pessoa)validationContext.ObjectInstance;

//                if (pessoa.TipoPessoa == 1)
//                {
//                    pessoa.CpfCnpj = Utils.ApenasNumeros(pessoa.CpfCnpj);

//                    bool result = pessoa.CpfCnpj.Length.Equals(CpfValidacao.TamanhoCpf);
//                    if (result)
//                    {
//                        result = CpfValidacao.Validar(pessoa.CpfCnpj).Equals(true);
//                        return (result) ? ValidationResult.Success
//                            : new ValidationResult("O CPF é inválido.");
//                    }
//                    return new ValidationResult($"O campo CPF precisa ter 11 caracteres e foi fornecido { pessoa.CpfCnpj }.");
//                }

//                if (pessoa.TipoPessoa == 2)
//                {
//                    pessoa.CpfCnpj = Utils.ApenasNumeros(pessoa.CpfCnpj);

//                    bool result = pessoa.CpfCnpj.Length.Equals(CnpjValidacao.TamanhoCnpj);
//                    if (result)
//                    {
//                        result = CnpjValidacao.Validar(pessoa.CpfCnpj).Equals(true);
//                        return (result) ? ValidationResult.Success
//                            : new ValidationResult("O CNPJ é inválido.");
//                    }
//                    return new ValidationResult($"O campo CNPJ precisa ter 14 caracteres e foi fornecido { pessoa.CpfCnpj }.");
//                }
//                return ValidationResult.Success;
//            }
//        }
//        #endregion

//        #region Cpf Validção
//        public class CpfValidacao
//        {
//            public const int TamanhoCpf = 11;

//            public static bool Validar(string cpfNumeros)
//            {
//                if (!TamanhoValido(cpfNumeros)) return false;
//                return !TemDigitosRepetidos(cpfNumeros) && TemDigitosValidos(cpfNumeros);
//            }

//            private static bool TamanhoValido(string valor)
//            {
//                return valor.Length == TamanhoCpf;
//            }

//            private static bool TemDigitosRepetidos(string valor)
//            {
//                string[] invalidNumbers =
//                {
//                "00000000000",
//                "11111111111",
//                "22222222222",
//                "33333333333",
//                "44444444444",
//                "55555555555",
//                "66666666666",
//                "77777777777",
//                "88888888888",
//                "99999999999"
//            };
//                return invalidNumbers.Contains(valor);
//            }

//            private static bool TemDigitosValidos(string valor)
//            {
//                var number = valor.Substring(0, TamanhoCpf - 2);
//                var digitoVerificador = new DigitoVerificador(number)
//                    .ComMultiplicadoresDeAte(2, 11)
//                    .Substituindo("0", 10, 11);
//                var firstDigit = digitoVerificador.CalculaDigito();
//                digitoVerificador.AddDigito(firstDigit);
//                var secondDigit = digitoVerificador.CalculaDigito();

//                return string.Concat(firstDigit, secondDigit) == valor.Substring(TamanhoCpf - 2, 2);
//            }
//        }
//        #endregion

//        #region Cnpj Validação
//        public class CnpjValidacao
//        {
//            public const int TamanhoCnpj = 14;

//            public static bool Validar(string cnpjNumeros)
//            {
//                if (!TemTamanhoValido(cnpjNumeros)) return false;
//                return !TemDigitosRepetidos(cnpjNumeros) && TemDigitosValidos(cnpjNumeros);
//            }

//            private static bool TemTamanhoValido(string valor)
//            {
//                return valor.Length == TamanhoCnpj;
//            }

//            private static bool TemDigitosRepetidos(string valor)
//            {
//                string[] invalidNumbers =
//                {
//                "00000000000000",
//                "11111111111111",
//                "22222222222222",
//                "33333333333333",
//                "44444444444444",
//                "55555555555555",
//                "66666666666666",
//                "77777777777777",
//                "88888888888888",
//                "99999999999999"
//            };
//                return invalidNumbers.Contains(valor);
//            }

//            private static bool TemDigitosValidos(string valor)
//            {
//                var number = valor.Substring(0, TamanhoCnpj - 2);

//                var digitoVerificador = new DigitoVerificador(number)
//                    .ComMultiplicadoresDeAte(2, 9)
//                    .Substituindo("0", 10, 11);
//                var firstDigit = digitoVerificador.CalculaDigito();
//                digitoVerificador.AddDigito(firstDigit);
//                var secondDigit = digitoVerificador.CalculaDigito();

//                return string.Concat(firstDigit, secondDigit) == valor.Substring(TamanhoCnpj - 2, 2);
//            }
//        }
//        #endregion

//        #region DigitoVerificador
//        public class DigitoVerificador
//        {
//            private string _numero;
//            private const int Modulo = 11;
//            private readonly List<int> _multiplicadores = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9 };
//            private readonly IDictionary<int, string> _substituicoes = new Dictionary<int, string>();
//            private bool _complementarDoModulo = true;

//            public DigitoVerificador(string numero)
//            {
//                _numero = numero;
//            }

//            public DigitoVerificador ComMultiplicadoresDeAte(int primeiroMultiplicador, int ultimoMultiplicador)
//            {
//                _multiplicadores.Clear();
//                for (var i = primeiroMultiplicador; i <= ultimoMultiplicador; i++)
//                    _multiplicadores.Add(i);

//                return this;
//            }

//            public DigitoVerificador Substituindo(string substituto, params int[] digitos)
//            {
//                foreach (var i in digitos)
//                {
//                    _substituicoes[i] = substituto;
//                }
//                return this;
//            }

//            public void AddDigito(string digito)
//            {
//                _numero = string.Concat(_numero, digito);
//            }

//            public string CalculaDigito()
//            {
//                return !(_numero.Length > 0) ? "" : GetDigitSum();
//            }

//            private string GetDigitSum()
//            {
//                var soma = 0;
//                for (int i = _numero.Length - 1, m = 0; i >= 0; i--)
//                {
//                    var produto = (int)char.GetNumericValue(_numero[i]) * _multiplicadores[m];
//                    soma += produto;

//                    if (++m >= _multiplicadores.Count) m = 0;
//                }

//                var mod = (soma % Modulo);
//                var resultado = _complementarDoModulo ? Modulo - mod : mod;

//                return _substituicoes.ContainsKey(resultado) ? _substituicoes[resultado] : resultado.ToString();
//            }
//        }
//        #endregion

//        #region Utils
//        public class Utils
//        {
//            public static string ApenasNumeros(string valor)
//            {
//                var onlyNumber = "";
//                foreach (var s in valor)
//                {
//                    if (char.IsDigit(s))
//                    {
//                        onlyNumber += s;
//                    }
//                }
//                return onlyNumber.Trim();
//            }
//        }
//        #endregion
//    }
//}