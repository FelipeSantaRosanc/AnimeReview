
### Padrões Implementados

- ✅ **Clean Architecture** - Separação clara de responsabilidades
- ✅ **Repository Pattern** - Abstração de acesso a dados
- ✅ **Unit of Work** - Transações atômicas e consistentes
- ✅ **Result Pattern** - Tratamento de erros sem exceções de controle de fluxo
- ✅ **CQRS Ready** - Preparado para separação de comandos e consultas
- ✅ **Soft Delete** - Exclusão lógica com Global Query Filters

---

## ⚡ Funcionalidades

### 🔐 Autenticação & Autorização
- Registro e login com JWT Bearer tokens
- Refresh tokens para sessões persistentes
- Roles (Admin, User) com políticas de acesso
- Rate limiting por endpoint (proteção contra brute force)

### 📺 Gestão de Animes
- CRUD completo com soft delete
- Upload de imagens para Cloudinary (com resize automático)
- Filtros avançados: busca textual, gênero, ano de lançamento
- Ordenação dinâmica por título, ano ou avaliação
- Paginação com metadados completos

### ⭐ Sistema de Reviews
- Avaliações de 1 a 10 estrelas
- Comentários opcionais
- Média automática de avaliações por anime
- Um review por usuário/anime (prevenção de duplicatas)
- Controle de edição/exclusão apenas pelo autor

### 👤 Gestão de Usuários
- Perfil completo com foto, bio, endereço
- Alteração de senha com validação
- Histórico de reviews do usuário

### 🚀 Performance & Segurança
- Cache distribuído Redis para consultas frequentes
- Rate limiting: 100 req/min (anônimo), 1000 req/h (autenticado)
- Health checks para monitoramento
- Logging estruturado com Serilog

---

## 📦 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (opcional, recomendado)
- [Git](https://git-scm.com/downloads)

---

## 🔧 Instalação

### 1. Clone o repositório

```bash
git clone https://github.com/FelipeSantaRosanc/AnimeReview.git
cd AnimeReview
