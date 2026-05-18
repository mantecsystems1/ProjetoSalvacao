import type { Metadata, Viewport } from "next";
import "./globals.css";
import { Providers } from "@/app/providers";
import { SiteHeader } from "@/components/site-header";
import { getSiteUrl } from "@/lib/env";

export const metadata: Metadata = {
  metadataBase: new URL(getSiteUrl()),
  applicationName: "Biblia Online",
  title: {
    default: "Biblia Online — plataforma moderna para leitura bíblica",
    template: "%s · Biblia Online",
  },
  description:
    "Leitura bíblica responsiva com busca instantânea, tema claro/escuro, favoritos, histórico e PWA.",
  keywords: ["Bíblia", "leitura", "versão bíblica", "PWA", "Next.js", "SEO"],
  authors: [{ name: "Biblia Online" }],
  manifest: "/manifest.json",
  icons: {
    icon: [
      { url: "/icons/icon-192.svg", sizes: "192x192", type: "image/svg+xml" },
      { url: "/icons/icon-512.svg", sizes: "512x512", type: "image/svg+xml" },
    ],
    apple: [{ url: "/icons/apple-touch-icon.svg", sizes: "180x180", type: "image/svg+xml" }],
    other: [{ rel: "mask-icon", url: "/icons/maskable-icon-512.svg", color: "#0b1220" }],
  },
  openGraph: {
    title: "Biblia Online",
    description: "Leitura bíblica responsiva com busca instantânea, favoritos, histórico e PWA.",
    url: getSiteUrl(),
    siteName: "Biblia Online",
    type: "website",
    locale: "pt_BR",
  },
  twitter: {
    card: "summary_large_image",
  },
};

export const viewport: Viewport = {
  themeColor: "#0b1220",
  width: "device-width",
  initialScale: 1,
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="pt-BR" suppressHydrationWarning>
      <body className="min-h-dvh bg-background font-sans antialiased">
        <Providers>
          <SiteHeader />
          <main>{children}</main>
        </Providers>
      </body>
    </html>
  );
}
