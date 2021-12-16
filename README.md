# WebMvc.Condominio

 * Esse projeto é um software para ser utilizado em portaria de condomínio horizontal ou vertical.
 1. Que permite, cadastrar o imóvel e neste vincular os moradores residente no imóvel com foto.
 2. Possibilita registrar os visitantes vinculando ao imóvel com foto.
 3. Registrar a entrada de prestadores de serviço vinculando ao imóvel com foto.
 4. Receber encomendas na portaria, que fica vinculando ao morador que está na encomenda e ao imóvel.
 5. Entregar a encomenda para o morador que recebeu e incluir a foto deste morador.
      
# WebMvc.OrdemServico

* Esse projeto possibilita registrar ordens de serviço por cliente.

# WebMvc.Vendas

* Esse projeto possibilita registrar pedidos de venda por cliente.

# WebMvc.CadastroPessoa

Nesse projeto foi implementado:

1. Validação customizada de CPF ou CNPJ com marcara na tela.
2. Nas telas de exibição dos dados de Pessoa é mostrado com a macara correta dependendo se é CPF ou CNPJ.
3. Também da data de anivesário.
4. A busca automatica do endereço pelo CEP com marcara na tela.
5. Possui mascara nos telefones na tela.

* Validação customizada de CPF ou CNPJ: Dependendo da opção de tipo de pessoa marcada, será validado quando Pessoa Física se o CPF é valido e se Pessoa Jurídica se o CNPJ é valido.
* Validação customizada da data de anivesário: Verifica se a data de nascimento/aniversário é maior de 18 anos.
* Ao digitar o CEP é realizada a busca automaticamento do endereço e preenchido os campos de Logradouro, Bairro, Cidade e Estado, que ficam bloqueados para edição.
