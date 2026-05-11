export function getPublicApiUrl() {
  return process.env.NEXT_PUBLIC_API_URL ?? "https://api.biblia.portalmantec.com.br";
}

export function getSiteUrl() {
  return process.env.NEXT_PUBLIC_SITE_URL ?? "https://biblia.portalmantec.com.br";
}
