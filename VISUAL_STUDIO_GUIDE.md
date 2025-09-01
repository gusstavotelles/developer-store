# Guia Detalhado para Executar o Projeto no Visual Studio

Este guia fornece um passo a passo detalhado para abrir, configurar e executar a solução `Developer Store` no ambiente de desenvolvimento do Visual Studio.

## Parte 1: Abrindo a Solução no Visual Studio

### Via Linha de Comando (CLI)

1.  **Abra o Terminal:** Abra um terminal de sua preferência (PowerShell, CMD, etc.).
2.  **Navegue até a Pasta do Projeto:** Use o comando `cd` para navegar até a pasta raiz do projeto.
    ```sh
    cd E:\Dev\repos-ssd\Desafios\developer-store
    ```
3.  **Abra a Solução:** Execute o comando abaixo para abrir o arquivo de solução (`.sln`) no Visual Studio.
    ```sh
    start backend/Ambev.DeveloperEvaluation.sln
    ```
    O Visual Studio será iniciado e carregará a solução.

### Via Interface Gráfica (UI)

1.  **Abra o Visual Studio.**
2.  Clique em **"Abrir um projeto ou uma solução"**.
3.  Navegue até a pasta `E:\Dev\repos-ssd\Desafios\developer-store\backend`.
4.  Selecione o arquivo `Ambev.DeveloperEvaluation.sln` e clique em **"Abrir"**.

## Parte 2: Configurando e Executando o Projeto

O projeto está configurado para usar o Docker Compose para gerenciar as dependências (banco de dados, etc.) e executar a API localmente para facilitar o debug.

### Passo a Passo na UI do Visual Studio

1.  **Aguarde o Carregamento:** Espere o Visual Studio carregar completamente a solução e restaurar as dependências do NuGet.

2.  **Configure os Projetos de Inicialização:**
    *   No **Gerenciador de Soluções** (geralmente à direita), clique com o botão direito no nome da solução (`Solution 'Ambev.DeveloperEvaluation'`).
    *   Selecione **"Definir Projetos de Inicialização..."**.
    *   Na janela que abrir, selecione a opção **"Vários projetos de inicialização"**.
    *   Encontre os seguintes projetos na lista e configure a **Ação** para cada um:
        *   `docker-compose`: Defina a Ação como **"Iniciar"**.
        *   `Ambev.DeveloperEvaluation.WebApi`: Defina a Ação como **"Iniciar"**.
    *   Clique em **"Aplicar"** e depois em **"OK"**.

3.  **Execute a Solução:**
    *   Na barra de ferramentas superior do Visual Studio, você verá um botão de play verde. Certifique-se de que `Vários Projetos de Inicialização` esteja selecionado ao lado dele.
    *   Pressione **F5** ou clique no botão de play verde (**"Iniciar"**).

### O que Acontecerá?

*   O Visual Studio iniciará o Docker Desktop (se ainda não estiver em execução).
*   O Docker Compose criará e iniciará os contêineres para o banco de dados PostgreSQL e outros serviços.
*   A API (`Ambev.DeveloperEvaluation.WebApi`) será compilada e iniciada localmente.
*   Uma janela do navegador será aberta automaticamente, navegando para a página do Swagger (`https://localhost:7181/swagger` ou uma porta similar).

## Parte 3: Testando a Aplicação

Com a aplicação em execução, você pode seguir as instruções no `README.md` na seção **"Como testar via Swagger"** para criar usuários, autenticar e interagir com os endpoints da API.

Se encontrar qualquer problema, verifique o painel de **Saída (Output)** no Visual Studio, selecionando **"Docker"** e **"Build"** nas listas suspensas para ver os logs detalhados.
