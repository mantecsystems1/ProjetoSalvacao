"use client";

import { useEffect, useMemo, useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { api } from "@/lib/api-client";
import { useAuthStore } from "@/stores/auth-store";
import type { Chapter } from "@/lib/types";

function buildChapterText(chapter: Chapter) {
  return chapter.verses.map((verse) => `${verse.verseNumber}. ${verse.text}`).join(" ");
}

function getPreferredVoice(voices: SpeechSynthesisVoice[], versionCode: string | undefined) {
  const normalizedCode = versionCode?.toLowerCase() ?? "";

  // Detectar idioma baseado no código da versão
  const isEnglishVersion = normalizedCode.includes("kjv") || normalizedCode.includes("en") || normalizedCode === "kjv2006";
  const isPortugueseVersion = normalizedCode.includes("bsb") || normalizedCode.includes("pt") || normalizedCode === "bsb";

  if (isEnglishVersion) {
    // Preferir voz em inglês
    const englishVoice = voices.find((voice) => voice.lang.toLowerCase().startsWith("en"));
    return englishVoice ?? voices[0] ?? null;
  }

  if (isPortugueseVersion) {
    // Preferir voz em português
    const portugueseVoice = voices.find((voice) => voice.lang.toLowerCase().startsWith("pt"));
    return portugueseVoice ?? voices[0] ?? null;
  }

  // Fallback: tentar português primeiro, depois inglês
  const preferPt = voices.find((voice) => voice.lang.toLowerCase().startsWith("pt"));
  const preferEn = voices.find((voice) => voice.lang.toLowerCase().startsWith("en"));
  return preferPt ?? preferEn ?? voices[0] ?? null;
}

export function ReaderVerseList({ chapter }: { chapter: Chapter }) {
  const token = useAuthStore((s) => s.token);
  const [isSpeaking, setIsSpeaking] = useState(false);
  const [voices, setVoices] = useState<SpeechSynthesisVoice[]>([]);
  const [voiceLang, setVoiceLang] = useState<string>("pt-BR");
  const [ttsSupported, setTtsSupported] = useState(false);
  const [ttsError, setTtsError] = useState<string | null>(null);

  const touchHistory = useMutation({
    mutationFn: async () => {
      await api.post("/me/reading-history", {
        bibleVersionId: chapter.bibleVersionId,
        bookId: chapter.bookId,
        chapterNumber: chapter.chapterNumber,
      });
    },
  });

  const addFavorite = useMutation({
    mutationFn: async (verseNumber: number) => {
      await api.post("/me/favorites", {
        bibleVersionId: chapter.bibleVersionId,
        bookId: chapter.bookId,
        chapterNumber: chapter.chapterNumber,
        verseNumber,
      });
    },
  });

  const chapterText = useMemo(() => buildChapterText(chapter), [chapter]);

  useEffect(() => {
    if (typeof window === "undefined" || !("speechSynthesis" in window)) {
      setTtsSupported(false);
      return;
    }

    setTtsSupported(true);

    const synth = window.speechSynthesis;
    const updateVoices = () => {
      const availableVoices = synth.getVoices();
      setVoices(availableVoices);
      const preferred = getPreferredVoice(availableVoices, chapter.versionCode);
      if (preferred) {
        setVoiceLang(preferred.lang);
      }
    };

    updateVoices();
    synth.addEventListener("voiceschanged", updateVoices);
    return () => synth.removeEventListener("voiceschanged", updateVoices);
  }, [chapter.versionCode]);

  useEffect(() => {
    return () => {
      if (typeof window !== "undefined" && "speechSynthesis" in window) {
        window.speechSynthesis.cancel();
      }
    };
  }, []);

  const handleSpeakToggle = () => {
    if (typeof window === "undefined" || !("speechSynthesis" in window)) {
      setTtsSupported(false);
      setTtsError("Seu navegador não suporta síntese de voz.");
      return;
    }

    const synth = window.speechSynthesis;
    if (isSpeaking) {
      synth.cancel();
      setIsSpeaking(false);
      return;
    }

    if (!chapterText.trim()) {
      setTtsError("Conteúdo do capítulo vazio.");
      return;
    }

    const utterance = new SpeechSynthesisUtterance(chapterText);

    // Definir idioma baseado na versão
    const normalizedCode = chapter.versionCode?.toLowerCase() ?? "";
    const isEnglishVersion = normalizedCode.includes("kjv") || normalizedCode.includes("en") || normalizedCode === "kjv2006";
    const isPortugueseVersion = normalizedCode.includes("bsb") || normalizedCode.includes("pt") || normalizedCode === "bsb";

    if (isEnglishVersion) {
      utterance.lang = "en-US";
    } else if (isPortugueseVersion) {
      utterance.lang = "pt-BR";
    } else {
      utterance.lang = voiceLang;
    }

    const preferredVoice = getPreferredVoice(voices, chapter.versionCode);
    if (preferredVoice) {
      utterance.voice = preferredVoice;
    }
    utterance.rate = 0.95;
    utterance.onend = () => {
      setIsSpeaking(false);
    };
    utterance.onerror = () => {
      setTtsError("Houve um erro ao reproduzir o áudio.");
      setIsSpeaking(false);
    };

    synth.cancel();
    synth.speak(utterance);
    setIsSpeaking(true);
    setTtsError(null);
  };

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 rounded-lg border bg-muted/30 p-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p className="text-sm font-semibold">Ouvir capítulo</p>
          <p className="text-sm text-muted-foreground">
            Reproduz o texto em voz alta usando a síntese de fala do navegador.
          </p>
        </div>
        <div className="flex flex-wrap items-center gap-3">
          <Button size="sm" variant="secondary" onClick={handleSpeakToggle}>
            {isSpeaking ? "Parar leitura" : "Ouvir capítulo"}
          </Button>
          {!ttsSupported ? (
            <span className="text-sm text-red-600">Áudio não suportado neste navegador.</span>
          ) : null}
        </div>
      </div>

      {ttsError ? (
        <div className="rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
          {ttsError}
        </div>
      ) : null}

      {token ? (
        <div className="flex items-center justify-between gap-3 rounded-lg border bg-muted/30 p-3">
          <p className="text-sm text-muted-foreground">
            Autenticado: registre este capítulo no seu histórico de leitura.
          </p>
          <Button
            size="sm"
            variant="secondary"
            disabled={touchHistory.isPending}
            onClick={() => touchHistory.mutate()}
          >
            Registrar leitura
          </Button>
        </div>
      ) : (
        <p className="text-sm text-muted-foreground">
          Faça login para salvar histórico e favoritos por versículo.
        </p>
      )}

      <Separator />

      <div className="space-y-6">
        {chapter.verses.map((v) => (
          <section key={v.verseNumber} className="flex gap-3">
            <div className="w-10 shrink-0 pt-1 text-right text-sm font-semibold text-muted-foreground">
              {v.verseNumber}
            </div>
            <div className="flex-1 space-y-2">
              <p className="leading-relaxed text-[17px]">{v.text}</p>
              {token ? (
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  disabled={addFavorite.isPending}
                  onClick={() => addFavorite.mutate(v.verseNumber)}
                >
                  Favoritar versículo
                </Button>
              ) : null}
            </div>
          </section>
        ))}
      </div>
    </div>
  );
}
