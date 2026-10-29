# Publicar o back do Tribuna no IIS (Gerenciar)

O back é ASP.NET Core (.NET 10) + SQLite. O pacote é publicado **self-contained**: o .NET vai dentro da pasta,
então o servidor só precisa do **ASP.NET Core Module V2** (o mesmo que já roda o Saúde ao Seu Alcance).
Não precisa instalar .NET 10.

## 1. O que pedir à equipe

- Um site (ou aplicação) no IIS, de preferência com um endereço próprio, ex.: `api.otribuna.com.br`
  (o domínio `otribuna.com.br` é nosso; basta apontar o subdomínio para o IP do servidor).
- HTTPS válido nesse endereço.
- Application pool: **Sem código gerenciado** (No Managed Code), 64 bits, "Always running" desligado é ok.
- A pasta do site com **permissão de escrita** para a identidade do pool (ex.: `IIS AppPool\<nome>`),
  porque o banco (`App_Data\newsportal.db`) e as imagens (`App_Data\uploads`) ficam nela.
- Confirmar que o módulo **AspNetCoreModuleV2** está instalado (se o Saúde roda lá, está).

## 2. Gerar o pacote

No PowerShell, dentro da pasta do back:

```powershell
dotnet publish -c Release -r win-x64 --self-contained -o publish-iis
Compress-Archive -Path publish-iis\* -DestinationPath tribuna-api-iis.zip -Force
```

O `web.config` do repositório já vem configurado (hospedagem in-process, WebDAV desativado para liberar PUT/DELETE,
limite de upload de 12 MB).

## 3. Configuração no servidor

Descompactar o zip na pasta do site e criar, **ao lado do `NewsPortal.Api.exe`**, o arquivo
`appsettings.Production.json` (este arquivo contém segredos: nunca vai para o Git):

```json
{
  "Jwt": {
    "Key": "<gerar uma chave aleatória de 48+ caracteres>"
  },
  "Admin": {
    "Email": "<e-mail do primeiro administrador>",
    "Senha": "<senha forte: 10+ caracteres, maiúscula, minúscula, número e símbolo>"
  },
  "Cors": {
    "AllowedOrigins": [
      "https://otribuna.com.br",
      "https://www.otribuna.com.br",
      "https://tribuna-portal.vercel.app"
    ]
  },
  "Frontend": { "BaseUrl": "https://otribuna.com.br" },
  "Smtp": {
    "Host": "", "Port": "587", "User": "", "Password": "", "EnableSsl": "true",
    "FromEmail": "no-reply@otribuna.com.br", "FromName": "Tribuna"
  }
}
```

- `Jwt:Key` é obrigatória em produção: sem ela a API não sobe.
- `Admin` só é usado se o banco estiver sem usuários (cria o primeiro admin). Depois pode remover do arquivo.
- `Smtp` é opcional; sem ele, e-mails de recuperação de senha não são enviados.
- O banco é criado e migrado sozinho na primeira execução (com verticais, menu, rodapé e páginas iniciais).

## 4. Conferir

- `https://<endereço>/api/verticais` deve devolver a lista de verticais (200).
- `https://<endereço>/api/navegacao` deve devolver o menu.

## 5. Apontar o front

No repositório do front (`PortalTribuna`), em `src/environments/environment.prod.ts`, trocar `apiOrigin` e
`apiBaseUrl` pelo novo endereço (`https://<endereço>` e `https://<endereço>/api`) e dar push na `main` (o Vercel publica).

## 6. Atualizações futuras

Gerar o zip de novo (passo 2), **parar o site no IIS**, substituir os arquivos **sem apagar `App_Data` nem
`appsettings.Production.json`**, e iniciar o site. Migrações do banco rodam sozinhas ao subir.

## 7. Backup

Copiar periodicamente a pasta `App_Data` inteira (banco + `uploads`) para outro lugar. Com o site parado,
ou usando a cópia consistente do SQLite, para não pegar o arquivo no meio de uma escrita.

## Mensagem pronta para a equipe

> Precisamos de um site no IIS para uma API ASP.NET Core (self-contained, não precisa instalar .NET), em
> `api.otribuna.com.br` com HTTPS, pool "sem código gerenciado" e a pasta com permissão de escrita para o pool
> (ela guarda um banco SQLite e imagens). O módulo AspNetCoreModuleV2 é o mesmo que o Saúde ao Seu Alcance já usa.
> Mando o pacote .zip e o passo a passo.
