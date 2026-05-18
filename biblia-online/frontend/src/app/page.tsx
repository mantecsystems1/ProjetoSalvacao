import Link from "next/link";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";

export default function HomePage() {
  return (
    <main className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      <section className="overflow-hidden rounded-[2rem] bg-slate-950 text-white shadow-[0_40px_120px_rgba(15,23,42,0.25)]">
        <div className="grid gap-10 lg:grid-cols-[1.2fr_0.8fr] lg:items-center">
          <div className="space-y-6 px-6 py-10 sm:px-10 sm:py-14">
            <span className="inline-flex rounded-full bg-sky-400/10 px-4 py-2 text-xs font-semibold uppercase tracking-[0.32em] text-sky-300 ring-1 ring-sky-300/20">
              Bíblia online</span>
            <h1 className="text-4xl font-semibold tracking-tight text-white sm:text-5xl">
              Leia a Bíblia com fluidez, pesquisa instantânea e áudio integrado.
            </h1>
            <p className="max-w-2xl text-lg leading-8 text-slate-300 sm:text-xl">
              Uma experiência de leitura bíblica moderna e responsiva para celular, tablet e desktop. Navegue por versões, livros, capítulos e ouça o texto em voz alta.
            </p>
            <div className="flex flex-wrap gap-3">
              <Link href="/livros" className={cn(buttonVariants(), "min-w-[10rem]")}>
                Ler Bíblia
              </Link>
              <Link href="/buscar" className={cn(buttonVariants({ variant: "outline" }), "min-w-[10rem]")}>
                Buscar versículos
              </Link>
            </div>
          </div>
          <div className="grid gap-4 px-6 pb-10 sm:px-10">
            <div className="rounded-[1.5rem] bg-white/5 p-6 ring-1 ring-white/10 backdrop-blur">
              <p className="text-xs uppercase tracking-[0.32em] text-sky-300">Versão ativa</p>
              <h2 className="mt-4 text-2xl font-semibold text-white">Selecione a versão no cabeçalho</h2>
              <p className="mt-2 text-sm leading-6 text-slate-300">
                Clique no código da versão no topo da página para abrir o menu e escolher sua Bíblia preferida.
              </p>
            </div>
            <div className="rounded-[1.5rem] bg-white/5 p-6 ring-1 ring-white/10 backdrop-blur">
              <p className="text-xs uppercase tracking-[0.32em] text-sky-300">Áudio</p>
              <h2 className="mt-4 text-2xl font-semibold text-white">Ouça capítulos automaticamente</h2>
              <p className="mt-2 text-sm leading-6 text-slate-300">
                O leitor de áudio usa o idioma da versão selecionada para oferecer melhor experiência em português e inglês.
              </p>
            </div>
          </div>
        </div>
      </section>

      <section className="mt-10 grid gap-4 lg:grid-cols-3">
        <div className="rounded-[1.75rem] border border-white/10 bg-white/5 p-6 shadow-sm shadow-slate-950/10 backdrop-blur transition hover:-translate-y-1 hover:bg-white/10">
          <p className="text-sm font-semibold uppercase tracking-[0.32em] text-sky-300">Navegação</p>
          <h3 className="mt-4 text-2xl font-semibold text-white">Por livros e capítulos</h3>
          <p className="mt-3 text-sm leading-6 text-slate-300">
            Abra qualquer livro por capítulo, como em uma Bíblia física, com navegação fluida e moderna.
          </p>
        </div>
        <div className="rounded-[1.75rem] border border-white/10 bg-white/5 p-6 shadow-sm shadow-slate-950/10 backdrop-blur transition hover:-translate-y-1 hover:bg-white/10">
          <p className="text-sm font-semibold uppercase tracking-[0.32em] text-sky-300">Busca</p>
          <h3 className="mt-4 text-2xl font-semibold text-white">Pesquisa super rápida</h3>
          <p className="mt-3 text-sm leading-6 text-slate-300">
            Encontre versículos por palavra, frase ou referência e abra direto no capítulo desejado.
          </p>
        </div>
        <div className="rounded-[1.75rem] border border-white/10 bg-white/5 p-6 shadow-sm shadow-slate-950/10 backdrop-blur transition hover:-translate-y-1 hover:bg-white/10">
          <p className="text-sm font-semibold uppercase tracking-[0.32em] text-sky-300">Favoritos</p>
          <h3 className="mt-4 text-2xl font-semibold text-white">Salve seus trechos</h3>
          <p className="mt-3 text-sm leading-6 text-slate-300">
            Marque versos e retome a leitura rapidamente a partir do seu painel pessoal.</p>
        </div>
      </section>

      <section className="mt-10 rounded-[2rem] border border-border bg-card/80 p-8 shadow-sm shadow-black/5">
        <div className="grid gap-6 lg:grid-cols-[1fr_0.8fr] lg:items-center">
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.32em] text-primary">Comece</p>
            <h2 className="mt-3 text-3xl font-semibold tracking-tight text-foreground">Sua próxima leitura está a um clique</h2>
            <p className="mt-4 max-w-2xl text-base leading-7 text-muted-foreground">
              Use o menu de versão no cabeçalho e comece a leitura por capítulos. Tudo foi otimizado para mobile e tablets.
            </p>
          </div>
          <div className="flex flex-wrap gap-3">
            <Link href="/livros" className={cn(buttonVariants())}>
              Ler Bíblia
            </Link>
            <Link href="/buscar" className={cn(buttonVariants({ variant: "outline" }))}>
              Buscar agora
            </Link>
          </div>
        </div>
      </section>
    </main>
  );
}
