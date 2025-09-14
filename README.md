# EPW Recaster PT-BR

![Visão Geral](https://i.snipboard.io/i0SQku.jpg)

## Download
**[ [ Versões Mais Recentes e Antigas ](https://github.com/KimDebroye/EPW-Recaster/releases) ]**

___

## Em resumo
O EPW Recaster é uma ferramenta que
- automatiza a reforja de armas e equipamentos (armaduras) do EPW
- usando Reconhecimento Óptico de Caracteres (OCR) e
- condições de busca configuráveis pelo usuário.
> *O EPW Recaster não depende nem utiliza nenhum tipo de injeção no jogo (hook).<br />Ele se baseia unicamente no que é capturado usando OCR e realiza<br />escolhas e ações programáticas com base nos resultados capturados.*

> **Nota de Compatibilidade**
> - Foi confirmado que esta ferramenta também funciona em outros servidores (*Relentless, ...*).

___

## Versão Resumida 3.1 Início Rápido

| **Demonstração em Vídeo** | **Links Úteis do Leia-me** |
| :--- | :--- |
| [![EPW Recaster ~ Vídeo de Demonstração](https://i.snipboard.io/iB6j5q.jpg)](https://youtu.be/i75cPTjQQ6Q) | 🔽 [Configuração e Pré-requisitos](#setup)<br />🔽 [Formulário de Configuração Principal](#1--main--setup-form)<br />🔽 [Exemplos Comparativos de Condições](#1-4-3-comparative-condition-list-examples)<br />🔽 [FAQ](#faq)<br />🔽 [Contato \| Feedback](#contact--feedback) |

___

## Configuração

- Extraia o conteúdo do pacote fornecido<br />para qualquer pasta que tenha privilégios de escrita.<br />( *ex.:* `Área de Trabalho` | `C:\Apps\EPW Recaster` | ... )
- Inicie `EPW Recaster(.exe)`.
 - Clique no botão de idioma no formulário principal para alternar entre o OCR em Inglês e Português (ou edite `Config/Language.cfg` manualmente). Dados de idioma ausentes do Tesseract, como `por.traineddata`, são baixados automaticamente.

### ❗ Pré-requisitos de Configuração Adicionais ❗

**<details><summary>` [ (Mostrar|Ocultar) Pré-requisitos de Configuração Adicionais ] `</summary>**

- **Esta ferramenta pode exigir privilégios administrativos devido às ações de baixo nível utilizadas**<br />( *ex.: mover/clicar o mouse*, ... ).
  - **Passos para verificar/habilitar privilégios administrativos** :
    - Clique com o botão direito em `EPW Recaster(.exe)` e<br />escolha `Propriedades`.
    - Na aba `Compatibilidade`,<br />marque `☑ Executar este programa como administrador`<br />e confirme clicando em `OK`.
- **Esta ferramenta requer que a Escala de Exibição do Windows seja definida como `100%`<br />para um comportamento correto de captura de tela**.
  - **Passos para verificar/alterar a Escala de Exibição do Windows**<br />( *Windows 10 / Outras versões: use o Google* ) :
    - Clique com o botão direito em qualquer lugar na área de trabalho e<br />escolha `Configurações de Exibição`.
    - Certifique-se de que a opção Escala e layout esteja definida como `100%` e<br />feche a janela.<br />
      ![Escala de Exibição do Windows 10](https://i.snipboard.io/aMzD0F.jpg)

</details>

___

## Seções

![Seções](https://i.snipboard.io/NvjpMd.jpg)

### Notas Gerais
> - Uma vez que uma pré-visualização ou uma reforja automática é iniciada, o formulário principal será minimizado e restaurado programaticamente depois.<br />( *O formulário principal é usado principalmente apenas para fins de configuração.* )
> - Por outro lado, o formulário de informações sempre permanecerá sobre todas as janelas.
> - Todas as alterações são salvas e restauradas automaticamente ao reiniciar.
> - Usando qualquer tipo de editor de texto, as opções de tema podem ser alteradas em<br />`.\Config\ThemeColorStyle.cfg` (*inclui comentários adicionais*).

___

### 1. ( Principal ) Formulário de Configuração

___

#### 1-1. Região Transparente

![Região Transparente](https://snipboard.io/KETSjh.jpg)

- Ao iniciar o EPW Recaster pela primeira vez<br />( *e/ou sempre que a localização no jogo da janela de reforja<br />também conhecida como remodelar/fabricação for alterada* ),
- **mova a ferramenta e redimensione usando a alça de redimensionamento**<br />para que:
  - a região transparente se ajuste à janela de reforja do jogo<br />também conhecida como remodelar/fabricação,
  - os 3 pequenos quadrados (*indicando as regiões de clique*)<br />estejam localizados sobre os botões do jogo<br />( `Manter o atributo antigo` | `Reforjar` | `Usar o novo atributo` ),
  - a região de captura se ajuste ao texto a ser capturado.
    - **O ajuste não precisa ser perfeito ao nível do pixel para que o Reconhecimento Óptico de Caracteres funcione corretamente.**
    - Além disso, **tente evitar incluir quaisquer elementos adicionais da interface do usuário na região capturada**.<br />Motivo: Dependendo do ajuste, partes da interface do jogo podem ser detectadas como um caractere<br />( *ex.: o ícone de rolagem para cima do jogo pode ser detectado como a letra 'A' maiúscula* ).

___

#### 1-2. Região de Captura

![Região de Captura](https://snipboard.io/gimUN4.jpg)

- ( *Uma pré-visualização visível da* )
- A região que define os limites usados para o Reconhecimento Óptico de Caracteres.
- Dependendo do modo em que o processo será iniciado, a região de captura estará localizada:
  - **Modo de Pré-visualização** : largura total da região transparente e um pouco acima dos botões do jogo.
  - **Modo de Reforja** : metade direita da região transparente e um pouco acima dos botões do jogo.

> **❗ NOTA IMPORTANTE ❗**
> - **[ ! ] Sem nenhuma alteração real nos arquivos do jogo (*`configs.pck`* ),<br />não é recomendado usar o EPW Recaster<br />para procurar atributos em armas que possuem atributos únicos (*com descrição longa*)**,<br />a menos que seja um desses atributos únicos o alvo da reforja.
> - *Em outras palavras*, evite procurar por atributos em armas que possuam<br />`Purify Spell`, `God of Frenzy`, `Square Formation`, `Soul Shatter`, `Spirit Blackhole`, ...<br />como um atributo possível para não perder um atributo que precise de rolagem na janela do jogo<br />(*a menos que os atributos mencionados anteriormente sejam os alvos específicos*).

___

#### 1-3. Alternador de Lista de Condições

![Alternador de Lista de Condições](https://i.snipboard.io/jYq52c.jpg)

- **Clique com o Botão Esquerdo do Mouse**:
  - Selecione um dos 5 slots de lista de condições para trabalhar.
- **Clique com o Botão Direito do Mouse**:
  - **Copiar / Exportar Lista de Condições**.
    - Pode ser usado para:
      - compartilhar uma lista de condições com qualquer pessoa,
      - mover uma lista de condições para outro slot importando-a.
      - fazer backup de uma lista de condições (*ex.: em um documento de texto*).
  - **Colar / Importar Lista de Condições**.
    - Pode ser usado para:
      - importar uma lista de condições,
      - sobrescrever uma lista de condições existente com outra.
  - **Limpar Lista de Condições**.
      - Limpa todas as entradas de uma lista de condições.

___

#### 1-4. Lista de Condições

![Lista de Condições](https://i.snipboard.io/30e8dN.jpg)

Uma lista contendo as condições de reforja preferidas.
Usada para parar a reforja programaticamente quando uma das condições necessárias listadas for atendida.
A lista de condições pode ter entradas mistas de atributos de quantidade fixa e combos de atributos.
A ordem das entradas pode ser alterada arrastando uma entrada para outro local na lista de condições.

##### 1-4-1. Atributo(s) de Quantidade Fixa

![Atributo de Quantidade Fixa | Atributos de Quantidade Fixa](https://snipboard.io/cV2Tuo.jpg)

Embora **EXIJA UMA QUANTIDADE FIXA** de um atributo preferido, único ou agrupado,
os resultados da reforja **PODEM TER QUALQUER OUTRO ATRIBUTO**.

- **Aceitará uma reforja se**
  uma quantidade exata ou maior de um atributo único preferido ou de cada um dos atributos agrupados for detectada.
- **Rejeitará uma reforja se**
  uma quantidade exata ou maior de um atributo único preferido ou de cada um dos atributos agrupados não for detectada.
- Reconhecível pela cor de atributo azul.
- Sempre precedido por uma quantidade mínima fixa de um atributo preferido.
- Pode ter até 4 requisitos de atributos (agrupados) por entrada.
- Usado principalmente para reforjas:
    - que possuem atributos iguais:
        - 4 x Intervalo Entre Ataques
    - que precisam de pelo menos uma certa quantidade de atributos:
        - pelo menos 2 x Canalização ( e/ou qualquer outro atributo obtido )
    - ...

##### 1-4-2. Combo de Atributos

![Combo de Atributos](https://snipboard.io/mJXKZQ.jpg)

Embora **NÃO EXIJA UMA QUANTIDADE FIXA** de um atributo preferido, único ou agrupado,
os resultados da reforja **NÃO PODEM TER NENHUM OUTRO ATRIBUTO**.

- **Aceitará uma reforja se**
  uma combinação de pelo menos um de cada dos atributos agrupados preferidos for detectada (e somente eles).
- **Rejeitará uma reforja se**
    - uma combinação de pelo menos um de cada dos atributos agrupados preferidos não for detectada ou
    - um atributo que não está listado nos atributos agrupados preferidos for detectado.
- Reconhecível pela cor de atributo dourada.
- Não são precedidos por uma quantidade mínima fixa de um atributo preferido.
- Pode ter até 4 requisitos de atributos (agrupados) por entrada.
- Usado principalmente para reforjas:
    - que precisam de uma quantidade incerta de certos atributos específicos apenas:
        - pelo menos 1 x Canalização e pelo menos 1 x Redução de Dano Físico Recebido ( e NENHUM outro atributo obtido )
    - ...

##### 1-4-3. Exemplos Comparativos da Lista de Condições

| **Condição** | **Aceitaria** | **Rejeitaria** |
| :--- | :--- | :--- |
| **Atributo de Quantidade Fixa** | ✅<br>• Canalização -3%<br>• Canalização -2%<br>• Canalização -3%<br>• Canalização -2%<br><br>✅<br>• Canalização -3%<br>• Mágico +9<br>• Canalização -2%<br>• Redução de Dano Físico Recebido +2%<br><br>✅<br>• Canalização -3%<br>• Canalização -2%<br>• Canalização -3%<br>• Mágico +9 | ❌<br>• Canalização -3%<br>• Mágico +9<br>• Mágico +10<br>• Redução de Dano Físico Recebido +2%<br><br>❌<br>• Redução de Dano Físico Recebido +2%<br>• Redução de Dano Físico Recebido +1%<br>• Redução de Dano Físico Recebido +2%<br>• Redução de Dano Físico Recebido +2%<br><br>❌<br>• Canalização -3%<br>• Redução de Dano Físico Recebido +1%<br>• Redução de Dano Físico Recebido +2%<br>• Redução de Dano Físico Recebido +2% |
| **Atributos de Quantidade Fixa** | ✅<br>• Redução de Dano Físico Recebido +2%<br>• Canalização -3%<br>• Redução de Dano Físico Recebido +1%<br>• Canalização -2% | ❌<br>• Canalização -3%<br>• Canalização -2%<br>• Canalização -3%<br>• Canalização -1%<br><br>❌<br>• Canalização -3%<br>• Canalização -1%<br>• Canalização -3%<br>• Redução de Dano Físico Recebido +2%<br><br>❌<br>• Canalização -3%<br>• Mágico +9<br>• Mágico +10<br>• Redução de Dano Físico Recebido +2% |
| **Combo de Atributos** | ✅<br>• Canalização -3%<br>• Redução de Dano Físico Recebido +1%<br>• Canalização -2%<br>• Redução de Dano Físico Recebido +2%<br><br>✅<br>• Canalização -3%<br>• Canalização -2%<br>• Canalização -2%<br>• Redução de Dano Físico Recebido +2%<br><br>✅<br>• Redução de Dano Físico Recebido +2%<br>• Redução de Dano Físico Recebido +1%<br>• Redução de Dano Físico Recebido +2%<br>• Canalização -3% | ❌<br>• Canalização -3%<br>• Canalização -2%<br>• Canalização -3%<br>• Canalização -3%<br><br>❌<br>• Canalização -3%<br>• Canalização -3%<br>• Mágico +9<br>• Canalização -2%<br><br>❌<br>• Canalização -3%<br>• Canalização -3%<br>• Mágico +9<br>• Redução de Dano Físico Recebido +2% |

##### 1-5. Entrada de Condição (Entradas)

![Entrada de Condição](https://snipboard.io/uAz8kQ.jpg)

❗ **NOTA IMPORTANTE** ❗
Sempre pense bem sobre quais atributos reforjados seriam preferíveis e
adicione condições abrangentes de acordo
para não perder nenhuma boa reforja.

Para listar uma condição de reforja:

1.  Selecione uma quantidade preferida e um atributo preferido a ser encontrado.
2.  (Opcional) Selecione até 3 quantidades e atributos preferidos adicionais para serem encontrados/combinados.
3.  Assim que um segundo atributo preferido for selecionado na lista suspensa,
    uma caixa de seleção para ignorar quantidades ficará disponível.
      - Se marcada, a entrada se tornará uma entrada de combo (permitindo qualquer quantidade dos atributos selecionados, embora limitando a reforja a conter apenas os atributos selecionados).
4.  Clique no sinal de **+** verde.

Qualquer condição adicionada anteriormente pode ser removida
pressionando o **x** vermelho na lista de condições.

**Notas Adicionais**

- Ignore os atributos brancos, apenas os atributos azuis devem ser levados em conta.
  ( *ex.: 4 x Res. Fís. = máx., ignorando o quinto atributo branco de Res. Fís. em um equipamento* )
- Ao adicionar (acidentalmente) uma quantidade maior que 1 de um atributo único ( *ex.: Purify Spell* ),
  ele será listado como 1 x.
- Ao adicionar (acidentalmente) uma quantidade somada que exceda o máximo de atributos possível,
  ele será listado como 4 x ou 5 x (somente Atq. e Def.).
- Usando qualquer tipo de editor de texto, a lista de opções de atributos selecionáveis pode ser alterada em
  `.\Config\Stats.cfg` (*inclui comentários adicionais*).

___

### 2. Formulário de Informações

#### 2-1. Agrupador de Formulário

![Agrupador de Formulário](https://snipboard.io/Fc0akN.jpg)

Um botão de alternância que anexa/desanexa o formulário de informações do formulário principal.

- **Modo Agrupado** ( modo de formulários anexados | padrão na primeira inicialização ):
    - Apenas o formulário principal será móvel e redimensionável.
    - Apenas a localização e o tamanho do formulário principal serão salvos e restaurados ao reiniciar ( devido ao formulário de informações seguir suas alterações de localização e/ou tamanho ).
- **Modo Desagrupado** ( modo de formulários desanexados )
    - Tanto o formulário principal quanto o de informações serão móveis e redimensionáveis separadamente.
    - As localizações e tamanhos de ambos os formulários serão salvos e restaurados ao reiniciar.

#### 2-2. Pasta de Logs

![Pasta de Logs](https://snipboard.io/e24Ea5.jpg)

Clicar neste botão abre a pasta de logs.
Para cada reforja, um arquivo de texto e imagem resultante é registrado.
> [ ! ] Ocasionalmente, esvazie/exclua esta pasta<br />para liberar espaço de armazenamento.

#### 2-3. Informações do Resultado do OCR

![Informações do Resultado do OCR](https://snipboard.io/z4EmxV.jpg)

Exibe o texto capturado juntamente com algumas informações adicionais ao pré-visualizar ou reforjar.

#### 2-4. Modo de Pré-visualização | Reforja

![Modo de Pré-visualização | Reforja](https://snipboard.io/j9V6Is.jpg)

- **Modo de Pré-visualização** ( padrão na primeira inicialização ):
    - Uma vez iniciado, realizará uma única captura de texto.
    - Nenhuma reforja será realizada no jogo.
- **Modo de Reforja**
    - Uma vez iniciado, realizará um número definido de reforjas no jogo,
      obedecendo a quaisquer condições previamente definidas e
      resultando em um movimento programático do cursor do mouse e cliques do mouse.
    - Pode ser parado a qualquer momento clicando no botão **Parar**.

Usando qualquer tipo de editor de texto, as temporizações podem ser alteradas em
`\.\Config\Params.cfg` (*inclui comentários adicionais*).

___

## FAQ (Perguntas Frequentes)

**<details><summary>` [ (Mostrar|Ocultar Resposta) "A ferramenta não parece funcionar para mim... o que eu faço?" ] `</summary>**

> ➥ **Resposta**:
- **Certifique-se de que os pré-requisitos de configuração tenham sido atendidos.**
- Se estiver usando Windows 8 ou superior,<br />não instale a ferramenta em `C:\Program Files`.
- Tente executar a ferramenta como administrador.<br />Se estiver usando Windows 10:<br />verifique as configurações de exibição e certifique-se de que a escala esteja definida como `100%`.
- Se o problema persistir,<br />entre em contato e forneça informações detalhadas.

<hr />

</details>

**<details><summary>` [ (Mostrar|Ocultar Resposta) "Os atributos capturados não correspondem aos atributos reforjados... o que eu faço?" ] `</summary>**

> ➥ **Resposta**:
- **Possíveis causas**:
    - A região transparente foi ajustada incorretamente.<br />Verifique [ [ 1-1. Região Transparente ](#1-1-regiao-transparente) ] para mais informações.
    - O modo de reforja foi iniciado com o cursor sobre o botão errado.<br />Verifique [ [ 2-4. Modo de Pré-visualização | Reforja ](#2-4-modo-de-pre-visualizacao--reforja) ] para mais informações.
    - O jogo renderizou o texto de forma diferente do esperado.<br />Por favor, forneça informações detalhadas se você achar que este é o caso.

<hr />

</details>

**<details><summary>` [ (Mostrar|Ocultar Resposta) "Por que a ferramenta pulou uma reforja muito exótica?" ] `</summary>**

> ➥ **Resposta**:
- **Possíveis causas**:
    - **A reforja continha um atributo único com descrição longa.**
      - Verifique [ [ 1-2. Região de Captura ](#1-2-regiao-de-captura) ] para mais informações.
    - **Os atributos capturados não correspondiam aos atributos reforjados.**
      - Verifique [ [ "Os atributos capturados não correspondem aos atributos reforjados" FAQ ](#faq-qa2) ] para mais informações.
    - **Informe-me com informações detalhadas ( *e, se possível, passos para reproduzir* )<br />se você achar que nenhum dos itens acima é a razão.<br />Eu consideraria isso uma correção prioritária.**

<hr />

</details>

**<details><summary>` [ (Mostrar|Ocultar Resposta) "Quais são as minhas chances de obter certos atributos?" ] `</summary>**

> ➥ **Resposta**:
- **Uma boa e atualizada referência sobre as chances de reforja por atributo/equipamento** pode ser encontrada aqui:<br />
  [EPW Forum ~ R8 Recast Add-On Guide](https://epicpw.com/index.php?topic=2172.0).
- Quanto à ferramenta, ela não aumenta as chances de nenhuma maneira.<br />
  Diz a lenda que *`inf`* uma vez escreveu que esta ferramenta apenas tira a parte entediante de reforjar equipamentos ;).

<hr />

</details>

**<details><summary>` [ (Mostrar|Ocultar Resposta) "Eu ainda uso a versão 2 da ferramenta, devo atualizar?" ] `</summary>**

> ➥ **Resposta**:
- **Em geral: sim**, eu aconselharia atualizar.
- Em resumo:
  - Para reforjas como *`2 x Canalização`*, *`4 x Intervalo`*, qualquer versão anterior funciona bem.
  - Para reforjas mais exóticas (*combinações de atributos, ...*), a versão 3.1 é recomendada.

<hr />

</details>

**<details><summary>` [ (Mostrar|Ocultar Resposta) "Esta ferramenta funciona em outros servidores também (além do EPW)?" ] `</summary>**

> ➥ **Resposta**:
- **Em geral: se o servidor X segue as mesmas mecânicas de reforja de equipamentos, deve funcionar**.
- Eu só joguei EPW, embora tenha recebido confirmação de que funciona, por exemplo, em *Relentless* também.

<hr />

</details>

**<details><summary>` [ (Mostrar|Ocultar Resposta) "A velocidade da reforja pode ser ajustada/aumentada?" ] `</summary>**

> ➥ **Resposta**:
- **Sim**. Conforme declarado na seção [ [2-4. Modo de Pré-visualização | Reforja](#2-4-modo-de-pre-visualizacao--reforja) ]:<br />
  **Usando qualquer tipo de editor de texto, as temporizações podem ser alteradas em**<br />
   **`.\Config\Params.cfg (inclui comentários adicionais).`**
- Exemplo de configurações de velocidade (*padrões desde v3.1.2*) :

```ini
# =================================================================
# Tempo que leva para o botão de reforjar do jogo
# ficar disponível novamente.
# Nota: Deve ser acima de 1500 milissegundos (tempo medido pessoalmente).
# =================================================================

Await In-Game Reproduce Button Available    | 1750 milliseconds


# =================================================================
# Tempo que leva para os atributos no jogo serem reforjados.
# =================================================================

Await In-Game Stats Rolled            | 1750 milliseconds


# =================================================================
# Tempo de espera antes de aceitar/rejeitar uma reforja.
# Notas:
#       - Um valor menor acelera o processo de reforja.
#         No entanto, se reduzido: mais difícil parar o processo de reforja
#         devido ao movimento do cursor do mouse.
#       - Um valor maior torna mais fácil para o usuário
#         acompanhar o processo de reforja e, assim, se aproxima
#         mais do comportamento humano.
# =================================================================

Await Accept/Reject Action            | 2250 milliseconds
```

- A velocidade da reforja pode ser aumentada ainda mais se desejado.<br />( *Principalmente a 3ª opção de temporização; confira os comentários do arquivo de configuração para mais informações*.)
- Se a ferramenta ainda estiver aberta ao alterar o arquivo mencionado acima, reinicie a ferramenta para aplicar quaisquer alterações.

<hr />

</details>

**<details><summary>` [ (Mostrar|Ocultar Resposta) "Posso entrar em contato com você de alguma forma / fornecer algum feedback?" ] `</summary>**

> ➥ **Resposta**:
- **Claro.** Confira [abaixo](#contact--feedback) as maneiras de entrar em contato comigo.<br />Feedback é sempre bem-vindo e muito apreciado.

</details>

___

## Notas Técnicas e Referências

- Esta ferramenta foi programada no Visual Studio 2019 Community Edition usando a linguagem C#.
- Bibliotecas de terceiros utilizadas:
  - [Tesseract](https://github.com/UB-Mannheim/tesseract) (*OCR*)
  - [MetroFramework](https://github.com/thielj/MetroFramework) (*framework de UI*)
  - [Costura](https://github.com/Fody/Costura) (*compilação em executável autocontido*)
  - [Humanizer](https://github.com/Humanizr/Humanizer) (*diferença de tempo legível por humanos*)

___

## Contato | Feedback

- Poste uma mensagem no [ Tópico de Informações de Lançamento da Ferramenta EPW ](https://epicpw.com/index.php?topic=68651.0).
- Sinta-se à vontade para me enviar uma mensagem no jogo | no Discord.

> *(Qualquer tipo de) Feedback é sempre bem-vindo e muito apreciado.*
