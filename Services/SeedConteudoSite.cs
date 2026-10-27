using Microsoft.EntityFrameworkCore;
using NewsPortal.Api.Data;
using NewsPortal.Api.Models;

namespace NewsPortal.Api.Services;

// GERADO a partir dos JSONs estáticos que o frontend usava antes do conteúdo ir pro banco
// (assets/mock/navigation.json, footer.json e pages/*.json). Só roda quando a tabela
// correspondente está vazia — nunca sobrescreve o que o admin já editou.
public static class SeedConteudoSite
{
    public static async Task ExecutarAsync(ApplicationDbContext context, ILogger logger)
    {
        if (!await context.MenuItens.AnyAsync())
        {
            var item0 = new MenuItem { Label = "Tribuna&Co.", Href = "/tribunaco", Ordem = 0, Ativo = true };
            context.MenuItens.Add(item0);
            var item1 = new MenuItem { Label = "Capital", Href = "/capital", Ordem = 1, Ativo = true };
            context.MenuItens.Add(item1);
            var item2 = new MenuItem { Label = "Esportes", Href = "/esportes", Ordem = 2, Ativo = true };
            context.MenuItens.Add(item2);
            var item3 = new MenuItem { Label = "Tech", Href = "/tech", Ordem = 3, Ativo = true };
            context.MenuItens.Add(item3);
            var item4 = new MenuItem { Label = "Games", Href = "/gg", Ordem = 4, Ativo = true };
            context.MenuItens.Add(item4);
            var item5 = new MenuItem { Label = "Cultura", Href = "/cultura", Ordem = 5, Ativo = true };
            context.MenuItens.Add(item5);
            var item6 = new MenuItem { Label = "Conteúdos", Href = "#", Ordem = 6, Ativo = true };
            item6.Links.Add(new MenuLink { ColunaTitulo = "Conteúdos", Label = "Inovação", Href = "/blog", Ordem = 0, Ativo = true });
            item6.Links.Add(new MenuLink { ColunaTitulo = "Conteúdos", Label = "Entrevistas", Href = "/blog", Ordem = 1, Ativo = true });
            item6.Links.Add(new MenuLink { ColunaTitulo = "Conteúdos", Label = "Vídeos", Href = "/blog", Ordem = 2, Ativo = true });
            item6.Links.Add(new MenuLink { ColunaTitulo = "Conteúdos", Label = "Linha Editorial", Href = "/linha-editorial", Ordem = 3, Ativo = true });
            context.MenuItens.Add(item6);
            await context.SaveChangesAsync();
            logger.LogInformation("Menu padrão criado a partir do conteúdo estático.");
        }

        if (!await context.FooterLinks.AnyAsync())
        {
            context.FooterLinks.Add(new FooterLink { Coluna = "Editorial", Label = "Cultura", Href = "/blog", Ordem = 0, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Editorial", Label = "Tecnologia", Href = "/tech", Ordem = 1, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Editorial", Label = "Negócios & Economia", Href = "/capital", Ordem = 2, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Observatório", Label = "Economia Corporativa", Href = "/capital", Ordem = 3, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Observatório", Label = "Mercado de Mídia", Href = "/capital", Ordem = 4, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Observatório", Label = "Tribuna em Números", Href = "/tribunaco", Ordem = 5, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Conteúdos", Label = "Entrevistas", Href = "/blog", Ordem = 6, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Conteúdos", Label = "Vídeos", Href = "/blog", Ordem = 7, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Conteúdos", Label = "Inovação", Href = "/blog", Ordem = 8, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Institucional", Label = "Sobre o Portal", Href = "/sobre-o-portal", Ordem = 9, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Institucional", Label = "Quem Somos", Href = "/quem-somos", Ordem = 10, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Institucional", Label = "Linha Editorial", Href = "/linha-editorial", Ordem = 11, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Institucional", Label = "Termos de Uso", Href = "/termos-de-uso", Ordem = 12, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Institucional", Label = "Política de Privacidade", Href = "/politica-de-privacidade", Ordem = 13, Ativo = true });
            context.FooterLinks.Add(new FooterLink { Coluna = "Institucional", Label = "Fale Conosco", Href = "/contato", Ordem = 14, Ativo = true });
            await context.SaveChangesAsync();
        }

        if (!await context.FooterRedesSociais.AnyAsync())
        {
            context.FooterRedesSociais.Add(new FooterRedeSocial { Icone = "facebook", Label = "Facebook", Href = "#", Ordem = 0, Ativo = true });
            context.FooterRedesSociais.Add(new FooterRedeSocial { Icone = "instagram", Label = "Instagram", Href = "#", Ordem = 1, Ativo = true });
            context.FooterRedesSociais.Add(new FooterRedeSocial { Icone = "youtube", Label = "YouTube", Href = "#", Ordem = 2, Ativo = true });
            context.FooterRedesSociais.Add(new FooterRedeSocial { Icone = "linkedin", Label = "LinkedIn", Href = "#", Ordem = 3, Ativo = true });
            await context.SaveChangesAsync();
        }

        if (!await context.FooterProjetosEspeciais.AnyAsync())
        {
            context.FooterProjetosEspeciais.Add(new FooterProjetoEspecial { Icone = "ring", Titulo = "Tribuna Investiga", Descricao = "Jornalismo investigativo e apuração aprofundada", Href = "/blog", Ordem = 0, Ativo = true });
            context.FooterProjetosEspeciais.Add(new FooterProjetoEspecial { Icone = "dot", Titulo = "Prêmio Tribuna de Cultura", Descricao = "Reconhecendo o melhor da produção cultural brasileira", Href = "/blog", Ordem = 1, Ativo = true });
            context.FooterProjetosEspeciais.Add(new FooterProjetoEspecial { Icone = "square", Titulo = "Ranking Tribuna 500", Descricao = "As empresas que mais crescem no país", Href = "/blog", Ordem = 2, Ativo = true });
            context.FooterProjetosEspeciais.Add(new FooterProjetoEspecial { Icone = "cross", Titulo = "Tribuna Next", Descricao = "Inovação, startups e o futuro dos negócios", Href = "/blog", Ordem = 3, Ativo = true });
            await context.SaveChangesAsync();
        }

        if (!await context.ConfiguracoesRodape.AnyAsync())
        {
            context.ConfiguracoesRodape.Add(new ConfiguracaoRodape
            {
                Tagline = "Mídia. Cultura. Negócios.",
                NewsletterTitulo = "Receba conteúdos exclusivos",
                NewsletterDescricao = "Assine nossa newsletter e fique por dentro das principais novidades de mídia, cultura e negócios.",
                NewsletterPlaceholder = "Seu e-mail",
                NewsletterCta = "Inscrever",
            });
            await context.SaveChangesAsync();
        }

        if (!await context.PaginasInstitucionais.AnyAsync())
        {
            context.PaginasInstitucionais.Add(new PaginaInstitucional
            {
                Slug = "quem-somos",
                Titulo = "Quem Somos",
                ResumoBusca = "O Tribuna consolida-se como um hub editorial de referência e a voz central do ecossistema Tribuna&Co., afastando-se do formato de portal de entretenimento raso.",
                ConteudoHtml = "<p class=\"lead\">O <strong>Tribuna</strong> consolida-se como um <strong>hub editorial de referência</strong> e a voz central do <strong>ecossistema Tribuna&amp;Co.</strong>, afastando-se do formato de portal de entretenimento raso.</p><p>O nosso objetivo central é a produção, curadoria e disseminação de conteúdo de qualidade sobre cultura, negócios, tecnologia e os grandes movimentos que moldam o país. Mais do que uma página na internet, nascemos com o propósito claro de tornar a informação relevante acessível a um público exigente e informado.</p><p>Acreditamos que jornalismo de qualidade é a melhor forma de orientar decisões. Por isso, a nossa comunicação exige um equilíbrio cuidadoso: utilizamos uma linguagem clara, elegante e direta para dialogar com o leitor em geral, ao mesmo tempo que mantemos a densidade analítica suficiente para sermos respeitados por empresas, investidores, criadores e formadores de opinião.</p><p>Atuamos como um verdadeiro observatório do mercado, traduzindo a complexidade da economia, da cultura e da tecnologia para o dia a dia de quem consome as nossas reportagens.</p><blockquote><p>Para garantir que esse conteúdo chegue a quem precisa, estruturamo-nos como uma base editorial pronta para alimentar múltiplos canais. A nossa atuação transcende os artigos escritos, expandindo-se dinamicamente para podcasts, newsletters, vídeos e redes sociais.</p></blockquote><p>Desta forma, conseguimos acompanhar a jornada do leitor em diferentes momentos do seu dia, incentivando ativamente a compreensão dos negócios, da cultura e das inovações que transformam o país.</p><p>Todo este trabalho é sustentado por um pilar inegociável de independência editorial. Este compromisso reflete-se na nossa própria identidade visual, que remete à integração de todas as verticais do ecossistema Tribuna&amp;Co. — Media, Sports, Business, Tech, Culture e GG.</p><p>O nosso sucesso constrói-se diariamente, não através de conversões comerciais, mas pela geração de valor contínuo e pela construção de uma audiência própria, recorrente e bem orientada.</p>",
                Ativo = true,
            });
            context.PaginasInstitucionais.Add(new PaginaInstitucional
            {
                Slug = "sobre-o-portal",
                Titulo = "Sobre o Portal",
                ResumoBusca = "O Tribuna é um hub editorial de mídia, cultura e negócios e a porta de entrada do ecossistema Tribuna&Co. Afastando-se dos modelos tradicionais de portais de entretenimento raso, a nossa plataforma...",
                ConteudoHtml = "<p>O Tribuna é um hub editorial de mídia, cultura e negócios e a porta de entrada do ecossistema Tribuna&amp;Co. Afastando-se dos modelos tradicionais de portais de entretenimento raso, a nossa plataforma nasceu com a missão de oferecer informação clara e relevante sobre cultura, economia, tecnologia e os movimentos que moldam o país. Acreditamos que o acesso a informação de qualidade é o primeiro passo para decisões melhores, dentro e fora dos negócios.</p><p>Para materializar este propósito, funcionamos como uma base editorial multicanal, projetada para acompanhar o leitor em qualquer momento do seu dia. O nosso conteúdo dinâmico ganha vida através de publicações, podcasts, newsletters, vídeos e presença constante nas redes sociais. Cobrimos desde os bastidores da cultura e do entretenimento até aos movimentos complexos do mercado, atuando de forma ativa como um observatório e garantindo que a informação relevante chegue a quem precisa dela.</p><h2>Mídia. Cultura. Negócios.</h2><blockquote><p>A nossa própria identidade visual reflete o nosso compromisso com a integração e a clareza. O nosso ícone representativo simboliza a união de todas as verticais do ecossistema Tribuna&amp;Co. e o olhar atento sobre o que realmente importa.</p></blockquote><p>Esta abordagem conceptual estende-se diretamente à nossa comunicação, que procura um equilíbrio vital: utilizar uma linguagem clara, elegante e direta para o público em geral, mantendo, em simultâneo, a densidade analítica exigida para dialogar de igual para igual com investidores, gestores e especialistas de cada setor.</p><p>Em última análise, o Tribuna não mede o seu impacto ou sucesso através de conversões comerciais. A nossa estratégia fundacional reside na geração de valor contínuo e na edificação de uma audiência própria, recorrente e altamente qualificada, sempre sustentada por uma independência editorial percebida.</p><p>O nosso compromisso é consolidarmo-nos como a sua fonte de referência mais segura, colocando a informação, a cultura e os negócios, de facto, ao seu alcance.</p>",
                Ativo = true,
            });
            context.PaginasInstitucionais.Add(new PaginaInstitucional
            {
                Slug = "linha-editorial",
                Titulo = "Linha Editorial",
                ResumoBusca = "A nossa linha editorial baseia-se no compromisso inegociável com a independência editorial e a democratização do acesso à boa informação. O Tribuna não é um canal de entretenimento raso ou um espaç...",
                ConteudoHtml = "<p>A nossa linha editorial baseia-se no compromisso inegociável com a independência editorial e a democratização do acesso à boa informação. O Tribuna não é um canal de entretenimento raso ou um espaço focado em conversões comerciais, mas sim um hub editorial qualificado e a voz central do ecossistema Tribuna&amp;Co.</p><p>Produzimos e curamos informações com o propósito de levar clareza, relevância e confiança para o dia a dia de quem nos lê, transformando dados complexos em análises seguras.</p><p>Para cumprir esta missão, adotamos um equilíbrio cuidadoso na nossa comunicação. As nossas publicações utilizam uma linguagem clara, elegante e direta para dialogar de forma próxima com o público em geral. Simultaneamente, mantemos o rigor e a densidade analítica necessários para sermos respeitados como uma fonte de autoridade e credibilidade por empresas, investidores, criadores e outros agentes de cada mercado.</p><blockquote><p>Os nossos pilares de conteúdo refletem a amplitude do nosso olhar editorial. Focamo-nos fortemente na cultura, nos negócios e na tecnologia, abordando temas centrais como economia criativa, inovação e comportamento.</p></blockquote><p>Paralelamente, atuamos como um verdadeiro observatório do mercado, analisando e traduzindo tendências, inovações tecnológicas e as movimentações do cenário econômico e cultural que impactam diretamente a sociedade.</p><p>Todo o material que produzimos é pensado para nutrir uma jornada de leitura contínua, conceito que está no coração da nossa identidade visual. Esta base editorial qualificada ganha vida em múltiplos canais, alimentando os nossos artigos, podcasts, vídeos, redes sociais e newsletters.</p><p>A nossa métrica de sucesso não está ligada a conversões diretas, mas sim à geração de valor real e contínuo, construindo uma audiência própria, recorrente e guiada pela informação confiável.</p>",
                Ativo = true,
            });
            context.PaginasInstitucionais.Add(new PaginaInstitucional
            {
                Slug = "termos-de-uso",
                Titulo = "Termos de Uso",
                ResumoBusca = "Estes Termos de Uso regulam o acesso e a utilização dos conteúdos publicados pela Tribuna, vertical de mídia da Tribuna&Co. Ao navegar neste site, você concorda com as condições descritas abaixo.",
                ConteudoHtml = "<p class=\"lead\">Estes Termos de Uso regulam o acesso e a utilização dos conteúdos publicados pela <strong>Tribuna</strong>, vertical de mídia da <strong>Tribuna&amp;Co.</strong> Ao navegar neste site, você concorda com as condições descritas abaixo.</p><p>O conteúdo publicado pela Tribuna — textos, imagens, gráficos e demais materiais — é protegido por direitos autorais e destina-se exclusivamente a uso pessoal e não comercial, salvo autorização expressa. A reprodução total ou parcial sem crédito e permissão prévia não é permitida.</p><p>Buscamos manter precisão e atualização nas informações publicadas, mas não garantimos que todo o conteúdo esteja livre de erros a qualquer momento. Correções, quando identificadas, são aplicadas assim que possível.</p><p>Conteúdos marcados como &quot;Patrocinado&quot; ou &quot;Publicidade&quot; são de responsabilidade dos anunciantes envolvidos e seguem critérios editoriais de transparência, sendo identificados de forma clara em relação ao conteúdo jornalístico independente.</p><p>Reservamo-nos o direito de atualizar estes termos periodicamente. Alterações relevantes serão comunicadas por meio deste próprio site.</p>",
                Ativo = true,
            });
            context.PaginasInstitucionais.Add(new PaginaInstitucional
            {
                Slug = "politica-de-privacidade",
                Titulo = "Política de Privacidade",
                ResumoBusca = "A Tribuna respeita a sua privacidade e trata os dados pessoais coletados neste site com transparência e responsabilidade.",
                ConteudoHtml = "<p class=\"lead\">A <strong>Tribuna</strong> respeita a sua privacidade e trata os dados pessoais coletados neste site com <strong>transparência e responsabilidade</strong>.</p><p>Coletamos informações fornecidas voluntariamente, como e-mail na assinatura da newsletter ou no cadastro de conta, além de dados de navegação (páginas visitadas, tempo de sessão) usados para melhorar a experiência editorial e medir audiência de forma agregada.</p><p>Não vendemos dados pessoais a terceiros. Informações podem ser compartilhadas com parceiros técnicos estritamente para operação do site (por exemplo, envio de newsletter), sempre sob obrigação de confidencialidade.</p><p>Você pode solicitar a qualquer momento a atualização ou exclusão dos seus dados, além de cancelar a assinatura da newsletter pelo próprio link enviado em cada edição.</p><p>Dúvidas sobre esta política podem ser enviadas pelo nosso canal de contato.</p>",
                Ativo = true,
            });
            await context.SaveChangesAsync();
        }
    }
}
