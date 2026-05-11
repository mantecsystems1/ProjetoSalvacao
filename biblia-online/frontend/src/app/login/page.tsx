"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { Button, buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Separator } from "@/components/ui/separator";
import { api } from "@/lib/api-client";
import { useAuthStore } from "@/stores/auth-store";

type AuthResponse = {
  accessToken: string;
  expiresAtUtc: string;
  user: { id: string; email: string; displayName: string };
};

export default function LoginPage() {
  const router = useRouter();
  const setSession = useAuthStore((s) => s.setSession);

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [displayName, setDisplayName] = useState("");
  const [error, setError] = useState<string | null>(null);

  async function login() {
    setError(null);
    try {
      const res = await api.post<AuthResponse>("/auth/login", { email, password });
      const data = res.data;
      setSession(data.accessToken, data.expiresAtUtc, data.user);
      router.push("/conta");
      router.refresh();
    } catch {
      setError("Não foi possível entrar.");
    }
  }

  async function register() {
    setError(null);
    try {
      const res = await api.post<AuthResponse>("/auth/register", { email, password, displayName });
      const data = res.data;
      setSession(data.accessToken, data.expiresAtUtc, data.user);
      router.push("/conta");
      router.refresh();
    } catch {
      setError("Não foi possível registrar (email em uso ou dados inválidos).");
    }
  }

  return (
    <div className="mx-auto max-w-lg space-y-6 px-4 py-10">
      <header className="space-y-2">
        <h1 className="text-3xl font-semibold tracking-tight">Entrar</h1>
        <p className="text-muted-foreground">
          JWT emitido pela API (.NET). Sem Firebase e sem APIs pagas.
        </p>
      </header>

      <Card>
        <CardHeader>
          <CardTitle>Acesso</CardTitle>
          <CardDescription>Use email e senha.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input id="email" autoComplete="email" value={email} onChange={(e) => setEmail(e.target.value)} />
          </div>
          <div className="space-y-2">
            <Label htmlFor="password">Senha</Label>
            <Input
              id="password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <div className="flex gap-2">
            <Button type="button" onClick={login}>
              Entrar
            </Button>
            <Link href="/" className={cn(buttonVariants({ variant: "outline" }))}>
              Voltar
            </Link>
          </div>

          <Separator />

          <div className="space-y-2">
            <CardTitle className="text-base">Criar conta</CardTitle>
            <Label htmlFor="displayName">Nome</Label>
            <Input id="displayName" value={displayName} onChange={(e) => setDisplayName(e.target.value)} />
            <Button type="button" variant="secondary" onClick={register}>
              Registrar
            </Button>
          </div>

          {error ? <p className="text-sm text-red-600">{error}</p> : null}
        </CardContent>
      </Card>
    </div>
  );
}
