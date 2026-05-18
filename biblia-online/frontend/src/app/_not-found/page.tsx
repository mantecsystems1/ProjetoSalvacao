export const metadata = {
  title: "Página não encontrada",
};

export default function NotFound() {
  return (
    <main className="mx-auto max-w-4xl p-8 text-center">
      <h1 className="text-3xl font-semibold">Página não encontrada</h1>
      <p className="mt-4 text-sm text-muted-foreground">A rota solicitada não foi encontrada.</p>
    </main>
  );
}
