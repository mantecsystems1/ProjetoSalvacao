import type { NextConfig } from "next";
import withPWAInit from "@ducanh2912/next-pwa";

const withPWA = withPWAInit({
  dest: "public",
  disable: process.env.NODE_ENV === "development",
  register: true,
  fallbacks: {
    document: "/offline.html",
    image: "/icons/icon-192.svg",
  },
});

const nextConfig: NextConfig = {
  output: "standalone",
};

export default process.env.ENABLE_PWA === "1" ? withPWA(nextConfig) : nextConfig;
